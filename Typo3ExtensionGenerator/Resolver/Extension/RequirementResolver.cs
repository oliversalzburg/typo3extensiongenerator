﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Model;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  /// <summary>
  /// Resolves all requirements defined in an extension.
  /// A requirement is an external file that the extension depends upon.
  /// </summary>
  public static class RequirementResolver {
    
    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    /// <exception cref="GeneratorException">The given requirement root does not exist.</exception>
    public static List<Requirement> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> requirementFragments = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.Requirement );
      if( !requirementFragments.Any() ) return null;

      List<Requirement> requirements = new List<Requirement>();
      foreach( Fragment requirementFragment in requirementFragments ) {
        Requirement requirement = new Requirement {
                                                    SourceFolder   = requirementFragment.Parameters,
                                                    SourceFragment = requirementFragment
                                                  };
        requirements.Add( requirement );

        if( requirementFragment.Fragments.Any() ) {
          foreach( Fragment fileFilter in requirementFragment.Fragments ) {
            requirement.SourceFilter.Add( ParseHelper.UnwrapString( fileFilter.Keyword ) );
          }
        }
      }

      foreach( Requirement requirement in requirements ) {
        // If the requirement root does not exist, throw
        if( !Directory.Exists( requirement.SourceFolder ) ) {
          throw new GeneratorException( string.Format( "The given requirement root '{0}' does not exist.", requirement.SourceFolder ), requirement.SourceFragment.SourceDocument );
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
          try {
            FileInfo[] files = searchRoot.GetFiles( filter );
            // Convert filenames to RequiredFile instances
            requirement.Files.AddRange(
              files.Select(
                s => new Requirement.RequiredFile {FullSourceName = s.FullName, RelativeTargetName = currentPath + s.Name} ) );

          } catch( DirectoryNotFoundException e ) {
            throw new ParserException( e.Message, parsedFragment.SourceDocument );
          }
        }
        
      }

      return requirements;
    }
  }
}
