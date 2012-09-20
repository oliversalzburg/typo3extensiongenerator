using System.Collections.Generic;
using System.IO;
using System.Linq;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Model;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public static class RequirementResolver {
    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    public static List<Requirement> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> requirementPartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.Requirement );
      if( !requirementPartials.Any() ) return null;

      List<Requirement> requirements = new List<Requirement>();
      foreach( Fragment requirementPartial in requirementPartials ) {
        Requirement requirement = new Requirement {
                                                    SourceFolder = requirementPartial.Parameters,
                                                    SourceFragment = requirementPartial
                                                  };
        requirements.Add( requirement );

        if( requirementPartial.Fragments.Any() ) {
          foreach( Fragment fileFilter in requirementPartial.Fragments ) {
            requirement.SourceFilter.Add( ParseHelper.UnwrapString( fileFilter.Keyword ) );
          }
        }
      }

      foreach( Requirement requirement in requirements ) {
        // If the requirement root does not exist, throw
        if( !Directory.Exists( requirement.SourceFolder ) ) {
          throw new GeneratorException(
            string.Format( "The given requirement root '{0}' does not exist.", requirement.SourceFolder ),
            requirement.SourceFragment.SourceDocument );
        }

        // Find files in the requirement root
        DirectoryInfo sourceDirectory = new DirectoryInfo( requirement.SourceFolder );
        string currentPath = string.Empty;
        
        // Iterate over all defined filters in the requirement
        foreach( string sourceFilter in requirement.SourceFilter ) {
          DirectoryInfo searchRoot = sourceDirectory;
          currentPath = string.Empty;

          // Extract folder from filter and adjust filter expression.
          string filter = sourceFilter;
          if( sourceFilter.Contains( "/" ) ) {
            currentPath = sourceFilter.Substring( 0, sourceFilter.LastIndexOf( '/' ) + 1 );
            string activePath = Path.Combine( requirement.SourceFolder, currentPath );

            searchRoot = new DirectoryInfo( activePath );
            filter = sourceFilter.Substring( sourceFilter.LastIndexOf( '/' ) + 1 );
          }
          // Add all matched
          FileInfo[] files = searchRoot.GetFiles( filter );
          // Convert filenames to RequiredFile instances
          requirement.Files.AddRange(
            files.Select(
              s => new Requirement.RequiredFile {FullSourceName = s.FullName, RelativeTargetName = currentPath + s.Name} ) );
        }
        
      }

      return requirements;
    }
  }
}
