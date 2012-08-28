using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class ConfigurationFileGenerator : AbstractGenerator, IGenerator {
    public ConfigurationFileGenerator( string outputDirectory, Extension extension, Typo3ExtensionGenerator.Model.Configuration configuration ) : base( outputDirectory, extension ) {
      Configuration = configuration;
    }

    public string TargetFile {
      get { return "Configuration/TCA/" + NameHelper.GetExtbaseFileName( Subject, Configuration.Model ); }
    }

    public Typo3ExtensionGenerator.Model.Configuration Configuration { get; private set; }

    public void Generate() {
      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );
      
      WritePhpFile( TargetFile, GeneratePhp() );
    }

    /// <summary>
    /// Generates the PHP statements for the full TCA configuration of a model/configuration.
    /// </summary>
    /// <returns></returns>
    private string GeneratePhp() {
      const string template = "if( !defined( 'TYPO3_MODE' ) ) {\n" +
                              "  die( 'Access denied.' );\n" +
                              "}";
      return template;
    }
  }
}
