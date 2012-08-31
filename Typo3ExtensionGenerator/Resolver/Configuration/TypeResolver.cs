using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class TypeResolver {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a type definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The defined type.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Type Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      // Check if the type defines an interface
      ExtensionParser.ParsedPartial interfacePartial =
        parsedPartial.Partials.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfaceType );
      if( null == interfacePartial ) {
        throw new ParserException( string.Format( "Type '{0}' does not define an interface.", parsedPartial.Parameters ), parsedPartial.Line );
      }

      Typo3ExtensionGenerator.Model.Configuration.Type parsedType =
        new Typo3ExtensionGenerator.Model.Configuration.Type
        {Interface = interfacePartial.Parameters, SourceLine = parsedPartial.Line, SourcePartial = interfacePartial};

      return parsedType;
    }
  }
}
