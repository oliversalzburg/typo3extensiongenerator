using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class PaletteResolver {
    /// <summary>
    /// Resolves a palette definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The palette defintion.</returns>
    public static Palette Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial interfacePartial =
        parsedPartial.Partials.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfacePalette );
      if( null == interfacePartial ) return null;

      Palette parsedType = new Palette { Name = parsedPartial.Parameters, Interface = interfacePartial.Parameters };

      return parsedType;
    }
  }
}
