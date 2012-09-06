using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Configuration;
using Typo3ExtensionGenerator.Generator.Model;
using Typo3ExtensionGenerator.Generator.Module;
using Typo3ExtensionGenerator.Generator.Plugin;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Resources;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  public class ExtensionGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// The directory where the generated extension should be placed.
    /// </summary>
    public string TargetDirectory { get; set; }

    public ExtensionGenerator( string outputDirectory, Extension subject ) : base( outputDirectory, subject ) {}

    /// <summary>
    /// Generates the described extension.
    /// </summary>
    /// <param name="extension">The extension that should be generated.</param>
    public void Generate() {
      Log.Info( "Clearing output directory..." );
      if( Directory.Exists( TargetDirectory ) ) {
        DirectoryHelper.DeleteDirectory( TargetDirectory, true );
      }
      Directory.CreateDirectory( TargetDirectory );

      Log.Info( "Generating extension..." );

      ExtensionCoreGenerator extensionCoreGenerator = new ExtensionCoreGenerator( TargetDirectory, Subject );
      extensionCoreGenerator.Generate();
      PluginGenerator pluginGenerator = new PluginGenerator( TargetDirectory, Subject );
      pluginGenerator.Generate();
      ModuleGenerator moduleGenerator = new ModuleGenerator( TargetDirectory, Subject );
      moduleGenerator.Generate();
      ModelGenerator modelGenerator = new ModelGenerator( TargetDirectory, Subject );
      modelGenerator.Generate();
      ConfigurationGenerator configurationGenerator = new ConfigurationGenerator( TargetDirectory, Subject );
      configurationGenerator.Generate();
      RequirementGenerator requirementGenerator = new RequirementGenerator( TargetDirectory, Subject );
      requirementGenerator.Generate();

      // Create extension icon
      ResourceHelper.FlushIcon( "box.gif", TargetDirectory, "ext_icon.gif" );

      
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

      WrapAllVirtual( @"Resources/Private/Language/.*.xml", languageFilePrefix, languageFileSuffix );

      const string protectedPhpPrefix = "<?php\n" +
                                        "if( !defined( 'TYPO3_MODE' ) ) {\n" +
                                        "	die( 'Access denied.' );\n" +
                                        "}\n";
      const string phpSuffix = "?>";

      const string phpClassPrefix = "<?php\n" +
                                    "class {0} {{\n";
      const string phpClassSuffix = "}\n" +
                                    "?>";

      // Label hooks
      WriteLabelHooks( phpClassSuffix );

      // Write TypoScript include statements
      var dataObject =
        new {extensionName = "tx_" + NameHelper.UpperCamelCase( Subject.Key ).ToLower(), extensionKey = Subject.Key, extensionTitle = Subject.Title};
      const string typoScriptRegisterTemplate =
        "t3lib_extMgm::addStaticFile('{extensionKey}', 'Configuration/TypoScript', '{extensionTitle}');";

      WriteVirtual( "ext_tables.php", typoScriptRegisterTemplate.FormatSmart( dataObject ) );

      // Wrap virtual files as needed
      WrapVirtual( "ext_localconf.php", protectedPhpPrefix, phpSuffix );
      WrapVirtual( "ext_tables.php", protectedPhpPrefix, phpSuffix );
      // Flush virtual file system to disk
      AbstractGenerator.FlushVirtual( TargetDirectory );
    }

    /// <summary>
    /// Generates the class that will provide the label hooks for the extension
    /// </summary>
    /// <param name="phpClassSuffix"></param>
    private void WriteLabelHooks( string phpClassSuffix ) {
      if( !string.IsNullOrEmpty( Subject.LabelHookImplementation ) && !File.Exists( Subject.LabelHookImplementation ) ) {
        throw new GeneratorException(
          string.Format( "Label hook implementation '{0}' does not exist.", Subject.LabelHookImplementation ),
          Subject.SourceLine );
      }
      const string externalImplementationTemplate = "private $implementation;\n" +
                                                    "private function getImplementation() {{\n" +
                                                    "  if( null == $this->implementation ) {{\n" +
                                                    "    $this->implementation = new {implClassname}($this);\n" +
                                                    "  }}\n" +
                                                    "  return $this->implementation;\n" +
                                                    "}}\n";

      const string labelHookTemplate = "<?php\n" +
                                       "require_once('{implFilename}');\n" +
                                       "class {className} {{\n" +
                                       "{externalImplementation}";

      string implementationClassname = NameHelper.GetLabelHooksImplementationClassName( Subject );
      string implementationFilename = NameHelper.GetLabelHooksImplementationFileName( Subject );

      string labelHooks = labelHookTemplate.FormatSmart(
        new {
              className = NameHelper.GetExtbaseHookClassName( Subject, "labels" ),
              implFilename = implementationFilename,
              externalImplementation =
          ( !string.IsNullOrEmpty( Subject.LabelHookImplementation ) )
            ? externalImplementationTemplate.FormatSmart(
              new {implClassname = implementationClassname,} )
            : string.Empty
            } );
      WrapVirtual( "Classes/Hooks/Labels.php", labelHooks, phpClassSuffix );

      Log.InfoFormat( "Merging label hook implementation '{0}'...", Subject.LabelHookImplementation );
      string pluginImplementation = File.ReadAllText( Subject.LabelHookImplementation );
      DateTime lastWriteTimeUtc = new FileInfo( Subject.LabelHookImplementation ).LastWriteTimeUtc;
      if(
        !Regex.IsMatch(
          pluginImplementation, String.Format( "class {0} ?({{|extends|implements)", implementationClassname ) ) ) {
        Log.WarnFormat( "The class name of your label hooks implementation MUST be '{0}'!", implementationClassname );
      }
      WriteFile( "Classes/Hooks/" + implementationFilename, pluginImplementation, lastWriteTimeUtc );
    }
  }
}
