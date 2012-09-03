using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Module {
  public static class ModuleResolver {
    /// <summary>
    /// Resolves the modules of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The plauings of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Module> Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      IEnumerable<ExtensionParser.ParsedPartial> modulePartials = parsedPartial.Partials.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareModule );
      if( !modulePartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Module> modules = new List<Typo3ExtensionGenerator.Model.Module>();
      foreach( ExtensionParser.ParsedPartial modulePartial in modulePartials ) {
        Typo3ExtensionGenerator.Model.Module module = new Typo3ExtensionGenerator.Model.Module {Name = modulePartial.Parameters};
        foreach( ExtensionParser.ParsedPartial subPartial in modulePartial.Partials ) {
          if( subPartial.Keyword == Keywords.Category ) {
            module.MainModuleName = subPartial.Parameters;
          }
        }
        modules.Add( module );
      }

      return modules;
    }
  }
}
