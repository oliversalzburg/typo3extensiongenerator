﻿using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resolver.Configuration.Interface;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class InterfaceResolver {
    /// <summary>
    /// Resolves the model field interfaces of a data model configurationfrom a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The field interfaces of the data model configuration.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Interface Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      Typo3ExtensionGenerator.Model.Configuration.Interface @interface = new Typo3ExtensionGenerator.Model.Configuration.Interface {Target = parsedPartial.Parameters};
      if( parsedPartial.Partials.Any() ) {
        foreach( ExtensionParser.ParsedPartial setting in parsedPartial.Partials ) {
          @interface.Settings.Add( new KeyValuePair<string, string>( setting.Keyword, setting.Parameters ) );
        }
      }

      // Parse any settings we know
      foreach( KeyValuePair<string, string> setting in @interface.Settings ) {
        if( Keywords.ConfigurationDirectives.InterfaceDirectives.Exclude == setting.Key ) {
          @interface.Exclude = ParseHelper.ParseBool( setting.Value );
        }
        if( Keywords.ConfigurationDirectives.InterfaceDirectives.Title == setting.Key ) {
          @interface.Title = setting.Value;
        }
        if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representation == setting.Key ) {
          @interface.DisplayType = setting.Value;
          //DisplayTypeResolver.Resolve( parsedPartial, @interface, setting.Value );
        }
      }

      return @interface;
    }
  }
}