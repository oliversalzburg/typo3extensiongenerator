using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using log4net;

namespace Typo3ExtensionGenerator.PreProcess {
  /// <summary>
  /// Resolves #include statements in the given markup.
  /// </summary>
  public static class ResolveIncludes {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public static string Resolve( string markup ) {
      string[] codeLines = Regex.Split( markup, "\r\n|\r|\n" );
      foreach( string codeLine in codeLines ) {
        if( codeLine.Trim().StartsWith( "#include" ) ) {
          Log.Debug( "Found #include statement." );
        }
      }
      return markup;
    }
  }
}
