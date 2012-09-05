using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using SmartFormat.Core;
using Typo3ExtensionGenerator.Generator.Configuration;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resolver.Model;
using log4net;
using Action = Typo3ExtensionGenerator.Model.Plugin.Action;

namespace Typo3ExtensionGenerator.Generator.Plugin {
  public class PluginGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public PluginGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {

      Log.Info( "Generating plugins..." );

      GeneratePlugin();
    }

    /// <summary>
    /// Generates the plugins.
    /// </summary>
    /// <returns></returns>
    private void GeneratePlugin( ) {
      StringBuilder extTables = new StringBuilder();
      StringBuilder extLocalconf = new StringBuilder();

      const string registerPlugin = "Tx_Extbase_Utility_Extension::registerPlugin(\n" +
                                    "  '{0}',\n" +
                                    "  '{1}',\n" +
                                    "  'LLL:EXT:{0}/Resources/Private/Language/locallang_be.xml:{2}'\n" +
                                    ");";

      const string controllerAction = "    '{controllerName}' => '{actionList}'";

      const string configurePlugin = "Tx_Extbase_Utility_Extension::configurePlugin(" +
                                     "  '{extensionKey}'," +
                                     "  '{pluginName}'," +
                                     "  array(" +
                                     "{cachableActions}" +
                                     "  )," +
                                     "  array(" +
                                     "{unCachableActions}" +
                                     "  )" +
                                     ");";

      for( int pluginIndex = 0; pluginIndex < Subject.Plugins.Count; pluginIndex++ ) {
        Typo3ExtensionGenerator.Model.Plugin.Plugin plugin = Subject.Plugins[ pluginIndex ];
        
        string languageConstant = String.Format( "{0}_title", plugin.Name.ToLower() );
        extTables.Append( String.Format( registerPlugin, Subject.Key, NameHelper.UpperCamelCase( plugin.Name ), languageConstant ) + "\n" );

        // Also write label string to language file
        WriteFile( "Resources/Private/Language/locallang_be.xml", string.Format( "<label index=\"{0}\">{1}</label>", languageConstant, plugin.Title ), true );

        // Generate the plugin signature which will be used in the tt_content table and as a flexform ID.
        string pluginSignature = String.Format( "{0}_{1}", NameHelper.UpperCamelCase( Subject.Key ).ToLower(), plugin.Name.ToLower() );

        // Register FlexForm through ext_tables
        extTables.Append(
          string.Format(
            "$TCA['tt_content']['types']['list']['subtypes_addlist']['{0}'] = 'pi_flexform';\n", pluginSignature ) );
        extTables.Append(
          string.Format(
            "t3lib_extMgm::addPiFlexFormValue('{0}', 'FILE:EXT:{1}/Configuration/FlexForms/flexform_{2}.xml');\n",
            pluginSignature, Subject.Key, plugin.Name.ToLower() ) );

        // Generate allowed action combination that should be configured
        StringBuilder actions = new StringBuilder();
        StringBuilder uncachableActions = new StringBuilder();
        foreach( Action action in plugin.Actions ) {
          if( action.Uncachable ) {
            uncachableActions.Append(  action.Name + "," );
          } else {
            actions.Append( action.Name + "," );
          }
        }
        
        string actionsCachable = actions.ToString();
        actionsCachable = ( actionsCachable.Length > 0 )
                            ? actionsCachable.Substring( 0, actionsCachable.Length - 1 )
                            : actionsCachable;
        string actionsUncachable = uncachableActions.ToString();
        actionsUncachable = ( actionsUncachable.Length > 0 )
                            ? actionsUncachable.Substring( 0, actionsUncachable.Length - 1 )
                            : actionsUncachable;
        var controllerData =
            new {controllerName = NameHelper.UpperCamelCase( plugin.Name ), actionList = actionsCachable };
        var uncachableControllerData =
            new {controllerName = NameHelper.UpperCamelCase( plugin.Name ), actionList = actionsUncachable };

        string allControllerActions = controllerAction.FormatSmart( controllerData );
        string allUncachableControllerActions = controllerAction.FormatSmart( uncachableControllerData );


        // Add configurePlugin line to ext_localconf
        var configurePluginData =
          new {
                extensionKey = Subject.Key,
                pluginName = NameHelper.UpperCamelCase( plugin.Name ),
                cachableActions = allControllerActions,
                unCachableActions = allUncachableControllerActions,
              };
        
        extLocalconf.Append( configurePlugin.FormatSmart( configurePluginData ) + "\n" );

        // Resolve the foreign key references in the flexform model
        ForeignKeyResolver.Resolve( new List<DataModel> {plugin.Model}, Subject.Models );

        // Generate the flexform XML for all interface elements.
        GenerateFlexForm( plugin );

        // Generate the ExtBase controller for this plugin.
        GenerateController( plugin );

        // Generate the typoscript for this plugin.
        GenerateTypoScript( plugin );

        // Generate Fluid templating elements
        GenerateFluidTemplate( plugin );
      }

      string extTablesPhp = extTables.ToString().Substring( 0, extTables.Length - 1 );
      string extLocalconfPhp = extLocalconf.ToString().Substring( 0, extLocalconf.Length - 1 );
      WriteFile( "ext_tables.php", extTablesPhp, true );
      WriteFile( "ext_localconf.php", extLocalconfPhp, true );
    }

