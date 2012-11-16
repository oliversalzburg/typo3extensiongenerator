using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Configuration.Interface;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Plugin;
using Action = System.Action;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public static class ServiceResolver {
    public static List<Service> Resolve( Fragment parsedFragment ) {
       IEnumerable<Fragment> serviceFragments = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareService );
      if( !serviceFragments.Any() ) return null;

      List<Service> services = new List<Service>();
      foreach( Fragment serviceFragment in serviceFragments ) {
        // Construct the plugin with the given name
        Service service = new Service {Name = serviceFragment.Parameters, SourceFragment = serviceFragment};
        
        // Resolve service
        foreach( Fragment serviceParameter in serviceFragment.Fragments ) {
          if( serviceParameter.Keyword == Keywords.PluginDirectives.Action ) {
            Typo3ExtensionGenerator.Model.Action action = ActionResolver.ResolveAction( serviceParameter );
            service.Actions.Add( action );

          } else if( serviceParameter.Keyword == Keywords.PluginDirectives.Listener ) {
            Listener listener = ListenerResolver.ResolveListener( serviceParameter );
            service.Actions.Add( listener.TargetAction );
            throw new NotImplementedException();
            //service.Listeners.Add( listener );
            
          } else if( serviceParameter.Keyword == Keywords.PluginDirectives.Implementation ) {
            service.Implementation = serviceParameter.Parameters;
          }
        }

        // If no name was defined, use the common placeholder names
        if( string.IsNullOrEmpty( service.Name ) ) {
          throw new ParserException( "Service has no name!", parsedFragment.SourceDocument );
        }

        services.Add( service );
      }

      return services;
    }
  }
}
