using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Compatibility;
using Typo3ExtensionGenerator.Generator.Configuration;
using Typo3ExtensionGenerator.Generator.Model;
using Typo3ExtensionGenerator.Generator.Module;
using Typo3ExtensionGenerator.Generator.Plugin;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resources;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates a TYPO3 extension.
  /// </summary>
  public class ExtensionGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="subject">The extension object that should be generated.</param>
    public ExtensionGenerator( Context context, Extension subject ) : base( context, subject ) {}

    /// <summary>
    /// Generates the described extension.
    /// </summary>
    /// <exception cref="ParserException">Illegal extension key. Can't contain whitespace.</exception>
    public void Generate() {
      if( Subject.Key.IndexOfAny( new[]{'\r','\n','\t',' '} ) > -1 ) {
        throw new ParserException( "Illegal extension key. Can't contain whitespace.", Subject.SourceFragment.SourceDocument );
      }

      Log.Info( "Clearing output directory..." );
      try {
        if( Directory.Exists( GeneratorContext.OutputDirectory ) ) {
          DirectoryHelper.DeleteDirectory( GeneratorContext.OutputDirectory, true );
        }
      } catch( IOException ex ) {
        Log.Error( string.Format( "Unable to clear target directory '{0}'", GeneratorContext.OutputDirectory ), ex );
      }
      Directory.CreateDirectory( GeneratorContext.OutputDirectory );

      Log.Info( "Generating extension..." );

      ExtensionCoreGenerator extensionCoreGenerator = new ExtensionCoreGenerator( GeneratorContext, Subject );
      extensionCoreGenerator.Generate();
      PluginGenerator pluginGenerator = new PluginGenerator( GeneratorContext, Subject );
      pluginGenerator.Generate();
      ModuleGenerator moduleGenerator = new ModuleGenerator( GeneratorContext, Subject );
      moduleGenerator.Generate();
      ModelGenerator modelGenerator = new ModelGenerator( GeneratorContext, Subject );
      modelGenerator.Generate();
      ConfigurationGenerator configurationGenerator = new ConfigurationGenerator( GeneratorContext, Subject );
      configurationGenerator.Generate();
      ServiceGenerator serviceGenerator = new ServiceGenerator( GeneratorContext, Subject );
      serviceGenerator.Generate();
      TaskGenerator taskGenerator = new TaskGenerator( GeneratorContext, Subject );
      taskGenerator.Generate();
      
      // We generate the requirements last so that they can overwrite previous work
      RequirementGenerator requirementGenerator = new RequirementGenerator( GeneratorContext, Subject );
      requirementGenerator.Generate();
      

      // Create extension icon
      ResourceHelper.FlushIcon( "box.gif", this, "ext_icon.gif" );

      
      const string languageFilePrefix = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>\n" +
                                        "<T3locallang>\n" +
                                        "	<meta type=\"array\">\n" +
                                        "		<type>database</type>\n" +
                                        "		<description>TYPO3 Language file</description>\n" +
                                        "	</meta>\n" +
                                        "	<data type=\"array\">\n" +
                                        "		<languageKey index=\"default\" type=\"array\">";

      const string languageFileSuffix = "		</languageKey>\n" +
                                        "	</data>\n" +
                                        "</T3locallang>";

      WrapAllVirtual( @"Resources/Private/Language/.*.xml", languageFilePrefix, languageFileSuffix );
      
      #region Prefixes
      const string protectedPhpPrefix = "<?php\n" +
                                        "if( !defined( 'TYPO3_MODE' ) ) {\n" +
                                        "	die( 'Access denied.' );\n" +
                                        "}\n";

      const string phpClassPrefix = "<?php\n" +
                                    "class {0} {{\n";
      #endregion


      const string phpSuffix = "?>";

      const string phpClassSuffix = "}\n" +
                                    "?>";

      // Label hooks
      WriteLabelHooks( phpClassSuffix );

      // Write TypoScript
      GenerateTypoScript();

      // Wrap virtual files as needed
      WrapVirtual( "ext_autoload.php", "<?php\n$extensionPath=t3lib_extMgm::extPath('{_extKey}');return array(".FormatSmart( new {_extKey = Subject.Key} ), ");" + phpSuffix );
      WrapVirtual( "ext_localconf.php", protectedPhpPrefix, phpSuffix );
      WrapVirtual( "ext_tables.php",    protectedPhpPrefix, phpSuffix );
      // Flush virtual file system to disk
      FlushVirtual( GeneratorContext.OutputDirectory );
    }

    /// <summary>
    /// Writes the default TypoScript constants and settings files for the extension.
    /// </summary>
    private void GenerateTypoScript() {
      const string constantsTemplate = "plugin.{extensionName} {{\n" +
                                       "	view {{\n" +
                                       "		# cat=plugin.{extensionName}/file; type=string; label=Path to template root (FE)\n" +
                                       "		templateRootPath = EXT:{extensionKey}/Resources/Private/Templates/\n" +
                                       "		# cat=plugin.{extensionName}/file; type=string; label=Path to template partials (FE)\n" +
                                       "		partialRootPath = EXT:{extensionKey}/Resources/Private/Partials/\n" +
                                       "		# cat=plugin.{extensionName}/file; type=string; label=Path to template layouts (FE)\n" +
                                       "		layoutRootPath = EXT:{extensionKey}/Resources/Private/Layouts/\n" +
                                       "	}}\n" +
                                       "	persistence {{\n" +
                                       "		# cat=plugin.{extensionName}//a; type=int+; label=Default storage PID\n" +
                                       "		storagePid = \n" +
                                       "	}}\n" +
                                       "	settings {{\n" +
                                       "	 # cat=plugin.{extensionName}/file; type=string; label=Path to file type icons\n" +
                                       "    iconPath = EXT:{extensionKey}/Resources/Public/Icons/TypeIcons/\n" +
                                       "  }}\n" +
                                       "}}";

      const string setupTemplate = "plugin.{extensionName} {{\n" +
                                   "	view {{\n" +
                                   "		templateRootPath = {{$plugin.{extensionName}.view.templateRootPath}}\n" +
                                   "		partialRootPath  = {{$plugin.{extensionName}.view.partialRootPath}}\n" +
                                   "		layoutRootPath   = {{$plugin.{extensionName}.view.layoutRootPath}}\n" +
                                   "	}}\n" +
                                   "	persistence {{\n" +
                                   "		storagePid = {{$plugin.{extensionName}.persistence.storagePid}}\n" +
                                   "	}}\n" +
                                   "	settings {{\n" +
                                   "	  iconPath = {{$plugin.{extensionName}.settings.iconPath}}\n" +
                                   "	  example {{\n" +
                                   "	    // Place your own TS here\n" +
                                   "	  }}\n" +
                                   "	}}\n" +
                                   "}}";

      var dataObject =
        new {
              extensionName = "tx_" + NameHelper.UpperCamelCase( Subject.Key ).ToLower(),
              extensionKey = Subject.Key,
              extensionTitle = Subject.Title
            };

      Log.Info( "Generating TypoScript constants..." );
      string constants = constantsTemplate.FormatSmart( dataObject );
      WriteFile( "Configuration/TypoScript/constants.txt", constants, DateTime.UtcNow );

      Log.Info( "Generating TypoScript setup..." );
      string setup = setupTemplate.FormatSmart( dataObject );
      WriteFile( "Configuration/TypoScript/setup.txt", setup, DateTime.UtcNow );

      const string typoScriptRegisterTemplate =
        "t3lib_extMgm::addStaticFile('{extensionKey}', 'Configuration/TypoScript', '{extensionTitle} Base');";

      WriteVirtual( "ext_tables.php", typoScriptRegisterTemplate.FormatSmart( dataObject ) );
    }

    /// <summary>
    /// Generates the class that will provide the label hooks for the extension
    /// </summary>
    /// <param name="phpClassSuffix"></param>
    /// <exception cref="GeneratorException">Label hook implementation does not exist.</exception>
    private void WriteLabelHooks( string phpClassSuffix ) {
      if( string.IsNullOrEmpty( Subject.LabelHookImplementation ) ) return;

      if( !File.Exists( Subject.LabelHookImplementation ) ) {
        throw new GeneratorException(
          string.Format( "Label hook implementation '{0}' does not exist.", Subject.LabelHookImplementation ),
          Subject.SourceFragment.SourceDocument );
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
              new {implClassname = implementationClassname} )
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
