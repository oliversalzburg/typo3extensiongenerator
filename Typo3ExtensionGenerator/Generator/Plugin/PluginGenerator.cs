﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartFormat;
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
        Typo3ExtensionGenerator.Model.Plugin.Plugin plugin = Subject.Plugins[ pluginIndex ];
        
        string languageConstant = String.Format( "{0}_title", plugin.Name.ToLower() );
        extTables.Append( String.Format( registerPlugin, Subject.Key, NameHelper.UpperCamelCase( plugin.Name ), languageConstant ) + "\n" );

        // Also write label string to language file
        WriteFile( "Resources/Private/Language/locallang_be.xml", string.Format( "<label index=\"{0}\">{1}</label>", languageConstant, plugin.Title ), true );

        string extensionName = NameHelper.UpperCamelCase( Subject.Key );
        string pluginSignature = String.Format( "{0}_{1}", extensionName.ToLower(), plugin.Name );

        // Register FlexForm through ext_tables
        extTables.Append(
          string.Format(
            "$TCA['tt_content']['types']['list']['subtypes_addlist']['{0}'] = 'pi_flexform';\n", pluginSignature ) );
        extTables.Append(
          string.Format(
            "t3lib_extMgm::addPiFlexFormValue('{0}', 'FILE:EXT:{1}/Configuration/FlexForms/flexform_{2}.xml');\n",
            pluginSignature, Subject.Key, plugin.Name.ToLower() ) );

        // Add configurePlugin line to ext_localconf
        extLocalconf.Append( string.Format( configurePlugin, Subject.Key, NameHelper.UpperCamelCase( plugin.Name ) ) + "\n" );

        // Resolve the foreign key references in the flexform model
        ForeignKeyResolver.Resolve( new List<DataModel> {plugin.Model}, Subject.Models );

        // Generate the flexform XML for all interface elements.
        GenerateFlexForm( plugin );

        // Generate the ExtBase controller for this plugin.
        GenerateController( plugin );
      }

      string extTablesPhp = extTables.ToString().Substring( 0, extTables.Length - 1 );
      string extLocalconfPhp = extLocalconf.ToString().Substring( 0, extLocalconf.Length - 1 );
      WriteFile( "ext_tables.php", extTablesPhp, true );
      WriteFile( "ext_localconf.php", extLocalconfPhp, true );
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
      const string membersTemplate = "\n<{0}.settings><TCEforms>{1}</TCEforms></{0}.settings>";
      foreach( Interface @interface in plugin.Interfaces ) {
        string members = InterfaceGenerator.Generate( this, Subject, @interface, SimpleContainer.Format.Xml );

        string setting = string.Format( membersTemplate, @interface.Target, members );
        settings.Append( setting );
      }

      // Generate actions
      const string actionTemplate = "                  <numIndex index=\"{actIndex}\">" +
                                    "                    <numIndex index=\"0\">{title}</numIndex>" +
                                    "                    <numIndex index=\"1\">{controllerName}->{actionName};</numIndex>" +
                                    "                  </numIndex>";

      int actionIndex = 0;
      foreach( Action action in plugin.Actions ) {
        var dataObject =
          new {
                title          = action.Title,
                controllerName = NameHelper.UpperCamelCase( plugin.Name ),
                actionName     = action.Name,
                actIndex       = actionIndex
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
      const string actionTemplate = "public function {0}Action({1}) {{}}\n";

      foreach( Action action in plugin.Actions ) {
        // Prefix each parameter with a $ and join them together with , in between.
        string parameters = action.Requirements.Aggregate(
          string.Empty,
          ( current, requirement ) =>
          current + ( "$" + requirement + ( ( requirement != action.Requirements.Last() ) ? "," : string.Empty ) ) );

        actions.Append( String.Format( actionTemplate, action.Name, parameters ) );
      }

      const string controllerTemplate = "class {className} extends Tx_Extbase_MVC_Controller_ActionController {{\n{controllerActions}}}";

      string controller =
        controllerTemplate.FormatSmart(
          new {
                className         = NameHelper.GetExtbaseControllerClassName( Subject, plugin ),
                controllerActions = actions.ToString()
              } );

      WritePhpFile(
        string.Format( "Classes/Controller/{0}", NameHelper.GetExtbaseControllerFileName( Subject, plugin ) ),
        controller );
    }
  }
}
