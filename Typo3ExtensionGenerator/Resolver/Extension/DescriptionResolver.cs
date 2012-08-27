using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public static class DescriptionResolver {
    /// <summary>
    /// Resolves the description of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The description of the extension</returns>
    public static string Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial descriptionPartial = parsedPartial.Partials.FirstOrDefault( p => p.Keyword == "description" );
      if( null == descriptionPartial ) return null;

      return descriptionPartial.Parameters;
    }
  }
}
