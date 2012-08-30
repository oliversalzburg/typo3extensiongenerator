using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Generator.PrettyPrint {
  /// <summary>
  /// The ludicrous pretty printer has no clue what it's doing. It does not really understand the input and assumes a LOT!
  /// It is intended for debugging purposes to make the output more readable.
  /// </summary>
  public static class LudicrousPrettyPrinter {
    [Obsolete( "Use at your own risk!" )]
    public static string PrettyPrint( string code ) {
      
      StringBuilder output = new StringBuilder();
      
      char[] ignored = new char[] {
                                    '\n','\r','\t'
                                  };

      bool inString = false;
      bool wasSpace = false;

      int scopeDepth = 0;

      int characterPointer = 0;
      while( characterPointer < code.Length ) {
        char currentCharacter = code.ElementAt( characterPointer );
        if( !inString ) {
          // We are not parsing within a string

          // Replaced ignored characters with a space.
          if( ignored.Contains( currentCharacter ) ) {
            if( !wasSpace ) {
              output.Append( ' ' );
            }
            wasSpace = true;

          } else {
            // This character should not be ignored

            // Is it a string delimiter?
            if( '"' == currentCharacter || '\'' == currentCharacter ) {
              // We're now remembering that we'll be parsing in a string
              inString = true;
              output.Append( currentCharacter );

            // Is this a space character?
            } else if ( ' ' == currentCharacter ) {
              // Did we already print a space?
              if( !wasSpace ) {
                // No? Great, let's print one and remember not to print another one until we hit a non-space in the input again.
                wasSpace = true;
                output.Append( ' ' );
              }

            // Is it an opening scope?
            } else if ( '{' == currentCharacter ) {
              output.Append( "{\n" );
              ++scopeDepth;
              output.Append( new string( ' ', scopeDepth * 2 ) );
              wasSpace = true;

            // Is it a closing scope?
            } else if ( '}' == currentCharacter ) {
              --scopeDepth;
              output.Append( new string( ' ', scopeDepth * 2 ) );
              output.Append( "}" );
              

              // Is it a comma?
            } else if ( ',' == currentCharacter ) {
              output.Append( ",\n" );
              output.Append( new string( ' ', scopeDepth * 2 ) );
              wasSpace = true;

            // Is it the end of a statement?
            } else if ( ';' == currentCharacter ) {
              output.Append( ";\n" );
              output.Append( new string( ' ', scopeDepth * 2 ) );
              wasSpace = true;

            // Is it possibly a PHP array?
            } else if ( '(' == currentCharacter /*&& "array(" == code.Substring( characterPointer - "array(".Length + 1, "array(".Length ) */) {
              output.Append( "(\n" );
              ++scopeDepth;
              output.Append( new string( ' ', scopeDepth * 2 ) );
              wasSpace = true;

            } else if ( ')' == currentCharacter ) {
              output.Append( "\n" );
              --scopeDepth;
              output.Append( new string( ' ', scopeDepth * 2 ) );
              output.Append( ")" );
              
              // Is it any other character?
            } else {
              // Remember that this was not a space character
              wasSpace = false;
              output.Append( currentCharacter );
            }
            
          }
        } else {
          // We are parsing within a string

          // If the current character is the string delimiter, we're no longer in a string
          if( '"' == currentCharacter || '\'' == currentCharacter ) {
            inString = false;
          }
          output.Append( currentCharacter );
        }
        ++characterPointer;
      }

      return output.ToString();
    }
  }
}
