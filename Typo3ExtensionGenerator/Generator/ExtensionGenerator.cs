using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator.Configuration;
using Typo3ExtensionGenerator.Generator.Model;
using Typo3ExtensionGenerator.Generator.Module;
using Typo3ExtensionGenerator.Generator.Plugin;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator {
  class ExtensionGenerator {
    /// <summary>
    /// The directory where the generated extension should be placed.
    /// </summary>
    public string TargetDirectory { get; set; }

    /// <summary>
    /// Generates the described extension.
    /// </summary>
    /// <param name="extension">The extension that should be generated.</param>
    public void Generate( Extension extension ) {
      if( !Directory.Exists( TargetDirectory ) ) {
        throw new GeneratorException( "The provided target directory does not exist." );
      }

      Console.WriteLine( "Generating extension..." );

      ExtEmconfGenerator extEmconfGenerator = new ExtEmconfGenerator( TargetDirectory, extension );
      extEmconfGenerator.Generate();
      PluginGenerator pluginGenerator = new PluginGenerator( TargetDirectory, extension );
      pluginGenerator.Generate();
      ModuleGenerator moduleGenerator = new ModuleGenerator( TargetDirectory, extension );
      moduleGenerator.Generate();
      ModelGenerator modelGenerator = new ModelGenerator( TargetDirectory, extension );
      modelGenerator.Generate();
      ConfigurationGenerator configurationGenerator = new ConfigurationGenerator( TargetDirectory, extension );
      configurationGenerator.Generate();

      // Wrap virtual files as needed
      const string extTablesPrefix = "<?php\n" +
                                     "if( !defined( 'TYPO3_MODE' ) ) {\n" +
                                     "	die( 'Access denied.' );\n" +
                                     "}\n";
      const string extTablesSuffix = "?>";

      AbstractGenerator.WrapVirtual( TargetDirectory, "ext_tables.php", extTablesPrefix, extTablesSuffix );

      // Flush virtual file system to disk
      AbstractGenerator.FlushVirtual();
    }
  }
}