    private void GenerateFluidTemplate( Typo3ExtensionGenerator.Model.Plugin.Plugin plugin ) {
      // The default frontend layout
      const string defaultLayoutTemplate = "{namespace d=Tx_Downloads_ViewHelpers}" +
                                           "<d:includeFile path=\"EXT:downloads/Resources/Public/Css/downloads.css\" />" +
                                           "<div class=\"tx-downloads\">" +
                                           "	<f:render section=\"main\" />" +
                                           "</div>";

      const string layoutFilename = "Resources/Private/Layouts/Default.html";
      Log.InfoFormat( "Generating default Fluid layout '{0}'...", layoutFilename );
      WriteFile( layoutFilename, defaultLayoutTemplate );

      // A generic partial that will render all the fields in a model
      //WriteFile( "Resources/Private/Partials/Models/Download.html", string.Empty );

      const string defaultTemplateTemplate = "<f:layout name=\"Default\" />" +
                                             "<f:section name=\"main\">" +
                                             "  <f:flashMessages class=\"flashMessages\" />" +
                                             "  <h3>You need to create your own Fluid templates and point TYPO3 to them via TypoScript.</h3>" +
/*
"  <f:groupedFor each="{downloads}" as="category" groupBy="downloadCategory" groupKey="downloadCategory">"+
"  <h2>{downloadCategory.name}</h2>"+
"  <f:for each="{category}" as="download">"+
"    <f:render partial="List/Download" arguments="{download: download}"/>"+
"  </f:for>"+
"  </f:groupedFor>"+
*/
                                             "</f:section>";

      // Write Fluid templates for each action
      foreach( Action action in plugin.Actions ) {
        // The template that is used for the given action of the given controller
        string templateFilename = string.Format(
          "Resources/Private/Templates/{0}/{1}.html", NameHelper.UpperCamelCase( plugin.Name ),
          NameHelper.UpperCamelCase( action.Name ) );

        Log.InfoFormat( "Generating Fluid template '{0}'...", templateFilename );
        WriteFile( templateFilename, defaultTemplateTemplate );
      }
    }

    /// <summary>
    /// Generates the FlexForm XML for the given plugin.
    /// </summary>
    /// <param name="plugin"></param>
    private void GenerateFlexForm( Typo3ExtensionGenerator.Model.Plugin.Plugin plugin ) {

      Log.InfoFormat( "Generating FlexForm for '{0}'...", plugin.Name );

      // Collect FlexForms from interfaces
        StringBuilder settings = new StringBuilder();
        StringBuilder actions  = new StringBuilder();

      // TODO: Should possibly render ALL fields in the model
      const string membersTemplate = "\n<settings.{0}><TCEforms>{1}</TCEforms></settings.{0}>";
      foreach( Interface @interface in plugin.Interfaces ) {
        string members = InterfaceGenerator.Generate( this, Subject, @interface, SimpleContainer.Format.Xml );

        string setting = string.Format( membersTemplate, @interface.Target, members );
        settings.Append( setting );
      }

      // Generate actions
      const string controllerActionTemplate = "{controllerName}->{actionName}";
      const string actionTemplate = "                  <numIndex index=\"{actIndex}\">" +
                                    "                    <numIndex index=\"0\">{title}</numIndex>" +
                                    "                    <numIndex index=\"1\">{controllerName}->{actionName};{nonDefault}</numIndex>" +
                                    "                  </numIndex>";

      // We'll generate an entry for each action in the controller.
      // The combination will always list the given action as the first (default) action; all other actions will be allowed in that combination as well.
      int actionIndex = 0;
      foreach( Action action in plugin.Actions ) {
        string nonDefault = plugin.Actions.Where( nonDefaultAction => nonDefaultAction != action ).Aggregate(
          string.Empty, ( current, nonDefaultAction ) => current + ( controllerActionTemplate.FormatSmart(
            new {
                  controllerName = NameHelper.UpperCamelCase( plugin.Name ),
                  actionName = nonDefaultAction.Name
                } ) + ";" ) );
        if( !string.IsNullOrEmpty( nonDefault ) ) {
          nonDefault = nonDefault.Substring( 0, nonDefault.Length - 1 );
        }

        var dataObject =
          new {
                title          = action.Title,
                controllerName = NameHelper.UpperCamelCase( plugin.Name ),
                actionName     = action.Name,
                actIndex       = actionIndex,
                nonDefault     = nonDefault.ToString()
              };
        string actionString = actionTemplate.FormatSmart( dataObject );
        actions.Append( actionString );

        

        ++actionIndex;
      }

      string allActions = actions.ToString();
      string allSettings = settings.ToString();
      const string actionsTemplate = "<switchableControllerActions><TCEforms><label>Action</label><config><type>select</type><items>{0}</items></config></TCEforms></switchableControllerActions>";

      const string flexFormTemplate = "<T3DataStructure>" +
                                      "  <meta>" +
                                      "    <langDisable>1</langDisable>" +
                                      "  </meta>" +
                                      "  " +
                                      "  <sheets>" +
                                      "    <sDEF>" +
                                      "      <ROOT>" +
                                      "        <TCEforms>" +
                                      "         <sheetTitle>General</sheetTitle>" +
                                      "        </TCEforms>" +
                                      "        <type>array</type>" +
                                      "        <el>" +
                                      "{actions}" +
                                      "{settings}" +
                                      "        </el>" +
                                      "      </ROOT>" +
                                      "    </sDEF>" +
                                      "  </sheets>" +
                                      "</T3DataStructure>";

      var dataObjectFlexForm = new {settings = allSettings, actions = string.Format( actionsTemplate, allActions )};
      string flexFormsXml = flexFormTemplate.FormatSmart( dataObjectFlexForm );

      // Write final result to file
      WriteFile( string.Format( "Configuration/FlexForms/flexform_{0}.xml", plugin.Name.ToLower() ), flexFormsXml, true );
    }

