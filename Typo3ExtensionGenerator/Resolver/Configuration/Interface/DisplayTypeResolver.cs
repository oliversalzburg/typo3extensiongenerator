using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration.Interface {
  public static class DisplayTypeResolver {
    public static void Resolve( ExtensionParser.ParsedPartial parsedPartial, Typo3ExtensionGenerator.Model.Configuration.Interface @interface, string displayType ) {
      if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.Dropdown == displayType ) {
        ExtensionParser.ParsedPartial representation =
          parsedPartial.Partials.Where(
            p => Keywords.ConfigurationDirectives.InterfaceDirectives.Representation == p.Keyword ).SingleOrDefault();

        if( null != representation ) {
          ExtensionParser.ParsedPartial foreignModel =
            representation.Partials.SingleOrDefault(
              p => p.Keyword == Keywords.ConfigurationDirectives.InterfaceDirectives.Foreign );
        }
      }
    }
  }
}
