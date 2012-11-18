using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public static class RepositoryResolver {
    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    public static List<Repository> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> repositoryPartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareRepository );
      if( !repositoryPartials.Any() ) return null;

      List<Repository> repositories = new List<Repository>();
      foreach( Fragment repositoryPartial in repositoryPartials ) {
        Repository repository = new Repository {
                                                 TargetModelName = repositoryPartial.Parameters,
                                                 SourceFragment = repositoryPartial,
                                               };
        repositories.Add( repository );
        if( repositoryPartial.Fragments.Any() ) {
          foreach( Fragment dataMember in repositoryPartial.Fragments ) {
            if( dataMember.Keyword == Keywords.PluginDirectives.Action ) {
              Action action = ActionResolver.ResolveAction( dataMember );
              repository.Methods.Add( action );

            } else if( dataMember.Keyword == Keywords.Implementation) {
              repository.Implementation = dataMember.Parameters;
            
            } else if( dataMember.Keyword == Keywords.InternalType ) {
              repository.InternalType = dataMember.Parameters;
            }
          }
        }
      }

      return repositories;
    }
  }
}
