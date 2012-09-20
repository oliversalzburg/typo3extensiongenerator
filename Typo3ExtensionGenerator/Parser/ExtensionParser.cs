using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Parser.Document;
using Typo3ExtensionGenerator.PreProcess;
using Typo3ExtensionGenerator.Resolver;
using log4net;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// Entry point for parsing operations
  /// </summary>
  public class ExtensionParser {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public Extension Parse( string markup ) {
      // Remove whitespace
      //markup = markup.Trim();

      VirtualDocument document = VirtualDocument.FromText( markup );

      Log.Info( "Pre-processing..." );
      document = ResolveIncludes.Resolve( document );

      // Translate the markup into an object tree
      Fragment fragment = FragmentParser.ParseFragment( document );
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
        throw new ParserException( "Missing extension declaration.", fragment.SourceDocument );
      }

      Extension result = ExtensionResolver.Resolve( fragment );

      return result;
    }
  }
}
