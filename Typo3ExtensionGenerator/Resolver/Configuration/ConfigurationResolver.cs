﻿using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class ConfigurationResolver {
    /// <summary>
    /// Resolves the configurations of data models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The configurations of the extension</returns>
    public static List<Typo3ExtensionGenerator.Model.Configuration> Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      IEnumerable<ExtensionParser.ParsedPartial> configurationPartials = parsedPartial.Partials.Where( p => p.Keyword == Keywords.DeclareConfiguration );
      if( !configurationPartials.Any() ) return null;

      List<Typo3ExtensionGenerator.Model.Configuration> configurations = new List<Typo3ExtensionGenerator.Model.Configuration>();
      foreach( ExtensionParser.ParsedPartial configurationPartial in configurationPartials ) {
        Typo3ExtensionGenerator.Model.Configuration configuration = new Typo3ExtensionGenerator.Model.Configuration { Target = configurationPartial.Parameters };
        configurations.Add( configuration );
        if( configurationPartial.Partials.Any() ) {
          foreach( ExtensionParser.ParsedPartial configurationDirective in configurationPartial.Partials ) {
            
            // Find configuration directives
            if( Keywords.ConfigurationDirectives.Label == configurationDirective.Keyword ) {
              configuration.Label = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.LabelAlternative == configurationDirective.Keyword ) {
              configuration.LabelAlternative = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.LabelHook == configurationDirective.Keyword ) {
              configuration.LabelHook = "TODO to be generated!";

            } else if( Keywords.ConfigurationDirectives.SearchFields == configurationDirective.Keyword ) {
              configuration.SearchFields = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.Thumbnail == configurationDirective.Keyword ) {
              configuration.Thumbnail = configurationDirective.Parameters;

            } else if( Keywords.ConfigurationDirectives.InterfaceInfo == configurationDirective.Keyword ) {
              configuration.InterfaceInfo = configurationDirective.Parameters;
            }

          }
        }
      }

      return configurations;
    }
  }
}
