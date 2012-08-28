using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StringLib;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class ConfigurationGenerator : AbstractGenerator, IGenerator {
    public ConfigurationGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public string TargetFile {
      get { return "ext_tables.php"; }
    }

    public void Generate() {
      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );
      
      WriteFile( "ext_tables.php", GeneratePhp(), true );
    }

    /// <summary>
    /// Generates the PHP statements to required instructions in the TCA.
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
                              "    'iconfile'                 => t3lib_extMgm::extRelPath( '{extensionKey}' ) . 'Resources/Public/Icons/{model}.gif',\n" +
                              "{thumbnail}" +
                              "{searchFields}" +
                              "  )\n" +
                              ");";

      const string t3CommonFieldsTemplate = "    'tstamp'                   => 'tstamp',\n" +
                                            "    'crdate'                   => 'crdate',\n" +
                                            "    'cruser_id'                => 'cruser_id',\n" +
                                            "    'delete'                   => 'deleted',\n" +
                                            "    'enablecolumns'            => array(\n" +
                                            "      'disabled'  => 'hidden',\n" +
                                            "      'starttime' => 'starttime',\n" +
                                            "      'endtime'   => 'endtime',\n" +
                                            "      'fe_group'  => 'fe_group'\n" +
                                            "    ),\n" +
                                            "    'editlock'                 => 'editlock',\n";

      const string t3TranslationFieldsTemplate = "    'origUid'                  => 't3_origuid',\n" +
                                                 "    'languageField'            => 'sys_language_uid',\n" +
                                                 "    'transOrigPointerField'    => 'l10n_parent',\n" +
                                                 "    'transOrigDiffSourceField' => 'l10n_diffsource',\n";

      const string t3VersioningFieldsTemplate = "    'versioningWS'             => 2,\n" +
                                                "    'versioning_followPages'   => TRUE,\n";

      foreach( Typo3ExtensionGenerator.Model.Configuration configuration in Subject.Configurations ) {
        // First, find the data model this configuration applies to
        DataModel targetModel = Subject.Models.SingleOrDefault( m => m.Name == configuration.Target );
        if( null == targetModel ) {
          throw new GeneratorException( string.Format( "Unable to find target data model '{0}'.", configuration.Target ) );
        }
        configuration.Model = targetModel;

        // Were the T3CommonFields included in this model?
        string finalCommonFields = string.Empty;
        if( configuration.Model.Members.Any( m => m.Key == Keywords.DataModelTemplate && m.Value == Keywords.DataModelTemplates.T3CommonFields ) ) {
          finalCommonFields = t3CommonFieldsTemplate;
        }

        // Were the T3TranslationFields included in this model?
        string finalTranslationFields = string.Empty;
        if( configuration.Model.Members.Any( m => m.Key == Keywords.DataModelTemplate && m.Value == Keywords.DataModelTemplates.T3TranslationFields ) ) {
          finalTranslationFields  = t3TranslationFieldsTemplate;
        }

        // Were the T3VersioningFields included in this model?
        string finalVersioningFields = string.Empty;
        if( configuration.Model.Members.Any( m => m.Key == Keywords.DataModelTemplate && m.Value == Keywords.DataModelTemplates.T3VersioningFields ) ) {
          finalVersioningFields = t3VersioningFieldsTemplate;
        }

        // Is an alternative label defined?
        string labelAlternative = string.Empty;
        if( !string.IsNullOrEmpty( configuration.LabelAlternative ) ) {
          labelAlternative = string.Format(
            "    'label_alt'                => '{0}',\n", configuration.LabelAlternative );
        }

        // Is a label hook requested?
        string labelFunction = string.Empty;
        if( !string.IsNullOrEmpty( configuration.LabelHook ) ) {
          labelFunction = String.Format(
            "    'label_userFunc'           => '{0}->getUserLabel{1}',\n",
            NameHelper.GetExtbaseHookClassName( Subject, "labels" ),
            NameHelper.UpperCamelCase( configuration.Model.Name ) );
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

        var dataObject = new {
                               extensionKey = Subject.Key,
                               model = NameHelper.GetAbsoluteModelName( Subject, configuration.Model ),
                               label = configuration.Label,
                               labelAlt = labelAlternative,
                               labelFunc = labelFunction,
                               commonFields = finalCommonFields,
                               translationFields = finalTranslationFields,
                               versioningFields = finalVersioningFields,
                               configFilename = NameHelper.GetExtbaseFileName( Subject,configuration.Model ),
                               thumbnail = thumbnailField,
                               searchFields = finalSearchFields
                             };
        string generatedConfiguration = template.HaackFormat( dataObject );

        result.Append( generatedConfiguration + "\n" );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }    
  }
}
