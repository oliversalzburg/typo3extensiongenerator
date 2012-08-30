using System;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Module {
  public class ModuleGenerator : AbstractGenerator, IGenerator {
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

      Console.WriteLine( "Registering modules..." );

      const string template = "if( TYPO3_MODE === 'BE' ) {{\n" +
                              "  Tx_Extbase_Utility_Extension::registerModule(\n" +
                              "    '{0}',\n" +
                              "    '{1}',\n" +
                              "    '{2}',\n" +
                              "    '',\n" +
                              "    array(\n" +
                              "      'TODO Import' => 'index, listCategories, listInstallNotes, enumDirectory, importEntityAsDownload',\n" +
                              "    ),\n" +
                              "    array(\n" +
                              "      'TODO access' => 'user,group',\n" +
                              "      'icon'   => 'EXT:{0}/ext_icon.gif',\n" +
                              "      'TODO labels' => 'LLL:EXT:{0}/Resources/Private/Language/locallang_downloadimportkey.xml',\n" +
                              "    )\n" +
                              "  );\n" +
                              "}}";

      for( int moduleIndex = 0; moduleIndex < Subject.Modules.Count; moduleIndex++ ) {
        Typo3ExtensionGenerator.Model.Module module = Subject.Modules[ moduleIndex ];

        Console.WriteLine( "Registering module '{0}'...", module.Name );

        string moduleKey = string.Format( "tx_{0}_m{1}", Subject.Key, moduleIndex + 1 );
        result.Append( String.Format( template, Subject.Key, module.MainModuleName, moduleKey ) + "\n" );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }    
  }
}
