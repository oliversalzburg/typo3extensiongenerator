using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using StringLib;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class ConfigurationFileGenerator : AbstractGenerator, IGenerator {
    public ConfigurationFileGenerator( string outputDirectory, Extension extension, Typo3ExtensionGenerator.Model.Configuration configuration ) : base( outputDirectory, extension ) {
      Configuration = configuration;
    }

    public string TargetFile {
      get { return "Configuration/TCA/" + NameHelper.GetExtbaseFileName( Subject, Configuration.Model ); }
    }

    public Typo3ExtensionGenerator.Model.Configuration Configuration { get; private set; }

    public void Generate() {
      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );
      
      WritePhpFile( TargetFile, GeneratePhp() );
    }

    /// <summary>
    /// Generates the PHP statements for the full TCA configuration of a model/configuration.
    /// </summary>
    /// <returns></returns>
    private string GeneratePhp() {
      const string template = "if( !defined( 'TYPO3_MODE' ) ) {{\n" +
                              "  die( 'Access denied.' );\n" +
                              "}}\n" +
                              "$TCA['{model}'] = array(\n" +
                              "  'ctrl' => $TCA['{model}']['ctrl'],\n" +
                              "{interfaceFields}" +
                              ");";

      // Describes which fields (and in which order) are shown in the Info/View Item dialog in the BE.
      const string infoInterfaceTemplate = "'interface' => array(\n" +
                                           "	'showRecordFieldList' => '{0}'\n" +
                                           ")";

      // title, download_category, file_name, description, install_notes, qualifier

      // Were the T3CommonFields included in this model?
      string finalInterfaceFields = string.Empty;
      if( Configuration.Model.Members.Any( m => m.Key == Keywords.DataModelTemplate && m.Value == Keywords.DataModelTemplates.T3CommonFields ) ) {
        finalInterfaceFields += T3TranslationFields.InterfaceInfoFields + ", ";
      }

      // Were the T3TranslationFields included in this model?
      if( Configuration.Model.Members.Any( m => m.Key == Keywords.DataModelTemplate && m.Value == Keywords.DataModelTemplates.T3TranslationFields ) ) {
        finalInterfaceFields += T3TranslationFields.InterfaceInfoFields + ", ";
      }

      // Validate and translate the user-supplied interface members
      if( null != Configuration.InterfaceInfo ) {
        string[] userDefinedInterfaceFields = Configuration.InterfaceInfo.Split( new[] {','} );
        foreach( string interfaceField in userDefinedInterfaceFields ) {
          string field = interfaceField;
          KeyValuePair<string, string> referencedModelMember =
            Configuration.Model.Members.SingleOrDefault( m => m.Value == field );

          if( null == referencedModelMember.Key ) {
            throw new GeneratorException(
              string.Format(
                "The interface field '{0}' does not exist in the data model '{1}'.", interfaceField,
                Configuration.Model.Name ) );
          }

          finalInterfaceFields += NameHelper.GetSqlColumnName( Subject, interfaceField ) + ",";
        }
      }

      // Cut off trailing comma and unify spacing
      finalInterfaceFields = finalInterfaceFields.TrimEnd( new[] {',',' '} );
      finalInterfaceFields = Regex.Replace( finalInterfaceFields, ", *", ", " );

      string finalInterface = string.Format( infoInterfaceTemplate, finalInterfaceFields );

      var dataObject = new {
                               extensionKey = Subject.Key,
                               model = NameHelper.GetAbsoluteModelName( Subject, Configuration.Model ),
                               interfaceFields = finalInterface
                             };
      string generatedConfiguration = template.HaackFormat( dataObject );
      return generatedConfiguration;
    }
  }
}
