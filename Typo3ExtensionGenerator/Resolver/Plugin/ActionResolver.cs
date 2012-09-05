using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Plugin {
  public static class ActionResolver {
    public static Action ResolveAction( ExtensionParser.ParsedPartial parsedPartial ) {
      Action resultingAction = new Action {Name = parsedPartial.Parameters};
      foreach( ExtensionParser.ParsedPartial actionDirective in parsedPartial.Partials ) {
        if( actionDirective.Keyword == Keywords.Title ) {
          resultingAction.Title = actionDirective.Parameters;

        } else if( actionDirective.Keyword == Keywords.Requirement ) {
          resultingAction.Requirements.AddRange( actionDirective.Parameters.Split( new[] {','} ) );
        
        } else if( actionDirective.Keyword == Keywords.PluginDirectives.Uncachable ) {
          resultingAction.Uncachable = true;
        }
      }
      
      return resultingAction;
    }
  }
}
