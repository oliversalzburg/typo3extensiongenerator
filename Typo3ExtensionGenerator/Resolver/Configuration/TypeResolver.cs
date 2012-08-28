using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class TypeResolver {
    /// <summary>
    /// Resolves a type definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The defined type.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Type Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial interfacePartial =
        parsedPartial.Partials.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfaceType );
      if( null == interfacePartial ) return null;

      Typo3ExtensionGenerator.Model.Configuration.Type parsedType =
        new Typo3ExtensionGenerator.Model.Configuration.Type {Interface = interfacePartial.Parameters};

      return parsedType;
    }
  }
}
