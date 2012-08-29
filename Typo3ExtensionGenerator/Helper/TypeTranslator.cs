using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Helper {
  /// <summary>
  /// Converts between different markup types.
  /// </summary>
  public static class TypeTranslator {
    public static bool CanTranslate( string typeDescription ) {
      return new Regex( string.Format( "^({0}|{1}|{2})", Keywords.Types.CharacterArray, Keywords.Types.String, Keywords.Types.UnsignedInt ) ).IsMatch( typeDescription );
    }
    /// <summary>
    /// Converts the given type description to a MySQL type.
    /// </summary>
    /// <param name="typeDescription"></param>
    /// <returns></returns>
    public static string ToSql( string typeDescription ) {
      switch( typeDescription ) {
        case Keywords.Types.String:
          return "TEXT";
        case Keywords.Types.UnsignedInt:
          return "INT(11) UNSIGNED DEFAULT '0'";
        
        default:
          // Is this a char[123] type definition?
          if( typeDescription.Substring( 0, Keywords.Types.CharacterArray.Length ) == Keywords.Types.CharacterArray ) {
            // Extract size of character array
            string length = typeDescription.Substring(
              Keywords.Types.CharacterArray.Length + 1,
              typeDescription.Length - ( Keywords.Types.CharacterArray.Length + 2 ) );

            int memberLength = 0;
            if( !int.TryParse( length, out memberLength ) ) {
              throw new ParserException( string.Format( "Unable to translate type character '{0}'.", typeDescription ) );
            }
            return string.Format( "VARCHAR({0}) DEFAULT '' NOT NULL", memberLength );

          } else {
            throw new ParserException( string.Format( "Unable to translate type '{0}'.", typeDescription ) );
          }
          
      }
    }
  }
}