    /// <summary>
    /// Generates the ExtBase controller for the given plugin.
    /// </summary>
    /// <param name="plugin"></param>
    private void GenerateController( Typo3ExtensionGenerator.Model.Plugin.Plugin plugin ) {
      string className = NameHelper.GetExtbaseControllerClassName( Subject, plugin );
      Log.InfoFormat( "Generating controller '{0}'...", className );

      StringBuilder actions = new StringBuilder();
      const string actionTemplate = "/**\n" +
                                    "{2}" +
                                    "*/\n" +
                                    "public function {0}Action({1}) {{ return $this->getImplementation()->{0}Action({1}); }}\n";

      foreach( Action action in plugin.Actions ) {
        string phpDoc = action.Requirements.Aggregate(
          string.Empty, ( current, requirement ) => current + ( "* @param mixed " + requirement + "\n" ) );

        // Prefix each parameter with a $ and join them together with , in between.
        string parameters = action.Requirements.Aggregate(
          string.Empty,
          ( current, requirement ) =>
          current + ( "$" + requirement + ( ( requirement != action.Requirements.Last() ) ? "," : string.Empty ) ) );

        actions.Append( String.Format( actionTemplate, action.Name, parameters, phpDoc ) );
      }

      bool isExternallyImplemented = false;
      string implementationClassname = string.Empty;
      string implementationFilename = string.Empty;
      if( !string.IsNullOrEmpty( plugin.Implementation ) ) {
        isExternallyImplemented = true;
        implementationClassname = NameHelper.GetExtbaseControllerImplementationClassName( Subject, plugin );
        implementationFilename  = NameHelper.GetExtbaseControllerImplementationFileName( Subject, plugin );
      }

      if( isExternallyImplemented ) {
        if( !File.Exists( plugin.Implementation ) ) {
          throw new GeneratorException(
            string.Format( "Implementation '{0}' for plugin '{1}' does not exist.", plugin.Implementation, plugin.Name ),
            plugin.SourceLine );
        }
        Log.InfoFormat( "Merging implementation '{0}'...", plugin.Implementation );
        string pluginImplementation = File.ReadAllText( plugin.Implementation );
        if( !Regex.IsMatch( pluginImplementation, String.Format( "class {0} ?({{|extends|implements)", implementationClassname ) ) ) {
          Log.WarnFormat( "The class name of your implementation MUST be '{0}'!", implementationClassname );  
        }
        WriteFile( "Classes/Controller/" + implementationFilename, pluginImplementation );

      } else {
        if( plugin.Actions.Count > 0 ) {
          Log.WarnFormat(
            "Plugin '{0}' defines actions, but has no implementation provided. If any of these actions is invoked by TYPO3, a PHP error will be generated!",
            plugin.Name );
        }
      }

      const string controllerImplementationTemplate = "private $implementation;\n" +
                                                      "private function getImplementation() {{\n" +
                                                      "  if( null == $this->implementation ) {{\n" +
                                                      "    $this->implementation = new {implClassname}($this);\n" +
                                                      "  }}\n" +
                                                      "  return $this->implementation;\n" +
                                                      "}}\n"+
                                                      "function __construct() {{\n"+
                                                      "}}\n";

      StringBuilder propertiesList = new StringBuilder();
      foreach( DataModel dataModel in Subject.Models ) {
        const string memberTemplate = "/**\n" +
                                      "* {0}Repository\n" +
                                      "* @var {1}\n" +
                                      "*/\n" +
                                      "protected ${0}Repository;\n";

        // Check if the repository is internally implemented.
        // An example for an inernally implemented repository would be Tx_Extbase_Domain_Repository_FrontendUserRepository
        Repository repository = Subject.Repositories.SingleOrDefault( r => r.TargetModelName == dataModel.Name );
        if( null != repository && !string.IsNullOrEmpty( repository.InternalType ) ) {
          propertiesList.Append(
            String.Format(
              memberTemplate, dataModel.Name, repository.InternalType ) );

        } else {

          propertiesList.Append(
            String.Format(
              memberTemplate, dataModel.Name, NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, dataModel ) ) );
        }

      const string injectorTemplate =
          "/**\n"+
          "* inject{0}Repository\n"+
          "* @param {1} ${2}Repository\n"+
          "*/\n"+
          "public function inject{0}Repository({1} ${2}Repository) {{\n" +
          "  $this->{2}Repository = ${2}Repository;\n" +
          "}}\n";

        // Check again if the repository is internally implemented.
        // An example for an inernally implemented repository would be Tx_Extbase_Domain_Repository_FrontendUserRepository
        string injector = string.Empty;
        if( null != repository && !string.IsNullOrEmpty( repository.InternalType ) ) {
          injector = String.Format(
            injectorTemplate, NameHelper.UpperCamelCase( dataModel.Name ), repository.InternalType, dataModel.Name );

        } else {
          injector = String.Format(
            injectorTemplate, NameHelper.UpperCamelCase( dataModel.Name ),
            NameHelper.GetExtbaseDomainModelRepositoryClassName( Subject, dataModel ), dataModel.Name );
        }

        propertiesList.Append( injector );
      }

