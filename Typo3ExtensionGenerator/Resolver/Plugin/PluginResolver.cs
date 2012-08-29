using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Plugin {
  public static class PluginResolver {
    /// <summary>
    /// Resolves the plugins of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The plauings of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Plugin> Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      IEnumerable<ExtensionParser.ParsedPartial> pluginPartials = parsedPartial.Partials.Where( p => p.Keyword == Keywords.DeclarePlugin );
      if( !pluginPartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Plugin> plugins = new List<Typo3ExtensionGenerator.Model.Plugin>();
      foreach( ExtensionParser.ParsedPartial pluginPartial in pluginPartials ) {
        Typo3ExtensionGenerator.Model.Plugin plugin = new Typo3ExtensionGenerator.Model.Plugin {Name = pluginPartial.Parameters};

        // Resolve plugin
        foreach( ExtensionParser.ParsedPartial pluginParameter in pluginPartial.Partials ) {
          if( Keywords.Title == pluginParameter.Keyword ) {
            plugin.Title = pluginParameter.Parameters;
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
