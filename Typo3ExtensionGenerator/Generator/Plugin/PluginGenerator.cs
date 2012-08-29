using System;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Plugin {
  public class PluginGenerator : AbstractGenerator, IGenerator {
    public PluginGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {

      Console.WriteLine( string.Format( "Generating Plugins..." ) );

      GeneratePhp();
    }

    /// <summary>
    /// Generates the PHP statements to register the plugins.
    /// </summary>
    /// <returns></returns>
    private void GeneratePhp( ) {
      StringBuilder extTables = new StringBuilder();
      StringBuilder extLocalconf = new StringBuilder();

      const string registerPlugin = "Tx_Extbase_Utility_Extension::registerPlugin(\n" +
                                    "  '{0}',\n" +
                                    "  '{1}',\n" +
                                    "  'LLL:EXT:{0}/Resources/Private/Language/locallang_be.xml:{2}'\n" +
                                    ");";

      const string configurePlugin = "Tx_Extbase_Utility_Extension::configurePlugin(" +
                                     "  '{0}'," +
                                     "  '{1}'," +
                                     "  array(" +
                                     "    'ControllerGoesHere' => 'actionGoesHere'" +
                                     "  )," +
                                     "  array(" +
                                     "    'ControllerGoesHere' => 'uncachedActionGoesHere'" +
                                     "  )" +
                                     ");";

      for( int pluginIndex = 0; pluginIndex < Subject.Plugins.Count; pluginIndex++ ) {
        Typo3ExtensionGenerator.Model.Plugin plugin = Subject.Plugins[ pluginIndex ];
        
        string languageConstant = String.Format( "{0}_title", plugin.Name.ToLower() );
        extTables.Append( String.Format( registerPlugin, Subject.Key, NameHelper.UpperCamelCase( plugin.Name ), languageConstant ) + "\n" );

        // Also write label string to language file
        WriteFile( "Resources/Private/Language/locallang_be.xml", string.Format( "<label index=\"{0}\">{1}</label>", languageConstant, plugin.Title ), true );

        string extensionName = NameHelper.UpperCamelCase( Subject.Key );
        string pluginSignature = String.Format( "{0}_{1}", extensionName.ToLower(), plugin.Name );

        extTables.Append(
          string.Format(
            "$TCA['tt_content']['types']['list']['subtypes_addlist']['{0}'] = 'pi_flexform';\n", pluginSignature ) );
        extTables.Append(
          string.Format(
            "t3lib_extMgm::addPiFlexFormValue('{0}', 'FILE:EXT:{1}/Configuration/FlexForms/flexform_{2}.xml');\n",
            pluginSignature, Subject.Key, plugin.Name.ToLower() ) );

        extLocalconf.Append( string.Format( configurePlugin, Subject.Key, NameHelper.UpperCamelCase( plugin.Name ) ) + "\n" );
      }

      string extTablesPhp = extTables.ToString().Substring( 0, extTables.Length - 1 );
      string extLocalconfPhp = extLocalconf.ToString().Substring( 0, extLocalconf.Length - 1 );
      WriteFile( "ext_tables.php", extTablesPhp, true );
      WriteFile( "ext_localconf.php", extLocalconfPhp, true );
    }    
  }
}
