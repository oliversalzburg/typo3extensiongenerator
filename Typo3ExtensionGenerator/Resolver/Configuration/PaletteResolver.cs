using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  public static class PaletteResolver {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a palette definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The palette defintion.</returns>
    public static Palette Resolve( Fragment parsedFragment ) {
      // Check if the palette defines an interface
      Fragment interfacePartial =
        parsedFragment.Fragments.SingleOrDefault( p => p.Keyword == Keywords.ConfigurationDirectives.InterfacePalette );
      if( null == interfacePartial ) {
        throw new ParserException( string.Format( "Palette '{0}' does not define an interface.", parsedFragment.Parameters ), parsedFragment.SourceDocument );
      }
      
      Palette parsedType = new Palette {
                                         Name = parsedFragment.Parameters,
                                         Interface = interfacePartial.Parameters,
                                         SourceFragment = interfacePartial
                                       };

      return parsedType;
    }
  }
}
