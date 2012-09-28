using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using log4net;
using Action = Typo3ExtensionGenerator.Model.Plugin.Action;

namespace Typo3ExtensionGenerator.Generator.Model {
  /// <summary>
  /// Generates files that are closely related to data models.
  /// Includes ExtBase models, repositories and Fluid partials.
  /// </summary>
  public class ModelGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ModelGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      GenerateSql();

      if( null != Subject.Repositories ) {
        foreach( Repository repository in Subject.Repositories ) {
          DataModel repositoryModel = Subject.Models.SingleOrDefault( m => m.Name == repository.TargetModelName );
          if( null == repositoryModel ) {
            throw new GeneratorException(
              string.Format( "The target type '{0}' for the repository is not defined.", repository.TargetModelName ),
              repository.SourceFragment.SourceDocument);
          }
        }
      }

      if( null != Subject.Models ) {
        foreach( DataModel dataModel in Subject.Models ) {
          const string modelPath      = "Classes/Domain/Model";
          const string repositoryPath = "Classes/Domain/Repository";
          const string partialsPath   = "Resources/Private/Partials";

          string modelFileContent      = GenerateModelFile( dataModel );
          string repositoryFileContent = GenerateRepositoryFile( dataModel );
          string fluidPartialContent   = GenerateFluidPartial( dataModel );

          string modelFilename = Path.Combine( modelPath, NameHelper.GetExtbaseDomainModelFileName( Subject, dataModel ) );
          Log.InfoFormat( "Generating model '{0}'...", modelFilename );
          if( !string.IsNullOrEmpty( modelFileContent ) ) {
            WritePhpFile( modelFilename, modelFileContent, DateTime.UtcNow );
          }

          string respositoryFilename = Path.Combine( repositoryPath, NameHelper.GetExtbaseDomainModelRepositoryFileName( Subject, dataModel ) );
          Log.InfoFormat( "Generating repository '{0}'...", respositoryFilename );
          if( !string.IsNullOrEmpty( repositoryFileContent ) ) {
            WritePhpFile( respositoryFilename, repositoryFileContent, DateTime.UtcNow );
          }

          string fluidPartialFilename = Path.Combine( partialsPath, NameHelper.GetFluidPartialFileName( Subject, dataModel ) );
          Log.InfoFormat( "Generating Fluid partial '{0}'...", fluidPartialFilename );
          if( !string.IsNullOrEmpty( fluidPartialContent ) ) {
            WriteFile( fluidPartialFilename, fluidPartialContent, DateTime.UtcNow );
          }
        }
      }
    }

    /// <summary>
    /// Generate the ExtBase model file for the given data model.
    /// </summary>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    private string GenerateModelFile( DataModel dataModel ) {
      // If this model implements an internal type, we don't need to generate anything.
      if( !string.IsNullOrEmpty( dataModel.InternalType ) ) {
        return string.Empty;
      }

      const string template =
        "class {className} extends Tx_Extbase_DomainObject_AbstractEntity {{\n" +
        "{properties}" +
        "{gettersSetters}" +
        "}}";


      const string propertyTemplate = "/**\n" +
                                      "* {0}\n" +
                                      "* @var {1}\n" +
                                      "*/\n" +
                                      "protected ${0};\n";

      StringBuilder dataMembers = new StringBuilder();
      foreach( DataModel.DataModelMember member in dataModel.Members ) {
        if( member.Name == Keywords.DataModelTemplate ) continue;
        bool containsKey = dataModel.ForeignModels.ContainsKey( member.Value );
        if( containsKey ) {
          DataModel foreignModel = dataModel.ForeignModels[ member.Value ];
          // Make sure to use the internal type name if it is defined.
          string foreignModelClassName = foreignModel.InternalType
                                         ??
                                         NameHelper.GetExtbaseDomainModelClassName( Subject, dataModel.ForeignModels[ member.Name ] );
          dataMembers.Append( string.Format( propertyTemplate, member.Value, foreignModelClassName ) );   

        } else {
          dataMembers.Append( string.Format( propertyTemplate, member.Value, TypeTranslator.ToPhp( member.Name, dataModel.SourceFragment.SourceDocument ) ) );   
        }
        
      }

      const string accessor = "public function get{1}() {{" +
                              "	 return $this->{0};" +
                              "}}" +
                              "public function set{1}( ${0} ) {{" +
                              "	 $this->{0} = ${0};" +
                              "}}\n";

      StringBuilder accessors = new StringBuilder();
      foreach( DataModel.DataModelMember member in dataModel.Members ) {
        accessors.Append( string.Format( accessor, member.Value, NameHelper.UpperCamelCase( member.Value ) ) );
      }

      var dataObject = new {
                             className = NameHelper.GetExtbaseDomainModelClassName( Subject, dataModel ),
                             properties = dataMembers,
                             gettersSetters = accessors
                           };

      return template.FormatSmart( dataObject );
    }

    /// <summary>
    /// Generates an ExtBase repository for the given data model.
    /// This allows ExtBase controllers to retrieve data records from the database.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private string GenerateRepositoryFile( DataModel model ) {
      
      const string repositoryImplementationTemplate = "private $implementation;\n" +
                                                      "private function getImplementation() {{\n" +
                                                      "  if( null == $this->implementation ) {{\n" +
                                                      "    $this->implementation = new {implClassname}($this);\n" +
                                                      "  }}\n" +
                                                      "  return $this->implementation;\n" +
                                                      "}}\n";

      
      string implementationClassname = NameHelper.GetExtbaseDomainModelRepositoryImplementationClassName( Subject, model ); 
      string implementationFilename = NameHelper.GetExtbaseDomainModelRepositoryImplementationFileName( Subject, model );

      // Did the user define additional information for our repository
      bool isExternallyImplemented = false;
      Repository repositoryDefinition = Subject.Repositories.SingleOrDefault( r => r.TargetModelName == model.Name );

      // If the repository type is an internal type, no need to generate anything
      if( null != repositoryDefinition && !string.IsNullOrEmpty( repositoryDefinition.InternalType ) ) {
        return string.Empty;
      }

      if( null != repositoryDefinition && !string.IsNullOrEmpty( repositoryDefinition.Implementation ) ) {
        if( !File.Exists( repositoryDefinition.Implementation ) ) {
          throw new GeneratorException(
            string.Format(
              "Implementation '{0}' for repository for '{1}' does not exist.", repositoryDefinition.Implementation,
              model.Name ),
            repositoryDefinition.SourceFragment.SourceDocument );
        }

        isExternallyImplemented = true;

        Log.InfoFormat( "Merging implementation '{0}'...", repositoryDefinition.Implementation );
        string repositoryImplementationContent = File.ReadAllText( repositoryDefinition.Implementation );
        DateTime lastWriteTimeUtc = new FileInfo( repositoryDefinition.Implementation ).LastWriteTimeUtc;
        if( !Regex.IsMatch( repositoryImplementationContent, String.Format( "class {0} ?({{|extends|implements)", implementationClassname ) ) ) {
          Log.WarnFormat( "The class name of your implementation MUST be '{0}'!", implementationClassname );
        }

        WriteFile( "Classes/Domain/Repository/" + implementationFilename, repositoryImplementationContent, lastWriteTimeUtc );
      }

      // Generate methods as described in definition
      StringBuilder actions = new StringBuilder();
      if( null != repositoryDefinition ) {
        const string actionTemplate = "public function {0}({1}) {{ return $this->getImplementation()->{0}({1}); }}\n";

        foreach( Action action in repositoryDefinition.Methods ) {
          // Prefix each parameter with a $ and join them together with , in between.
          string parameters = action.Requirements.Aggregate(
            string.Empty,
            ( current, requirement ) =>
            current + ( "$" + requirement + ( ( requirement != action.Requirements.Last() ) ? "," : string.Empty ) ) );

          actions.Append( String.Format( actionTemplate, action.Name, parameters ) );
        }
      }

      string repositoryImplementation =
        repositoryImplementationTemplate.FormatSmart(
          new {
                implClassname = implementationClassname,
                className     = NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, model )
              } );

      // Generate final class
      const string template = "class {className} extends Tx_Extbase_Persistence_Repository {{\n" +
                              "{repositoryMethods}\n" +
                              "}}\n" +
                              "{requireImplementation}";

      return
        template.FormatSmart(
          new {
                className = NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, model ),
                repositoryMethods = repositoryImplementation + actions,
                requireImplementation = ( isExternallyImplemented ) ? string.Format( "require_once('{0}');\n", implementationFilename ) : string.Empty
              } );
    }

    /// <summary>
    /// Generates a placeholder Fluid partial that will render a given data model.
    /// </summary>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    private string GenerateFluidPartial( DataModel dataModel ) {
      const string modelTemplate               = "<div class=\"tx-{cssRoot}-item\">\n{fieldsList}</div>\n";
      const string fieldTemplate               = "<div>{{{fieldAccessor}}}</div>\n";

      StringBuilder fieldsCollector = new StringBuilder();
      foreach( DataModel.DataModelMember member in dataModel.Members ) {
        fieldsCollector.Append( fieldTemplate.FormatSmart( new {fieldAccessor = dataModel.Name + "." + member.Value} ) );
      }
      string fields = fieldsCollector.ToString();

      string model =
        modelTemplate.FormatSmart(
          new {fieldsList = fields, cssRoot = NameHelper.UpperCamelCase( Subject.Key ).ToLower()} );

      return model;
    }

    /// <summary>
    /// Generates the SQL statements for the data model tables
    /// </summary>
    /// <returns></returns>
    private void GenerateSql( ) {
      if( null == Subject.Models || !Subject.Models.Any() ) return;

      string result = string.Empty;

      Log.Info( "Generating SQL tables..." );

      const string template = "CREATE TABLE {0} (\n{1}\n);";
      foreach( DataModel dataModel in Subject.Models ) {
        // Don't generate SQL table for internally implemented types
        if( !string.IsNullOrEmpty( dataModel.InternalType ) ) {
          continue;
        }

        string modelName = NameHelper.GetAbsoluteModelName( Subject, dataModel );
        Log.InfoFormat( "Generating SQL table '{0}'...", modelName );

        string sqlMembers = GenerateSqlMembers( dataModel );
        result += string.Format( template, modelName, sqlMembers ) + "\n";
      }

      WriteFile( "ext_tables.sql", result, DateTime.UtcNow );
    }

    private string GenerateSqlMembers( DataModel dataModel ) {
      StringBuilder dataMembers = new StringBuilder();
      StringBuilder keys = new StringBuilder();
      foreach( DataModel.DataModelMember member in dataModel.Members ) {
        // Is this a template request or a normal data member?
        if( Keywords.DataModelTemplate == member.Name ) {
          switch( member.Value ) {
            case Keywords.DataModelTemplates.T3ManagedFields:
              dataMembers.Append( T3ManagedFields.Content + ",\n" );
              keys.Append( T3ManagedFields.Keys + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3CommonFields:
              dataMembers.Append( T3CommonFields.Content + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3VersioningFields:
              dataMembers.Append( T3VersioningFields.Content + ",\n" );
              keys.Append( T3VersioningFields.Keys + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3TranslationFields:
              dataMembers.Append( T3TranslationFields.Content + ",\n" );
              keys.Append( T3TranslationFields.Keys + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3Sortable:
              dataMembers.Append( T3Sortable.Content + ",\n" );
              break;

            default:
              throw new GeneratorException(
                string.Format( "Data model template '{0}' is unknown", member.Value ),
                dataModel.SourceFragment.SourceDocument );
          }
        } else {
          if( dataModel.ForeignModels.ContainsKey( member.Value ) ) {
            // For a foreign key, we just insert the default uint
            dataMembers.Append(
              string.Format(
                "{0} {1},\n", NameHelper.GetSqlColumnName( Subject, member.Value ),
                TypeTranslator.ToSql( Keywords.Types.UnsignedInt, dataModel.SourceFragment.SourceDocument ) ) );

          } else if( TypeTranslator.CanTranslate( member.Name ) ) {
            // If it is a POD type, just translate it
            dataMembers.Append(
              string.Format(
                "{0} {1},\n", NameHelper.GetSqlColumnName( Subject, member.Value ),
                TypeTranslator.ToSql( member.Name, dataModel.SourceFragment.SourceDocument ) ) );

          } else {
            throw new GeneratorException(
              string.Format( "Data model field type '{0}' in model '{1}' is unknown.", member.Name, dataModel.Name ),
              dataModel.SourceFragment.SourceDocument );
          }
        }
      }
      String dataMembersAndKeys = string.Format(
        "{0},\n{1}", (dataMembers.Length >0) ? dataMembers.ToString().Substring( 0, dataMembers.Length - 2 ) : string.Empty,
        ( keys.Length > 0 ) ? keys.ToString().Substring( 0, keys.Length - 2 ) : string.Empty );

      return dataMembersAndKeys;
    }
    
  }
}
