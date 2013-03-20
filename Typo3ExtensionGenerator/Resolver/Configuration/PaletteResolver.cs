using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Configuration.Interface;
using log4net;

namespace Typo3ExtensionGenerator.Resolver.Configuration {
  /// <summary>
  /// Resolves a palette from markup.
  /// </summary>
  public static class PaletteResolver {
    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves a palette definition of data model configuration of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The palette definition.</returns>
    /// <exception cref="ParserException">Palette does not define an interface.</exception>
    public static Palette Resolve( Fragment parsedFragment ) {
      // Check if the palette defines an interface
      Fragment interfacePartial = parsedFragment.Fragments.SingleOrDefault( p => p.Keyword == Keywords.DefineInterface );
      if( null == interfacePartial ) {
        throw new ParserException( string.Format( "Palette '{0}' does not define an interface.", parsedFragment.Parameters ), parsedFragment.SourceDocument );
      }

      Palette parsedPalette = new Palette {
        Name           = parsedFragment.Parameters,
        Interface      = interfacePartial.Parameters,
        Visibility     = Palette.PaletteVisibility.Default,
        SourceFragment = interfacePartial
      };

      if( parsedFragment.Fragments.Any() ) {
        foreach( Fragment configurationDirective in parsedFragment.Fragments ) {
          // Find configuration directives
          if( Keywords.ConfigurationDirectives.Visibility.Hidden == configurationDirective.Keyword ) {
            parsedPalette.Visibility = Palette.PaletteVisibility.ShowNever;

          } else if( Keywords.ConfigurationDirectives.Visibility.Show == configurationDirective.Keyword ) {
            if( Keywords.ConfigurationDirectives.Visibility.Always == configurationDirective.Parameters ) {
              parsedPalette.Visibility = Palette.PaletteVisibility.ShowAlways;
            }
          }
        }
      }

      return parsedPalette;
    }
  }
}