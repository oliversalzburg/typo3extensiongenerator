using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Helper {
  public static class ParseHelper {
    public static bool ParseBool( string input ) {
      string trimmed = input.Trim().ToLower();
      return !( "off" == trimmed ||
                "false" == trimmed ||
                "no" == trimmed ||
                "0" == trimmed );
    }

    /// <summary>
    /// Removes "" around a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string UnwrapString( string input ) {
      if( input.First() == '\"' && input.Last() == '\"' ) {
        return input.Substring( 1, input.Length - 2 );
      }
      return input;
    }
  }
}
