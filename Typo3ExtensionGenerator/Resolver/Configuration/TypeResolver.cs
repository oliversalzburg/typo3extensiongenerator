using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class TypeResolver {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a type definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The defined type.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Type Resolve( Fragment parsedFragment ) {
      // Check if the type defines an interface
      Fragment interfacePartial =
        parsedFragment.Fragments.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfaceType );
      if( null == interfacePartial ) {
        throw new ParserException( string.Format( "Type '{0}' does not define an interface.", parsedFragment.Parameters ), parsedFragment.Line );
      }

      Typo3ExtensionGenerator.Model.Configuration.Type parsedType =
        new Typo3ExtensionGenerator.Model.Configuration.Type
        {Interface = interfacePartial.Parameters, SourceLine = parsedFragment.Line, SourceFragment = interfacePartial};

      return parsedType;
    }
  }
}
