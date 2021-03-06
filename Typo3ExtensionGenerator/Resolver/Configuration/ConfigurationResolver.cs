﻿using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Configuration.Interface;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  /// <summary>
  /// Resolves all configurations from markup.
  /// </summary>
  public static class ConfigurationResolver {
    /// <summary>
    /// Resolves the configurations of data models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The configurations of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Configuration.Configuration> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> configurationPartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareConfiguration );
      if( !configurationPartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Configuration.Configuration> configurations = new List<Typo3ExtensionGenerator.Model.Configuration.Configuration>();
      foreach( Fragment configurationPartial in configurationPartials ) {
        Typo3ExtensionGenerator.Model.Configuration.Configuration configuration = new Typo3ExtensionGenerator.Model.Configuration.Configuration { Target = configurationPartial.Parameters };
        configurations.Add( configuration );
        if( configurationPartial.Fragments.Any() ) {
          foreach( Fragment configurationDirective in configurationPartial.Fragments ) {
            
            // Find configuration directives
            if( Keywords.ConfigurationDirectives.Label == configurationDirective.Keyword ) {
              configuration.Label = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.LabelAlternative == configurationDirective.Keyword ) {
              configuration.LabelAlternative = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.LabelHook == configurationDirective.Keyword ) {
              configuration.LabelHook = true;

            } else if( Keywords.ConfigurationDirectives.SearchFields == configurationDirective.Keyword ) {
              configuration.SearchFields = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.Thumbnail == configurationDirective.Keyword ) {
              configuration.Thumbnail = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.InterfaceInfo == configurationDirective.Keyword ) {
              configuration.InterfaceInfo = configurationDirective.Parameters;
            
            } else if( Keywords.ConfigurationDirectives.TypeDeclaration == configurationDirective.Keyword ) {
              configuration.Types.Add( TypeResolver.Resolve( configurationDirective ) );

            } else if( Keywords.ConfigurationDirectives.PaletteDeclaration == configurationDirective.Keyword ) {
              configuration.Palettes.Add( PaletteResolver.Resolve( configurationDirective ) );

            } else if( Keywords.DefineInterface == configurationDirective.Keyword ) {
              Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface = InterfaceResolver.Resolve( configurationDirective );
              @interface.ParentModelTarget = configuration.Target;
              configuration.Interfaces.Add( @interface );

            } else if( Keywords.Title == configurationDirective.Keyword ) {
              configuration.Title = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.Visibility.Hidden == configurationDirective.Keyword ) {
              configuration.Hidden = true;

            }
          }
        }
      }

      return configurations;
    }
  }
}
