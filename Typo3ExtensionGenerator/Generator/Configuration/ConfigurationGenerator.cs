using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

      const string template = "$TCA['{0}'] = array(\n" +
                              "  'ctrl' => array(\n" +
                              "    'title'                    => 'LLL:EXT:downloads/Resources/Private/Language/locallang_db.xml:tx_downloads_domain_model_download',\n" +
                              "    'label'                    => '{1}',\n" +
                              "    'label_alt'                => 'file_name',\n" +
                              "    'label_userFunc'           => 'Tx_Downloads_Hooks_Labels->getUserLabelDownload',\n" +
                              "    'tstamp'                   => 'tstamp',\n" +
                              "    'crdate'                   => 'crdate',\n" +
                              "    'cruser_id'                => 'cruser_id',\n" +
                              "    'dividers2tabs'            => TRUE,\n" +
                              "    'versioningWS'             => 2,\n" +
                              "    'versioning_followPages'   => TRUE,\n" +
                              "    'origUid'                  => 't3_origuid',\n" +
                              "    'editlock'                 => 'editlock',\n" +
                              "    'languageField'            => 'sys_language_uid',\n" +
                              "    'transOrigPointerField'    => 'l10n_parent',\n" +
                              "    'transOrigDiffSourceField' => 'l10n_diffsource',\n" +
                              "    'delete'                   => 'deleted',\n" +
                              "    'enablecolumns'            => array(\n" +
                              "      'disabled'  => 'hidden',\n" +
                              "      'starttime' => 'starttime',\n" +
                              "      'endtime'   => 'endtime',\n" +
                              "      'fe_group'  => 'fe_group'\n" +
                              "    ),\n" +
                              "    'dynamicConfigFile'        => t3lib_extMgm::extPath( $_EXTKEY ) . 'Configuration/TCA/Download.php',\n" +
                              "    'iconfile'                 => t3lib_extMgm::extRelPath( $_EXTKEY ) . 'Resources/Public/Icons/tx_downloads_domain_model_download.gif',\n" +
                              "    'thumbnail'                => 'file_name',\n" +
                              "    'searchFields'             => 'title,file_name,qualifier'\n" +
                              "  ),\n" +
                              ");";

      foreach( Typo3ExtensionGenerator.Model.Configuration configuration in Subject.Configurations ) {
        // First, find the data model this configuration applies to
        DataModel targetModel = Subject.Models.SingleOrDefault( m => m.Name == configuration.Target );
        if( null == targetModel ) {
          throw new GeneratorException( string.Format( "Unable to find target data model '{0}'.", configuration.Target ) );
        }
        configuration.Model = targetModel;

        result.Append( String.Format( template, NameHelper.GetAbsoluteModelName( Subject, configuration.Model ) ) );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }    
  }
}
