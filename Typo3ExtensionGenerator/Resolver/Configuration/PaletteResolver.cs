using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class PaletteResolver {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a palette definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The palette defintion.</returns>
    public static Palette Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      // Check if the palette defines an interface
      ExtensionParser.ParsedPartial interfacePartial =
        parsedPartial.Partials.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfacePalette );
      if( null == interfacePartial ) {
        throw new ParserException( string.Format( "Palette '{0}' does not define an interface.", parsedPartial.Parameters ), parsedPartial.Line );
      }
      
      Palette parsedType = new Palette {
                                         Name = parsedPartial.Parameters,
                                         Interface = interfacePartial.Parameters,
                                         SourceLine = interfacePartial.Line,
                                         SourcePartial = interfacePartial
                                       };

      return parsedType;
    }
  }
}
