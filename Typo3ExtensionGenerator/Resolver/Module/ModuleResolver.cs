using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Module {
  public static class ModuleResolver {
    /// <summary>
    /// Resolves the modules of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The plauings of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Module> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> modulePartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareModule );
      if( !modulePartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Module> modules = new List<Typo3ExtensionGenerator.Model.Module>();
      foreach( Fragment modulePartial in modulePartials ) {
        Typo3ExtensionGenerator.Model.Module module = new Typo3ExtensionGenerator.Model.Module {Name = modulePartial.Parameters};
        foreach( Fragment subPartial in modulePartial.Fragments ) {
          if( subPartial.Keyword == Keywords.Category ) {
            module.MainModuleName = subPartial.Parameters;

          } else if( subPartial.Keyword == Keywords.Title ) {
            module.Title = subPartial.Parameters;
          }
        }
        modules.Add( module );
      }

      return modules;
    }
  }
}
