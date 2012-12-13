using System.Linq;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  /// <summary>
  /// Used to resolve a Type from markup.
  /// A Type is an interface description inside a configuration.
  /// A configuration could provide several types. These could have different interface definitions to show or hide different data model members.
  /// In practice, there's usually only one, unnamed type.
  /// </summary>
  public static class TypeResolver {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a type definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The defined type.</returns>
    /// <exception cref="ParserException">Type does not define an interface.</exception>
    public static Typo3ExtensionGenerator.Model.Configuration.Type Resolve( Fragment parsedFragment ) {
      // Check if the type defines an interface
      Fragment interfacePartial = parsedFragment.Fragments.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfaceType );
      if( null == interfacePartial ) {
        string typeName = parsedFragment.Parameters;
        if( string.IsNullOrEmpty( typeName ) ) {
          typeName = "<unnamed>";
        }
        throw new ParserException( string.Format( "Type '{0}' does not define an interface.", typeName ), parsedFragment.SourceDocument );
      }

      Typo3ExtensionGenerator.Model.Configuration.Type parsedType = new Typo3ExtensionGenerator.Model.Configuration.Type {Interface = interfacePartial.Parameters, SourceFragment = interfacePartial};

      return parsedType;
    }
  }
}
