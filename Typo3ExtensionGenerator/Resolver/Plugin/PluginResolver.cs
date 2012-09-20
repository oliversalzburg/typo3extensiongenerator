using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Configuration.Interface;
using Typo3ExtensionGenerator.Resolver.Model;

namespace Typo3ExtensionGenerator.Resolver.Plugin {
  public static class PluginResolver {
    /// <summary>
    /// Resolves the plugins of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The plauings of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Plugin.Plugin> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> pluginPartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclarePlugin );
      if( !pluginPartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Plugin.Plugin> plugins = new List<Typo3ExtensionGenerator.Model.Plugin.Plugin>();
      foreach( Fragment pluginPartial in pluginPartials ) {
        // Construct the plugin with the given name
        Typo3ExtensionGenerator.Model.Plugin.Plugin plugin = new Typo3ExtensionGenerator.Model.Plugin.Plugin
                                                             {Name = pluginPartial.Parameters};

        // Find the data models that are defined for this plugin
        List<DataModel> dataModels = ModelResolver.Resolve( pluginPartial );
        if( null == dataModels || dataModels.Count > 1 ) {
          throw new GeneratorException( "A plugin must contain exactly one model.", pluginPartial.SourceDocument );
        }
        plugin.Model = dataModels[ 0 ];
        plugin.Model.Name = "flexform";

        // Resolve plugin
        foreach( Fragment pluginParameter in pluginPartial.Fragments ) {
          if( pluginParameter.Keyword == Keywords.Title) {
            plugin.Title = pluginParameter.Parameters;

          } else if( pluginParameter.Keyword == Keywords.DefineInterface ) {
            Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface =
              InterfaceResolver.Resolve( pluginParameter );
            
            @interface.ParentModelTarget = "flexform";
            @interface.ParentModel = plugin.Model;
            plugin.Interfaces.Add( @interface );
          
          } else if( pluginParameter.Keyword == Keywords.PluginDirectives.Action ) {
            Action action = ActionResolver.ResolveAction( pluginParameter );
            plugin.Actions.Add( action );

          } else if( pluginParameter.Keyword == Keywords.PluginDirectives.Implementation ) {
            plugin.Implementation = pluginParameter.Parameters;
          }
        }

        // If no name was defined, use the common placeholder names
        if( string.IsNullOrEmpty( plugin.Name ) ) {
          plugin.Name = string.Format( "Pi{0}", ( plugins.Count + 1 ) );
        }

        plugins.Add( plugin );
      }

      return plugins;
    }
  }
}
