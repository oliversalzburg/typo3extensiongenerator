using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public static class RepositoryResolver {
    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    public static List<Repository> Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      IEnumerable<ExtensionParser.ParsedPartial> repositoryPartials = parsedPartial.Partials.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareRepository );
      if( !repositoryPartials.Any() ) return null;

      List<Repository> repositories = new List<Repository>();
      foreach( ExtensionParser.ParsedPartial repositoryPartial in repositoryPartials ) {
        Repository repository = new Repository {
                                                 TargetModelName = repositoryPartial.Parameters,
                                                 SourcePartial = repositoryPartial,
                                                 SourceLine = repositoryPartial.Line
                                               };
        repositories.Add( repository );
        if( repositoryPartial.Partials.Any() ) {
          foreach( ExtensionParser.ParsedPartial dataMember in repositoryPartial.Partials ) {
            if( dataMember.Keyword == Keywords.PluginDirectives.Action ) {
              Action action = ActionResolver.ResolveAction( dataMember );
              repository.Methods.Add( action );
            } else if( dataMember.Keyword == Keywords.PluginDirectives.Implementation) {
              repository.Implementation = dataMember.Parameters;
            }
          }
        }
      }

      return repositories;
    }
  }
}
