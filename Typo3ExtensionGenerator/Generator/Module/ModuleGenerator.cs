﻿using System;
using System.Linq;
using System.Text;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Class;
using Typo3ExtensionGenerator.Generator.Class.Naming;
using Typo3ExtensionGenerator.Generator.Helper;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator.Module {
  /// <summary>
  /// Generates backend modules for TYPO3
  /// </summary>
  public class ModuleGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="extension">The extension that is being worked on.</param>
    public ModuleGenerator( Context context, Extension extension ) : base( context, extension ) {}

    /// <summary>
    /// Generate all backend modules
    /// </summary>
    public void Generate() {
      GenerateModules();
    }

    /// <summary>
    /// Generates the PHP statements to register the modules.
    /// </summary>
    /// <returns></returns>
    private void GenerateModules() {
      if( null == Subject.Modules || !Subject.Modules.Any() ) return;

      StringBuilder result = new StringBuilder();

      Log.Info( "Registering modules..." );

      const string template = "if( TYPO3_MODE === 'BE' ) {{\n" +
                              "  Tx_Extbase_Utility_Extension::registerModule(\n" +
                              "    '{_extensionName}',\n" +
                              "    '{_mainModuleName}',\n" +
                              "    '{_subModuleName}',\n" +
                              "    '',\n" +
                              "    array(\n" +
                              "      {_actions}\n" +
                              "    ),\n" +
                              "    array(\n" +
                              "      'access' => 'user,group',\n" +
                              "      'icon'   => 'EXT:{_extensionName}/ext_icon.gif',\n" +
                              "      'labels' => 'LLL:EXT:{_extensionName}/Resources/Private/Language/locallang_{_langFileKey}.xml',\n" +
                              "    )\n" +
                              "  );\n" +
                              "}}";

      for( int moduleIndex = 0; moduleIndex < Subject.Modules.Count; moduleIndex++ ) {
        Typo3ExtensionGenerator.Model.Module module = Subject.Modules[ moduleIndex ];

        Log.InfoFormat( "Registering module '{0}'...", module.Name );

        ActionAggregator.AggregationResult aggregationResult = ActionAggregator.Aggregate( module, true );
        

        //string subKey = string.Format( "m{0}", moduleIndex + 1 );
        //string moduleKey = string.Format( "tx_{0}_{1}", Subject.Key, subKey );
        string moduleKey = NameHelper.GetModuleSignature( Subject, module );

        result.Append(
          template.FormatSmart(
            new {
                  _extensionName  = Subject.Key,
                  _mainModuleName = module.MainModuleName,
                  _subModuleName  = moduleKey,
                  _langFileKey    = module.Name.ToLower(),
                  _actions        = aggregationResult.Uncachable
                } ) + "\n" );

        // Valid labels that should/could be generated
        // <label index="mlang_tabs_tab">Download Importer</label>
        // <label index="mlang_labels_tabdescr">Import download records from files on the file system.</label>
        // <label index="mlang_labels_tablabel">Create download records from files on the file system.</label>

        WriteVirtual( string.Format( "Resources/Private/Language/locallang_{0}.xml", module.Name.ToLower() ), string.Format( "<label index=\"{0}\">{1}</label>", "mlang_tabs_tab", module.Title ) );

        ClassProxyGenerator classGenerator = new ClassProxyGenerator( GeneratorContext, Subject );
        classGenerator.GenerateClassProxy( module, new ControllerNamingStrategy(), "Classes/Controller/", true );
      }

      string modules = result.ToString().Substring( 0, result.Length - 1 );
      WriteVirtual( "ext_tables.php", modules );
    }    
  }
}
