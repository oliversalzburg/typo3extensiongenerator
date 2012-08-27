using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  public class AuthorResolver {
    /// <summary>
    /// Resolves the name of the author of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The name of the author</returns>
    public static string ResolveAuthor( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial authorPartial = parsedPartial.Partials.FirstOrDefault( p => p.Keyword == Keywords.DefineAuthor );
      if( null == authorPartial ) return null;

      return authorPartial.Parameters;
    }

    /// <summary>
    /// Resolves the email address of the author of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The email address of the author</returns>
    public static string ResolveEmail( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial authorEmailPartial = parsedPartial.Partials.FirstOrDefault( p => p.Keyword == Keywords.DefineAuthorEmail );
      if( null == authorEmailPartial ) return null;

      return authorEmailPartial.Parameters;
    }

    /// <summary>
    /// Resolves the company name of the author of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The company name of the author</returns>
    public static string ResolveCompany( ExtensionParser.ParsedPartial parsedPartial ) {
      ExtensionParser.ParsedPartial companyPartial = parsedPartial.Partials.FirstOrDefault( p => p.Keyword == Keywords.DefineAuthorCompany );
      if( null == companyPartial ) return null;

      return companyPartial.Parameters;
    }
  }
}
