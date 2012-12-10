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
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resources;
using log4net;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  /// <summary>
  /// Generates the "configuration" for an extension.
  /// The configuration describes how the data model are to be translated between the database and TYPO3.
  /// </summary>
  public class ConfigurationGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Constructs a ConfigurationGenerator.
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="extension">The extension.</param>
    public ConfigurationGenerator( Context context, Extension extension ) : base( context, extension ) {}

    /// <summary>
    /// Generates the configuration files for the extension.
    /// </summary>
    public void Generate() {
      WriteVirtual( "ext_tables.php", GeneratePhp() );
      WriteConfigurationFiles();
    }

    /// <summary>
    /// Writes the configurations files for the extension.
    /// </summary>
    private void WriteConfigurationFiles() {
      if( null == Subject.Configurations ) return;

      foreach( Typo3ExtensionGenerator.Model.Configuration.Configuration configuration in Subject.Configurations ) {
        // Export dynamic config file
        ConfigurationFileGenerator configurationFileGenerator = new ConfigurationFileGenerator( GeneratorContext, Subject, configuration );
        configurationFileGenerator.Generate();
      }
    }

    /// <summary>
    /// Generates the PHP statements to create required instructions in the TCA.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="GeneratorException">Unable to find target data model</exception>
    private string GeneratePhp( ) {
      if( null == Subject.Configurations ) return string.Empty;

      StringBuilder result = new StringBuilder();

      const string template = "$TCA['{model}'] = array(\n" +
                              "  'ctrl' => array(\n" +
                              "    'title'                    => 'LLL:EXT:{extensionKey}/Resources/Private/Language/locallang_db.xml:{model}',\n" +
                              "    'label'                    => '{label}',\n" +
                              "{tableHidden}" +
                              "{labelAlt}" +
                              "{labelFunc}" +
                              "    'dividers2tabs'            => TRUE,\n" +

                              "{commonFields}" +

                              "{versioningFields}" +
                              
                              "{translationFields}" +

                              "{sortableFields}" +
                              
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
          throw new GeneratorException( string.Format( "Unable to find target data model '{0}'.", configuration.Target ), configuration.SourceFragment.SourceDocument );
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

        // Were the T3VersioningFields included in this model?
        string finalSortableFields = string.Empty;
        if( configuration.Model.UsesTemplate( Keywords.DataModelTemplates.T3Sortable ) ) {
          finalSortableFields = T3Sortable.TableControlFields + ",\n";
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
          bool isExternallyImplemented = true;
          if( string.IsNullOrEmpty( Subject.LabelHookImplementation ) ) {
            Log.WarnFormat( "Label hook requested for '{0}', but the label hook implementation is missing.", configuration );
            isExternallyImplemented = false;
          }

          labelFunction = String.Format(
            "    'label_userFunc'           => '{0}->getUserLabel{1}',\n",
            NameHelper.GetExtbaseHookClassName( Subject, "labels" ),
            NameHelper.UpperCamelCase( configuration.Model.Name ) );

          // Write hook
          const string filename = "Classes/Hooks/Labels.php";
          string internallyImplemented = string.Format( "public static function getUserLabel{0}( array &$params, &$pObj ) {{ $params[ 'title' ] = 'This title is generated in {1}'; }}", NameHelper.UpperCamelCase( configuration.Model.Name ), filename );
          string externallyImplemented = string.Format( "public static function getUserLabel{0}( array &$params, &$pObj ) {{ return {1}::getUserLabel{0}( $params, $pObj ); }}", NameHelper.UpperCamelCase( configuration.Model.Name ), NameHelper.GetLabelHooksImplementationClassName( Subject ) );
          WriteVirtual( filename, ( isExternallyImplemented ) ? externallyImplemented : internallyImplemented );

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

        string hideTable = configuration.Hidden ? "'hideTable'=>1,\n" : string.Empty;

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
                               sortableFields = finalSortableFields,
                               configFilename = NameHelper.GetExtbaseDomainModelFileName( Subject, configuration.Model ),
                               thumbnail = thumbnailField,
                               searchFields = finalSearchFields,
                               tableHidden = hideTable
                             };
        string generatedConfiguration = template.FormatSmart( dataObject );

        result.Append( generatedConfiguration + "\n" );

        // Flush a placdeholder icon
        ResourceHelper.FlushIcon( "document.png", this, string.Format( "Resources/Public/Icons/{0}.png", absoluteModelName ) );
      }

      if( addRequireLabelHooks ) {
        string requireLabels = string.Format( "t3lib_div::requireOnce( t3lib_extMgm::extPath( '{0}' ) . 'Classes/Hooks/Labels.php' );", Subject.Key );
        result.Append( requireLabels + "\n" );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }

    /// <summary>
    /// Write all defined language fields to the locallang_db.xml
    /// </summary>
    /// <param name="configuration"></param>
    private void FlushLanguageFields( Typo3ExtensionGenerator.Model.Configuration.Configuration configuration ) {
      string languageConstant = NameHelper.GetAbsoluteModelName( Subject, configuration.Model );

      WriteVirtual( "Resources/Private/Language/locallang_db.xml", string.Format( "<label index=\"{0}\">{1}</label>", languageConstant, configuration.Title ) );
    }
  }
}
