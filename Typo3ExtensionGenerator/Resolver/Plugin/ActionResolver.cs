using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Plugin {
  public static class ActionResolver {
    public static Action ResolveAction( Fragment parsedFragment ) {
      Action resultingAction = new Action {Name = parsedFragment.Parameters};
      foreach( Fragment actionDirective in parsedFragment.Fragments ) {
        if( actionDirective.Keyword == Keywords.Title ) {
          resultingAction.Title = actionDirective.Parameters;

        } else if( actionDirective.Keyword == Keywords.Requirement ) {
          resultingAction.Requirements.AddRange( actionDirective.Parameters.Split( new[] {','} ) );
        
        } else if( actionDirective.Keyword == Keywords.PluginDirectives.ActionDirectives.Uncachable ) {
          resultingAction.Uncachable = true;

        }
      }
      
      return resultingAction;
    }
  }
}
