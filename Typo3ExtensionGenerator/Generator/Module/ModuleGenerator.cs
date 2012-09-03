using System;
using System.Text;
using SmartFormat;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator.Module {
  public class ModuleGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ModuleGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      WriteFile( "ext_tables.php", GeneratePhp(), true );
    }

    /// <summary>
    /// Generates the PHP statements to register the modules.
    /// </summary>
    /// <returns></returns>
    private string GeneratePhp( ) {
      StringBuilder result = new StringBuilder();

      Log.Info( "Registering modules..." );

      const string template = "if( TYPO3_MODE === 'BE' ) {{\n" +
                              "  Tx_Extbase_Utility_Extension::registerModule(\n" +
                              "    '{extensionName}',\n" +
                              "    '{mainModuleName}',\n" +
                              "    '{subModuleName}',\n" +
                              "    '',\n" +
                              "    array(\n" +
                              "      'TODO Import' => 'index, listCategories, listInstallNotes, enumDirectory, importEntityAsDownload',\n" +
                              "    ),\n" +
                              "    array(\n" +
                              "      'TODO access' => 'user,group',\n" +
                              "      'icon'   => 'EXT:{extensionName}/ext_icon.gif',\n" +
                              "      'labels' => 'LLL:EXT:{extensionName}/Resources/Private/Language/locallang_{langFileKey}.xml',\n" +
                              "    )\n" +
                              "  );\n" +
                              "}}";

      for( int moduleIndex = 0; moduleIndex < Subject.Modules.Count; moduleIndex++ ) {
        Typo3ExtensionGenerator.Model.Module module = Subject.Modules[ moduleIndex ];

        Log.InfoFormat( "Registering module '{0}'...", module.Name );

        string subKey = string.Format( "m{0}", moduleIndex + 1 );
        string moduleKey = string.Format( "tx_{0}_{1}", Subject.Key, subKey );
        result.Append(
          template.FormatSmart(
            new {
                  extensionName = Subject.Key,
                  mainModuleName = module.MainModuleName,
                  subModuleName = moduleKey,
                  langFileKey = subKey.ToLower()
                } ) + "\n" );

      // <label index="mlang_tabs_tab">Download Importer</label>
      // <label index="mlang_labels_tabdescr">Import download records from files on the file system.</label>
      // <label index="mlang_labels_tablabel">Create download records from files on the file system.</label>

        WriteFile( string.Format( "Resources/Private/Language/locallang_{0}.xml", subKey.ToLower() ), string.Format( "<label index=\"{0}\">{1}</label>", "mlang_tabs_tab", module.Title ), true );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }    
  }
}
