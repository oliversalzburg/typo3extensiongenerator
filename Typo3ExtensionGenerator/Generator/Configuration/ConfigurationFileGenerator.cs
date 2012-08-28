using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using StringLib;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using Type = Typo3ExtensionGenerator.Model.Configuration.Type;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class ConfigurationFileGenerator : AbstractGenerator, IGenerator {
    public ConfigurationFileGenerator( string outputDirectory, Extension extension, Typo3ExtensionGenerator.Model.Configuration.Configuration configuration ) : base( outputDirectory, extension ) {
      Configuration = configuration;
    }

    public string TargetFile {
      get { return "Configuration/TCA/" + NameHelper.GetExtbaseFileName( Subject, Configuration.Model ); }
    }

    public Typo3ExtensionGenerator.Model.Configuration.Configuration Configuration { get; private set; }

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
                              "{interfaceFields},\n" +
                              "{types},\n" +
                              ");";

      string finalInterface = GenerateInterface();
      string finalTypes     = GenerateTypes();

      var dataObject = new {
                             extensionKey = Subject.Key,
                             model = NameHelper.GetAbsoluteModelName( Subject, Configuration.Model ),
                             interfaceFields = finalInterface,
                             types = finalTypes
                           };

      string generatedConfiguration = template.HaackFormat( dataObject );
      return generatedConfiguration;
    }

    private string GenerateInterface() {
      // Describes which fields (and in which order) are shown in the Info/View Item dialog in the BE.
      const string infoInterfaceTemplate = "  'interface' => array( 'showRecordFieldList' => '{0}' )";

      // Were the T3CommonFields included in this model?
      string finalInterfaceFields = string.Empty;
      if( Configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3CommonFields ) ) {
        finalInterfaceFields += T3TranslationFields.InterfaceInfoFields + ", ";
      }

      // Were the T3TranslationFields included in this model?
      if( Configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3TranslationFields ) ) { 
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
      finalInterfaceFields = finalInterfaceFields.TrimEnd( new[] {',', ' '} );
      finalInterfaceFields = Regex.Replace( finalInterfaceFields, ", *", ", " );

      string finalInterface = string.Format( infoInterfaceTemplate, finalInterfaceFields );
      return finalInterface;
    }

    private string GenerateTypes() {
      const string typesTemplate = "  'types' => array( {0} )";

      const string typeTemplate = "'{0}' => array( {1} )";

      // Describes which fields (and in which order) are shown in the BE when editing a record.
      const string typeInterfaceTemplate = "'showitem' => '{0}'";

      StringBuilder finalTypes = new StringBuilder();
      for( int typeIndex = 0; typeIndex < Configuration.Types.Count; typeIndex++ ) {
        Type type = Configuration.Types[ typeIndex ];

        string allTypes = string.Empty;
        if( Configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3TranslationFields ) ) {
          allTypes += T3TranslationFields.InterfaceTypeFields + ",";
        }

        // Translate user fields to SQL fields and add them to the interface
        string[] userInterfaceFields = type.Interface.Split( new[] {','} );
        foreach( string userInterfaceField in userInterfaceFields ) {
          string field = userInterfaceField;
          KeyValuePair<string, string> referencedModelMember =
            Configuration.Model.Members.SingleOrDefault( m => m.Value == field );

          if( null == referencedModelMember.Key ) {
            throw new GeneratorException(
              string.Format(
                "The type interface field '{0}' does not exist in the data model '{1}'.", userInterfaceField,
                Configuration.Model.Name ) );
          }

          allTypes += NameHelper.GetSqlColumnName( Subject, userInterfaceField ) + ",";
        }

        // Add Access tab
        allTypes = allTypes.TrimEnd( new[] {',', ' '} );
        allTypes +=
          ",--div--;LLL:EXT:cms/locallang_ttc.xml:tabs.access,--palette--;LLL:EXT:cms/locallang_ttc.xml:palette.access;paletteAccess";

        // Cut off trailing comma and unify spacing
        allTypes = allTypes.TrimEnd( new[] {',', ' '} );
        allTypes = Regex.Replace( allTypes, ", *", ", " );

        string typeInterface = String.Format( typeInterfaceTemplate, allTypes );
        finalTypes.Append( string.Format( typeTemplate, typeIndex + 1, typeInterface ) );
      }

      return String.Format( typesTemplate, finalTypes );
    }
  }
}