      string controllerImplementation =
        controllerImplementationTemplate.FormatSmart(
          new {
                implClassname = implementationClassname,
                className     = NameHelper.GetExtbaseControllerClassName( Subject, plugin )
              } );

      const string controllerTemplate = "class {className} extends Tx_Extbase_MVC_Controller_ActionController {{\n" +
                                        "{controllerProperties}\n" +
                                        "{controllerActions}\n" +
                                        "}}\n" +
                                        "{requireImplementation}";

      string controller =
        controllerTemplate.FormatSmart(
          new {
                className             = NameHelper.GetExtbaseControllerClassName( Subject, plugin ),
                controllerProperties  = (( isExternallyImplemented ) ? controllerImplementation : string.Empty) + propertiesList,
                controllerActions     = actions.ToString(),
                requireImplementation = ( isExternallyImplemented ) ? string.Format( "require_once('{0}');\n", implementationFilename ) : string.Empty
              } );

      WritePhpFile(
        string.Format( "Classes/Controller/{0}", NameHelper.GetExtbaseControllerFileName( Subject, plugin ) ),
        controller );
    }


    /// <summary>
    /// Writes the default TypoScript constants and settings files for the extension.
    /// </summary>
    /// <param name="plugin"> </param>
    private void GenerateTypoScript( Typo3ExtensionGenerator.Model.Plugin.Plugin plugin ) {
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

      // Just to be safe, we also go all lower-case here.
      var dataObject =
        new {extensionName = "tx_" + NameHelper.UpperCamelCase( Subject.Key ).ToLower(), extensionKey = Subject.Key};

      Log.Info( "Generating TypoScript constants..." );
      string constants = constantsTemplate.FormatSmart( dataObject );
      WriteFile( "Configuration/TypoScript/constants.txt", constants );

      Log.Info( "Generating TypoScript setup..." );
      string setup = setupTemplate.FormatSmart( dataObject );
      WriteFile( "Configuration/TypoScript/setup.txt", setup );

      // TODO: Write plugin-specific TS files
      //const string typoScriptRegisterTemplate = "t3lib_extMgm::addStaticFile('{extensionKey}', 'Configuration/TypoScript', '{extensionTitle}');";
      //WriteFile( "ext_tables.php", typoScriptRegisterTemplate.FormatSmart( dataObject ), true );
    }
  }
}
