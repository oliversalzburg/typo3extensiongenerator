using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Class.Naming;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;
using Action = Typo3ExtensionGenerator.Model.Action;

namespace Typo3ExtensionGenerator.Generator.Class {
  /// <summary>
  /// Helps to generate the proxy classes that TYPO3 Extension Generator uses to
  /// connect generated extension code to real code.
  /// </summary>
  class ClassProxyGenerator : AbstractGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Constructs a ClassProxyGenerator
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="subject">The extension.</param>
    public ClassProxyGenerator( Context context, Extension subject ) : base( context, subject ) {}

    /// <summary>
    /// Generates a class proxy.
    /// This is a class that defines stubs for all actions, references to repositories and so forth and calls into the users own implementation upon invokation.
    /// </summary>
    /// <param name="classTemplate">The class template that should be used to generate the class proxy.</param>
    /// <param name="namingStrategy">The naming strategy that should be used to generate names for all the entities the class uses.</param>
    /// <param name="classDirectory">Into which subdirectory the files should be placed.</param>
    /// <param name="addDependencies">Should dependencies be placed in the class proxy? If true, the class will contain references and injectors to all repositories in the extension.</param>
    /// <exception cref="GeneratorException">Implementation does not exist.</exception>
    public void GenerateClassProxy( IClassTemplate classTemplate, INamingStrategy namingStrategy, string classDirectory, bool addDependencies ) {
      string className = namingStrategy.GetExtbaseClassName( Subject, classTemplate );
      Log.InfoFormat( "Generating class '{0}'...", className );

      #region Generate Methods
      StringBuilder methods = new StringBuilder();
      const string methodTemplate = "/**\n" +
                                    "{_phpDoc}" +
                                    "*/\n" +
                                    "public function {_methodName}{_methodSuffix}({_parameters}) {{ return $this->getImplementation()->{_methodName}{_methodSuffix}({_parameters}); }}\n";

      foreach( Action method in classTemplate.Actions ) {
        // Start building up the PHPDoc for this action
        string phpDoc = string.Empty;
        foreach( string requirement in method.Requirements ) {
          string typeName = "mixed";
          // See if the name of the requirement matches the name of a defined model,
          // if so, we assume the user wants to reference that model.
          DataModel requiredModel = Subject.Models.SingleOrDefault( m => m.Name.ToLower() == requirement );
          if( requiredModel != null ) {
            typeName = NameHelper.GetExtbaseDomainModelClassName( Subject, requiredModel );
            Log.InfoFormat(
              "Assuming requirement '{0}' for method '{1}:{2}' to be of type '{3}'.", requirement, classTemplate.Name,
              method.Name, typeName );
          }
          phpDoc = phpDoc + ( "* @param " + typeName + " $" + requirement + "\n" );
        }

        // Prefix each parameter with a $ and join them together with , in between.
        string parameters = method.Requirements.Aggregate(
          string.Empty,
          ( current, requirement ) =>
          current + ( "$" + requirement + ( ( requirement != method.Requirements.Last() ) ? "," : string.Empty ) ) );

        var methodData = new { _phpDoc = phpDoc, _methodSuffix = namingStrategy.MethodSuffix, _methodName = method.Name, _parameters = parameters };

        methods.Append( methodTemplate.FormatSmart( methodData ) );
      }
      #endregion

      #region Handle External Implementations
      bool isExternallyImplemented   = false;
      string implementationClassname = string.Empty;
      string implementationFilename  = string.Empty;
      if( !string.IsNullOrEmpty( classTemplate.Implementation ) ) {
        isExternallyImplemented = true;
        implementationClassname = namingStrategy.GetExtbaseImplementationClassName( Subject, classTemplate );
        implementationFilename  = namingStrategy.GetExtbaseImplementationFileName( Subject, classTemplate );
      }

      if( isExternallyImplemented ) {
        if( !File.Exists( classTemplate.Implementation ) ) {
          throw new GeneratorException(
            string.Format( "Implementation '{0}' for '{1}' does not exist.", classTemplate.Implementation, classTemplate.Name ),
            classTemplate.SourceFragment.SourceDocument );
        }
        Log.InfoFormat( "Merging implementation '{0}'...", classTemplate.Implementation );
        string serviceImplementationContent = File.ReadAllText( classTemplate.Implementation );
        if( !Regex.IsMatch( serviceImplementationContent, String.Format( "class {0} ?({{|extends|implements)", implementationClassname ) ) ) {
          Log.WarnFormat( "The class name of your implementation for '{1}' MUST be '{0}'!", implementationClassname, classTemplate.Name );  
        }
        WriteFile( classDirectory + implementationFilename, serviceImplementationContent, DateTime.UtcNow );

      } else {
        if( classTemplate.Actions.Count > 0 ) {
          Log.WarnFormat(
            "Proxy '{0}' defines actions, but has no implementation provided. If any of these actions is invoked by TYPO3, a PHP error will be generated!",
            classTemplate.Name );
        }
      }
      #endregion

      #region Generate Properties
      StringBuilder propertiesList = new StringBuilder();
      if( Subject.Models != null && addDependencies ) {
        foreach( DataModel dataModel in Subject.Models ) {
          const string memberTemplate = "/**\n" +
                                        "* {0}Repository\n" +
                                        "* @var {1}\n" +
                                        "*/\n" +
                                        "protected ${0}Repository;\n";

          // Check if the repository is internally implemented.
          // An example for an internally implemented repository would be Tx_Extbase_Domain_Repository_FrontendUserRepository
          Repository repository = Subject.Repositories.SingleOrDefault( r => r.TargetModelName == dataModel.Name );
          if( null != repository && PurelyWrapsInternalType( repository ) ) {
            propertiesList.Append(
              String.Format(
                memberTemplate, dataModel.Name, repository.InternalType ) );

          } else {
            propertiesList.Append(
              String.Format(
                memberTemplate, dataModel.Name,
                NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, dataModel ) ) );
          }

          const string injectorTemplate =
            "/**\n" +
            "* inject{0}Repository\n" +
            "* @param {1} ${2}Repository\n" +
            "*/\n" +
            "public function inject{0}Repository({1} ${2}Repository) {{\n" +
            "  $this->{2}Repository = ${2}Repository;\n" +
            "}}\n";

          // Check again if the repository is internally implemented.
          // An example for an inernally implemented repository would be Tx_Extbase_Domain_Repository_FrontendUserRepository
          string injector = string.Empty;
          if( null != repository && PurelyWrapsInternalType( repository ) ) {
            injector = String.Format(
              injectorTemplate, NameHelper.UpperCamelCase( dataModel.Name ), repository.InternalType, dataModel.Name );

          } else {
            injector = String.Format(
              injectorTemplate, NameHelper.UpperCamelCase( dataModel.Name ),
              NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, dataModel ), dataModel.Name );
          }

          propertiesList.Append( injector );
        }
      }
      #endregion

      const string implementationTemplate = "private $implementation;\n" +
                                            "private function getImplementation() {{\n" +
                                            "  if( null == $this->implementation ) {{\n" +
                                            "    $this->implementation = new {_implClassname}($this);\n" +
                                            "  }}\n" +
                                            "  return $this->implementation;\n" +
                                            "}}\n" +
                                            "function __construct() {{\n" +
                                            "}}\n";

      string serviceImplementation = implementationTemplate.FormatSmart(
          new {
                _implClassname = implementationClassname
              } );

      const string template = "class {_className} extends Tx_Extbase_MVC_Controller_ActionController {{\n" +
                              "{_properties}\n" +
                              "{_actions}\n" +
                              "}}\n" +
                              "{_requireImplementation}";

      string serviceResult = template.FormatSmart(
          new {
                _className             = namingStrategy.GetExtbaseClassName( Subject, classTemplate ),
                _properties            = (( isExternallyImplemented ) ? serviceImplementation : string.Empty) + propertiesList,
                _actions               = methods.ToString(),
                _requireImplementation = ( isExternallyImplemented ) ? string.Format( "require_once('{0}');\n", implementationFilename ) : string.Empty
              } );

      WritePhpFile( Path.Combine( classDirectory, namingStrategy.GetExtbaseFileName( Subject, classTemplate ) ), serviceResult, DateTime.UtcNow );
    }

    /// <summary>
    /// Does this repository wrap an internal type WITHOUT providing an own implementation.
    /// This is usually the case when referencing repositories that already exist in TYPO3, like Tx_Extbase_Domain_Repository_FrontendUserRepository
    /// </summary>
    /// <param name="repository"></param>
    /// <returns></returns>
    private static bool PurelyWrapsInternalType( Repository repository ) {
      return !string.IsNullOrEmpty( repository.InternalType ) && string.IsNullOrEmpty( repository.Implementation );
    }
  }
}
