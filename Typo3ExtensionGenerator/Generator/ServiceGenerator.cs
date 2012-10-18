using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;
using Action = Typo3ExtensionGenerator.Model.Plugin.Action;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates service classes
  /// </summary>
  public class ServiceGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ServiceGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      if( null == Subject.Services || !Subject.Services.Any() ) return;

      Log.Info( "Generating services..." );

      foreach( Service service in Subject.Services ) {
        GenerateService( service );
      }
    }

    private void GenerateService( Service service ) {
      string className = NameHelper.GetExtbaseServiceClassName( Subject, service );
      Log.InfoFormat( "Generating service '{0}'...", className );

      #region Generate Methods
      StringBuilder methods = new StringBuilder();
      const string methodTemplate = "/**\n" +
                                    "{2}" +
                                    "*/\n" +
                                    "public function {0}({1}) {{ return $this->getImplementation()->{0}({1}); }}\n";

      foreach( Action method in service.Actions ) {
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
              "Assuming requirement '{0}' for service method '{1}:{2}' to be of type '{3}'.", requirement, service.Name,
              method.Name, typeName );
          }
          phpDoc = phpDoc + ( "* @param " + typeName + " $" + requirement + "\n" );
        }

        // Prefix each parameter with a $ and join them together with , in between.
        string parameters = method.Requirements.Aggregate(
          string.Empty,
          ( current, requirement ) =>
          current + ( "$" + requirement + ( ( requirement != method.Requirements.Last() ) ? "," : string.Empty ) ) );

        methods.Append( String.Format( methodTemplate, method.Name, parameters, phpDoc ) );
      }
      #endregion

      #region Handle External Implementations
      bool isExternallyImplemented   = false;
      string implementationClassname = string.Empty;
      string implementationFilename  = string.Empty;
      if( !string.IsNullOrEmpty( service.Implementation ) ) {
        isExternallyImplemented = true;
        implementationClassname = NameHelper.GetExtbaseServiceImplementationClassName( Subject, service );
        implementationFilename  = NameHelper.GetExtbaseServiceImplementationFileName( Subject, service );
      }

      if( isExternallyImplemented ) {
        if( !File.Exists( service.Implementation ) ) {
          throw new GeneratorException(
            string.Format( "Implementation '{0}' for service '{1}' does not exist.", service.Implementation, service.Name ),
            service.SourceFragment.SourceDocument );
        }
        Log.InfoFormat( "Merging implementation '{0}'...", service.Implementation );
        string serviceImplementationContent = File.ReadAllText( service.Implementation );
        if( !Regex.IsMatch( serviceImplementationContent, String.Format( "class {0} ?({{|extends|implements)", implementationClassname ) ) ) {
          Log.WarnFormat( "The class name of your implementation MUST be '{0}'!", implementationClassname );  
        }
        WriteFile( "Classes/Service/" + implementationFilename, serviceImplementationContent, DateTime.UtcNow );

      } else {
        if( service.Actions.Count > 0 ) {
          Log.WarnFormat(
            "Service '{0}' defines actions, but has no implementation provided. If any of these actions is invoked by TYPO3, a PHP error will be generated!",
            service.Name );
        }
      }
      #endregion

      const string serviceImplementationTemplate = "private $implementation;\n" +
                                                   "private function getImplementation() {{\n" +
                                                   "  if( null == $this->implementation ) {{\n" +
                                                   "    $this->implementation = new {implClassname}($this);\n" +
                                                   "  }}\n" +
                                                   "  return $this->implementation;\n" +
                                                   "}}\n" +
                                                   "function __construct() {{\n" +
                                                   "}}\n";

      string serviceImplementation = serviceImplementationTemplate.FormatSmart(
          new {
                implClassname = implementationClassname,
                className     = NameHelper.GetExtbaseServiceClassName( Subject, service )
              } );

      const string serviceTemplate = "class {_className} {{\n" +
                                     "{_serviceProperties}\n" +
                                     "{_serviceActions}\n" +
                                     "}}\n" +
                                     "{_requireImplementation}";

      string serviceResult = serviceTemplate.FormatSmart(
          new {
                _className             = NameHelper.GetExtbaseServiceClassName( Subject, service ),
                _serviceProperties     = (( isExternallyImplemented ) ? serviceImplementation : string.Empty),
                _serviceActions        = methods.ToString(),
                _requireImplementation = ( isExternallyImplemented ) ? string.Format( "require_once('{0}');\n", implementationFilename ) : string.Empty
              } );

      WritePhpFile(
        string.Format( "Classes/Service/{0}", NameHelper.GetExtbaseServiceFileName( Subject, service ) ),
        serviceResult, DateTime.UtcNow );
    }
  }
}
