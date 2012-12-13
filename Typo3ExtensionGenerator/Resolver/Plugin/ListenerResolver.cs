using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Plugin {
  /// <summary>
  /// Resolves ExtBase SignalSlot listeners from the extension markup.
  /// </summary>
  public static class ListenerResolver {
    /// <summary>
    /// Find an ExtBase SignalSlot listener in a parsed fragment and return it.
    /// </summary>
    /// <param name="parsedFragment"></param>
    /// <returns></returns>
    /// <exception cref="ParserException">Listener does not define a signal host or slot.</exception>
    public static Listener ResolveListener( Fragment parsedFragment ) {
      Listener resultingListener = new Listener();
      resultingListener.TargetAction = ActionResolver.ResolveAction( parsedFragment );

      foreach( Fragment actionDirective in parsedFragment.Fragments ) {
        if( actionDirective.Keyword == Keywords.PluginDirectives.ActionDirectives.Host ) {
          resultingListener.Host = actionDirective.Parameters;

        } else if( actionDirective.Keyword == Keywords.PluginDirectives.ActionDirectives.Slot ) {
          resultingListener.Signal = actionDirective.Parameters;
        }
      }
      
      if( string.IsNullOrEmpty( resultingListener.Host ) ) {
        throw new ParserException( string.Format( "Listener '{0}' does not define a signal host.", parsedFragment.Parameters ), parsedFragment.SourceDocument );
      }
      if( string.IsNullOrEmpty( resultingListener.Signal ) ) {
        throw new ParserException( string.Format( "Listener '{0}' does not define a signal slot.", parsedFragment.Parameters ), parsedFragment.SourceDocument );
      }
      return resultingListener;
    }
  }
}
