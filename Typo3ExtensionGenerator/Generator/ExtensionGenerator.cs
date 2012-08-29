using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator.Configuration;
using Typo3ExtensionGenerator.Generator.Model;
using Typo3ExtensionGenerator.Generator.Module;
using Typo3ExtensionGenerator.Generator.Plugin;
using Typo3ExtensionGenerator.Helper;
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

      const string languageFilePrefix = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>\n" +
                                        "<T3locallang>\n" +
                                        "	<meta type=\"array\">\n" +
                                        "		<type>database</type>\n" +
                                        "		<description>Language labels for database tables/fields belonging to extension 'downloads'</description>\n" +
                                        "	</meta>\n" +
                                        "	<data type=\"array\">\n" +
                                        "		<languageKey index=\"default\" type=\"array\">";

      const string languageFileSuffix = "		</languageKey>\n" +
                                        "	</data>\n" +
                                        "</T3locallang>";

      const string phpClassPrefix = "<?php\n" +
                                    "class {0} {{\n";
      const string phpClassSuffix = "}\n" +
                                    "?>";

      AbstractGenerator.WrapVirtual( "ext_tables.php", extTablesPrefix, extTablesSuffix );
      AbstractGenerator.WrapVirtual(
        "Classes/Hooks/Labels.php",
        string.Format( phpClassPrefix, NameHelper.GetExtbaseHookClassName( extension, "Labels" ) ), phpClassSuffix );
      AbstractGenerator.WrapAllVirtual( @"Resources/Private/Language/.*.xml", languageFilePrefix, languageFileSuffix );

      // Flush virtual file system to disk
      AbstractGenerator.FlushVirtual( TargetDirectory );
    }
  }
}
