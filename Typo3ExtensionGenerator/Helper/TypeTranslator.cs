using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Helper {
  /// <summary>
  /// Converts between different markup types.
  /// </summary>
  public static class TypeTranslator {
    /// <summary>
    /// Converts the given type description to a MySQL type.
    /// </summary>
    /// <param name="typeDescription"></param>
    /// <returns></returns>
    public static string ToSql( string typeDescription ) {
      switch( typeDescription ) {
        case "string":
          return "TEXT";
        case "uint":
          return "INT(11) UNSIGNED DEFAULT '0'";
        
        default:
          // Is this a char[123] type definition?
          if( typeDescription.Substring( 0, 4 ) == "char" ) {
            string length = typeDescription.Substring( 5, typeDescription.Length - 6 );
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
