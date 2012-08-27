using System;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Plugin {
  public class PluginGenerator : AbstractGenerator, IGenerator {
    public PluginGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public string TargetFile {
      get { return "ext_tables.php"; }
    }

    public void Generate() {

      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );
      
      WriteFile( "ext_tables.php", GeneratePhp(), true );
    }

    /// <summary>
    /// Generates the PHP statements to register the plugins.
    /// </summary>
    /// <returns></returns>
    private string GeneratePhp( ) {
      StringBuilder result = new StringBuilder();

      const string template = "Tx_Extbase_Utility_Extension::registerPlugin(\n" +
                              "  '{0}',\n" + 
                              "  '{1}',\n" +
                              "  'LLL:EXT:{0}/Resources/Private/Language/locallang_be.xml:{2}'\n" +
                              ");";

      foreach( Typo3ExtensionGenerator.Model.Plugin plugin in Subject.Plugins ) {
        result.Append( String.Format( template, Subject.Key, plugin.Name, plugin.Name.ToLower() + "_title" ) + "\n" );
      }

      return result.ToString().Substring( 0, result.Length - 1 );
    }    
  }
}
