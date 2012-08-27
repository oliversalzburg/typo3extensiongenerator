using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public class TitleResolver {
    /// <summary>
    /// Resolves the title of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The title of the extension</returns>
    public static string Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial titlePartial = parsedPartial.Partials.FirstOrDefault( p => p.Keyword == "title" );
      if( null == titlePartial ) return null;

      return titlePartial.Parameters;
    }
  }
}
