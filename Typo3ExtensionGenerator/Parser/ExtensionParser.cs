using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// Entry point for parsing operations
  /// </summary>
  public class ExtensionParser {
    public Extension Parse( string markup ) {
      // Remove whitespace
      markup = markup.Trim();

      // Translate the markup into an object tree
      Fragment fragment = FragmentParser.ParseFragment( markup );
      // Parse the object tree
      Extension result = Parse( fragment );

      // Do we have a valid title?
      if( string.IsNullOrEmpty( result.Title ) ) {
        result.Title = result.Key;
      }

      // Do we have a valid author?
      if( null == result.Author ) {
        result.Author = Person.Someone;
      }

      return result;
    }

    /// <summary>
    /// Parses a ParsedPartial that should contain an extension definition.
    /// </summary>
    /// <param name="fragment"></param>
    /// <returns></returns>
    private static Extension Parse( Fragment fragment ) {
      // The fragment MUST be an extension definition
      if( Keywords.DeclareExtension != fragment.Header.Substring( 0, Keywords.DeclareExtension.Length ) ) {
        throw new ParserException( "Missing extension declaration.", 1 );
      }

      Extension result = ExtensionResolver.Resolve( fragment );

      return result;
    }
  }
}
