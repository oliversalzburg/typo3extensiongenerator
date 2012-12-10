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
  public static class ExtensionParser {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Parse extension markup into an extension model.
    /// </summary>
    /// <param name="markup">The markup that describes the extension.</param>
    /// <param name="sourceFileName">The filename in which the markup was/is contained</param>
    /// <returns></returns>
    public static Extension Parse( string markup, string sourceFileName ) {
      Log.InfoFormat( "Constructing virtual document from markup..." );
      VirtualDocument document = VirtualDocument.FromText( markup, sourceFileName );

      Log.Info( "Pre-processing virtual document..." );
      document = ResolveIncludes.Resolve( document );

      Log.Info( "Translating document to fragment tree..." );
      // Translate the markup into an object tree
      Fragment fragment = FragmentParser.ParseFragment( document );

      Log.Info( "Translating fragment tree to TYPO3 extension..." );
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
    /// <exception cref="ParserException">Missing extension declaration.</exception>
    private static Extension Parse( Fragment fragment ) {
      // The fragment MUST be an extension definition
      if( fragment.Header.Length < Keywords.DeclareExtension.Length || Keywords.DeclareExtension != fragment.Header.Substring( 0, Keywords.DeclareExtension.Length ) ) {
        throw new ParserException( "Missing extension declaration.", fragment.SourceDocument );
      }

      Extension result = ExtensionResolver.Resolve( fragment );
      result.SourceFragment = fragment;

      return result;
    }
  }
}
