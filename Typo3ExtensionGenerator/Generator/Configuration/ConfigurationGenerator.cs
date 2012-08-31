using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartFormat;
using SmartFormat.Extensions;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resources;
using log4net;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class ConfigurationGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ConfigurationGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      WriteFile( "ext_tables.php", GeneratePhp(), true );
      WriteConfigurationFiles();
    }

    private void WriteConfigurationFiles() {
      if( null == Subject.Configurations ) return;

      foreach( Typo3ExtensionGenerator.Model.Configuration.Configuration configuration in Subject.Configurations ) {
        // Export dynamic config file
        ConfigurationFileGenerator configurationFileGenerator = new ConfigurationFileGenerator(
          OutputDirectory, Subject, configuration );
        configurationFileGenerator.Generate();
      }
    }

    /// <summary>
    /// Generates the PHP statements to create required instructions in the TCA.
    /// </summary>
    /// <returns></returns>
    private string GeneratePhp( ) {
      if( null == Subject.Configurations ) return string.Empty;

      StringBuilder result = new StringBuilder();

      const string template = "$TCA['{model}'] = array(\n" +
                              "  'ctrl' => array(\n" +
                              "    'title'                    => 'LLL:EXT:{extensionKey}/Resources/Private/Language/locallang_db.xml:{model}',\n" +
                              "    'label'                    => '{label}',\n" +
                              "{labelAlt}" +
                              "{labelFunc}" +
                              "    'dividers2tabs'            => TRUE,\n" +

                              "{commonFields}" +

                              "{versioningFields}" +
                              
                              "{translationFields}" +
                              
                              "    'dynamicConfigFile'        => t3lib_extMgm::extPath( '{extensionKey}' ) . 'Configuration/TCA/{configFilename}',\n" +
                              "    'iconfile'                 => t3lib_extMgm::extRelPath( '{extensionKey}' ) . 'Resources/Public/Icons/{model}.png',\n" +
                              "{thumbnail}" +
                              "{searchFields}" +
                              "  )\n" +
                              ");";

      // Should a require_once line for the label hooks be placed into ext_tables.php?
      bool addRequireLabelHooks = false;

      foreach( Typo3ExtensionGenerator.Model.Configuration.Configuration configuration in Subject.Configurations ) {
        // First, find the data model this configuration applies to
        DataModel targetModel = Subject.Models.SingleOrDefault( m => m.Name == configuration.Target );
        if( null == targetModel ) {
          throw new GeneratorException( string.Format( "Unable to find target data model '{0}'.", configuration.Target ), configuration.SourceLine );
        }
        configuration.Model = targetModel;

        Log.InfoFormat( "Generating TCA for model '{0}'...", targetModel.Name );

        // Now flush out any language fields
        FlushLanguageFields( configuration );

        // Were the T3CommonFields included in this model?
        string finalCommonFields = string.Empty;
        if( configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3CommonFields ) ) {
          finalCommonFields = T3CommonFields.TableControlFields + ",\n";
        }

        // Were the T3TranslationFields included in this model?
        string finalTranslationFields = string.Empty;
        if( configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3TranslationFields ) ) {
          finalTranslationFields  = T3TranslationFields.TableControlFields + ",\n";
        }

        // Were the T3VersioningFields included in this model?
        string finalVersioningFields = string.Empty;
        if( configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3VersioningFields ) ) {
          finalVersioningFields = T3VersioningFields.TableControlFields + ",\n";
        }

        // Is an alternative label defined?
        string labelAlternative = string.Empty;
        if( !string.IsNullOrEmpty( configuration.LabelAlternative ) ) {
          labelAlternative = string.Format(
            "    'label_alt'                => '{0}',\n", configuration.LabelAlternative );
        }

        // Is a label hook requested?
        string labelFunction = string.Empty;
        if( configuration.LabelHook ) {
          labelFunction = String.Format(
            "    'label_userFunc'           => '{0}->getUserLabel{1}',\n",
            NameHelper.GetExtbaseHookClassName( Subject, "labels" ),
            NameHelper.UpperCamelCase( configuration.Model.Name ) );

          // Write hook
          string filename = "Classes/Hooks/Labels.php";
          WriteFile(
            filename,
            string.Format(
              "public static function getUserLabel{0}( array &$params, &$pObj ) {{ $params[ 'title' ] = 'This title is generated in {1}'; }}",
              NameHelper.UpperCamelCase( configuration.Model.Name ), filename ), true );

          addRequireLabelHooks = true;
        }

        // Is a thumbnail defined?
        string thumbnailField = string.Empty;
        if( !string.IsNullOrEmpty( configuration.Thumbnail ) ) {
          thumbnailField = string.Format(
            "    'thumbnail'                => '{0}',\n", configuration.Thumbnail );
        }

        // Are search fields defined?
        string finalSearchFields = string.Empty;
        if( !string.IsNullOrEmpty( configuration.SearchFields ) ) {
          finalSearchFields = string.Format(
            "    'searchFields'             => '{0}'\n", configuration.SearchFields );
        }

        string absoluteModelName = NameHelper.GetAbsoluteModelName( Subject, configuration.Model );
        var dataObject = new {
                               extensionKey = Subject.Key,
                               model = absoluteModelName,
                               label = configuration.Label,
                               labelAlt = labelAlternative,
                               labelFunc = labelFunction,
                               commonFields = finalCommonFields,
                               translationFields = finalTranslationFields,
                               versioningFields = finalVersioningFields,
                               configFilename = NameHelper.GetExtbaseDomainModelFileName( Subject, configuration.Model ),
                               thumbnail = thumbnailField,
                               searchFields = finalSearchFields
                             };
        string generatedConfiguration = template.FormatSmart( dataObject );

        result.Append( generatedConfiguration + "\n" );

        // Flush a placdeholder icon
        ResourceHelper.FlushIcon( "document.png", OutputDirectory, string.Format( "Resources/Public/Icons/{0}.png", absoluteModelName ) );
      }

      if( addRequireLabelHooks ) {
        string requireLabels = string.Format( "t3lib_div::requireOnce( t3lib_extMgm::extPath( '{0}' ) . 'Classes/Hooks/Labels.php' );", Subject.Key );
        result.Append( requireLabels + "\n" );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }

    private void FlushLanguageFields( Typo3ExtensionGenerator.Model.Configuration.Configuration configuration ) {
      string languageConstant = NameHelper.GetAbsoluteModelName( Subject, configuration.Model );

      WriteFile( "Resources/Private/Language/locallang_db.xml", string.Format( "<label index=\"{0}\">{1}</label>", languageConstant, configuration.Title ), true );
    }
  }
}
