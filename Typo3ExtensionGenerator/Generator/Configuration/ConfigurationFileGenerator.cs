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
                              "{palettes},\n" +
                              "{columns}\n" +
                              ");";

      string finalInterface = GenerateInterface();
      string finalTypes     = GenerateTypes();
      string finalPalettes  = GeneratePalettes();
      string finalColumns   = GenerateColumns();

      var dataObject = new {
                             extensionKey = Subject.Key,
                             model = NameHelper.GetAbsoluteModelName( Subject, Configuration.Model ),
                             interfaceFields = finalInterface,
                             types = finalTypes,
                             palettes = finalPalettes,
                             columns = finalColumns
                           };

      string generatedConfiguration = template.HaackFormat( dataObject );
      return generatedConfiguration;
    }

    /// <summary>
    /// Generates the 'interface' array.
    /// </summary>
    /// <returns></returns>
    private string GenerateInterface() {
      // Describes which fields (and in which order) are shown in the Info/View Item dialog in the BE.
      const string infoInterfaceTemplate = "  'interface' => array( 'showRecordFieldList' => '{0}' )";

      // Were the T3CommonFields included in this model?
      string finalInterfaceFields = string.Empty;
      if( Configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3CommonFields ) ) {
        finalInterfaceFields += T3CommonFields.InterfaceInfoFields + ", ";
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

    /// <summary>
    /// Generates the 'types' array.
    /// </summary>
    /// <returns></returns>
    private string GenerateTypes() {
      // Template for the 'types' collection.
      const string typesTemplate = "  'types' => array( {0} )";
      // Template for a single type definition.
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
            // Not found in data model. Is it a palette maybe?
            Palette referencedPalette = Configuration.Palettes.SingleOrDefault( p => p.Name == field );

            if( null == referencedPalette ) {
              throw new GeneratorException(
                string.Format(
                  "The type interface field '{0}' neither exists in the data model '{1}' nor is it a palette.",
                  userInterfaceField,
                  Configuration.Model.Name ) );

            } else {
              // The field that was provided by the user is actually a palette.
              allTypes += "--palette--;;" + userInterfaceField + ",";
            }


          } else {
            allTypes += NameHelper.GetSqlColumnName( Subject, userInterfaceField ) + ",";
          }
        }

        // Add Access tab
        allTypes = allTypes.TrimEnd( new[] {',', ' '} );
        allTypes +=
          ",--div--;LLL:EXT:cms/locallang_ttc.xml:tabs.access,--palette--;LLL:EXT:cms/locallang_ttc.xml:palette.access;paletteAccess";

        allTypes = Regex.Replace( allTypes, ", *", ", " );

        string typeInterface = String.Format( typeInterfaceTemplate, allTypes );
        finalTypes.Append( string.Format( typeTemplate, typeIndex + 1, typeInterface ) );
      }

      return String.Format( typesTemplate, finalTypes );
    }

    /// <summary>
    /// Generates the 'palettes' array.
    /// </summary>
    /// <returns></returns>
    private string GeneratePalettes() {
      // Template for the 'palettes' collection.
      const string palettesTemplate = "  'palettes' => array( {0} )";
      // Template for a single palette definition.
      const string paletteTemplate = "'{0}' => array( {1} )";

      // Describes which fields (and in which order) are shown in the palette in the BE when editing a record.
      const string paletteInterfaceTemplate = "'showitem' => '{0}'";

      StringBuilder finalPalettes = new StringBuilder();

      if( Configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3CommonFields ) ) {
        finalPalettes.Append( T3CommonFields.Palette + "," );
      }

      foreach( Palette palette in Configuration.Palettes ) {
        // Validate and translate fields
        string[] fields = palette.Interface.Split( new[] {','} );
        String paletteInterfaceFields = string.Empty;
        foreach( string field in fields ) {
          KeyValuePair<string, string> referencedModelMember =
            Configuration.Model.Members.SingleOrDefault( m => m.Value == field );

          if( null == referencedModelMember.Key ) {
            throw new GeneratorException(
                string.Format(
                  "The palette field '{0}' does not exist in the data model '{1}'.",
                  field,
                  Configuration.Model.Name ) );
          }

          paletteInterfaceFields += NameHelper.GetSqlColumnName( Subject, field ) + ",";
        }

        paletteInterfaceFields = paletteInterfaceFields.TrimEnd( new[] {',', ' '} );
        paletteInterfaceFields = Regex.Replace( paletteInterfaceFields, ", *", ", " );

        string paletteInterface = string.Format( paletteInterfaceTemplate, paletteInterfaceFields );
        string paletteBody = String.Format( paletteTemplate, palette.Name, paletteInterface );
        finalPalettes.Append( paletteBody );
      }

      string resultingPalettes = string.Format( palettesTemplate, finalPalettes );

      return resultingPalettes;
    }

    /// <summary>
    /// Generates the 'columns' array.
    /// </summary>
    /// <returns></returns>
    private string GenerateColumns() {
      // Template for the 'columns' collection.
      const string columnsTemplate = "  'columns' => array( {0} )";
      // Template for a single columns definition.
      const string columnTemplate = "'{0}' => array( {1} )";
      
      StringBuilder finalColumns = new StringBuilder();

      // Prepare data model references
      foreach( Interface fieldInterface in Configuration.Interfaces ) {
        fieldInterface.ParentModel = Subject.Models.Single( m => m.Name == fieldInterface.ParentModelTarget );
      }

      foreach( Interface fieldInterface in Configuration.Interfaces ) {
        // Check if the target field exists
        if( !Configuration.Model.Members.Any( m=>m.Value == fieldInterface.Target) ) {
          throw new GeneratorException( string.Format( "Could not generate interface for nonexistent field '{0}'", fieldInterface.Target ) );
        }
        // Generate the column
        string interfaceDefinition = InterfaceGenerator.Generate( Subject, fieldInterface );
        finalColumns.Append( string.Format( columnTemplate, NameHelper.GetSqlColumnName( Subject, fieldInterface.Target ), interfaceDefinition ) + "," );
      }

      string resultingColumns = string.Format( columnsTemplate, finalColumns );

      return resultingColumns;
    }
  }
}
