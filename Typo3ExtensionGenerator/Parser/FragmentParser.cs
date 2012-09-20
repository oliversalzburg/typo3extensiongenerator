using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// The FragmentParser understands the TYPO3 Extension Generator syntax.
  /// It can parse the defined code style into an object tree that can later be translated.
  /// </summary>
  public class FragmentParser {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="element">The element that should be parsed.</param>
    /// <param name="lineNumber">From where to start counting line numbers.</param>
    /// <returns></returns>
    public static Fragment ParseFragment( string element, int lineNumber = 1 ) {
      // The place where we're currently at in the markup
      int characterPointer = 0;

      // The currently collected scope body
      string body = String.Empty;

      // Sub-scopes which we'll find during parsing will be stored in this partial.
      Fragment result = new Fragment() {
                                                   Body   = string.Empty,
                                                   Header = string.Empty,
                                                   Line   = lineNumber
                                                 };
          
      // How deeply nested we are into scopes.
      int scopeLevel  = 0;
      // Are we currently inside a string?
      bool inString = false;
      // Is the current character escaped?
      bool isEscaped = false;
      // Are we currently inside a comment?
      bool inComment = false;

      // Iterate over the whole input string
      // The whole point of this operation is to collect the full header of the partial,
      // as well as the full body of the partial.
      // If, while parsing the body of the partial, we find nested scopes, we'll store those to parse them later.
      while( characterPointer < element.Length ) {
        // Increase line number counter
        if( "\n" == element.Substring( characterPointer, "\n".Length ) ) {
          ++lineNumber;
        }

        // We only check for scopes while we're not parsing within a string.
        if( !inString ) {
          // Check for scope terminator
          if( Syntax.ScopeTerminate == element.Substring( characterPointer, Syntax.ScopeTerminate.Length ) ) {
            // Did we find a scope terminator? Like: ;
            if( 1 == scopeLevel ) {
              // As long as we're on the first scope level, we can collect the body of scopes to parse them later.
              // If we're deeper nested, there's no point, we'll parse those when we recurse.
              result.Fragments.Add( new Fragment {Body = body.Trim(), Line = lineNumber} );
              result.Body += element.Substring( characterPointer, 1 );
              // Clear buffer
              body = string.Empty;
              // Skip ahead
              ++characterPointer;
              continue;
            }
            if( 0 == scopeLevel ) {
              // If we're on the root level, just increase the scope pointer and skip.
              ++characterPointer;
              continue;
            }
          }

          // Check for scopes
          if( Syntax.ScopeStart == element.Substring( characterPointer, Syntax.ScopeStart.Length ) ) {
            // Did we find the start of a new scope?
            ++scopeLevel;

            if( 1 == scopeLevel ) {
              // If we're on the root level (we are, because we just increased the scopeLevel), we need to skip ahead.
              ++characterPointer;
              continue;
            }

          } else if( Syntax.ScopeEnd == element.Substring( characterPointer, Syntax.ScopeEnd.Length ) ) {
            --scopeLevel;
            // Did we find the end of a scope?
            if( 1 == scopeLevel ) {
              // Great! Another temporary scope we can store for later
              body += element.Substring( characterPointer, 1 );
              // Calculate the line number by extracting the number of newlines in the body from the line counter.
              int line = lineNumber - body.Trim().Count( c => c == '\n' );
              result.Fragments.Add(
                new Fragment {Body = body.Trim(), Line = line} );
              result.Body += element.Substring( characterPointer, 1 );
              // Clear buffer
              body = string.Empty;
              // Skip ahead
              ++characterPointer;
              continue;
            }
            if( 0 == scopeLevel ) {
              // If we're on the root level, just increase the scope pointer and skip.
              ++characterPointer;
              continue;
            }
          }

          // Check for string delimiter
          if( Syntax.StringDelimiter == element.Substring( characterPointer, Syntax.StringDelimiter.Length ) ) {
            // Did we hit a string delimiter? Like: "
            inString = true;
          }

          // Check for comment
          try {
            if( characterPointer + Syntax.CommentMultilineStart.Length <= element.Length && 
              Syntax.CommentMultilineStart == element.Substring( characterPointer, Syntax.CommentMultilineStart.Length ) ) {
              inComment = true;
              // Skip ahead until comment is terminated
              while( characterPointer < element.Length && inComment ) {
                ++characterPointer;
                if( Syntax.CommentMultilineEnd
                    == element.Substring( characterPointer, Syntax.CommentMultilineEnd.Length ) ) {
                  characterPointer += Syntax.CommentMultilineEnd.Length;
                  inComment = false;
                }
              }
            } else if( characterPointer + Syntax.CommentSinglelineStart.Length <= element.Length && 
              Syntax.CommentSinglelineStart == element.Substring( characterPointer, Syntax.CommentSinglelineStart.Length ) ) {
              inComment = true;
              // Skip ahead until comment is terminated
              // Single line comments are terminated by newline.
              while( characterPointer < element.Length && inComment ) {
                ++characterPointer;
                if( "\n" == element.Substring( characterPointer, "\n".Length ) ) {
                  inComment = false;
                  ++lineNumber;
                }
              }
            }
          } catch( ArgumentOutOfRangeException ) {
            throw new ParserException( "Hit end of input while looking for end of comment.", lineNumber );
          }

        } else {
          // This is when we're parsing within a string.
          if( Syntax.StringEscape == element.Substring( characterPointer, Syntax.StringEscape.Length ) ) {
            // Did we find an escape sequence? Like: \"
            isEscaped = true;
            ++characterPointer;
          }
          if( !isEscaped && Syntax.StringDelimiter == element.Substring( characterPointer, Syntax.StringDelimiter.Length ) ) {
            // Did the string end?
            inString = false;
          }
        }

        // Decide if we're currently parsing a header or a body and store accordingly.
        if( 0 == scopeLevel && characterPointer < element.Length ) {
          // Store in persitent result
          result.Header += element.Substring( characterPointer, 1 );
        } else {
          // Store in persitent result
          result.Body += element.Substring( characterPointer, 1 );
          // Also store in temporary buffer
          body += element.Substring( characterPointer, 1 );
        }

        isEscaped = false;
        ++characterPointer;
      }

      result.Header = result.Header.Trim();
      result.Body   = result.Body.Trim();

      // Recurse to resolve all previously parsed fragments
      foreach( Fragment childFragment in result.Fragments ) {
        Fragment fragment = ParseFragment( childFragment.Body, childFragment.Line );

        childFragment.Header    = fragment.Header;
        childFragment.Body      = fragment.Body;
        childFragment.Fragments = fragment.Fragments;

        // Pull keyword from header
        int delimiterPosition = childFragment.Header.IndexOf( ' ' );
        if( 0 > delimiterPosition ) delimiterPosition = childFragment.Header.Length;
        childFragment.Keyword = childFragment.Header.Substring( 0, delimiterPosition ).Trim();

        // The remaining part of the header would now be the parameters
        childFragment.Parameters = childFragment.Header.Substring( childFragment.Keyword.Length ).Trim();

        // Is the parameter set in ""?
        if( !string.IsNullOrEmpty( childFragment.Parameters ) && "\"" == childFragment.Parameters.Substring( 0, 1 ) ) {
          if( "\"" != childFragment.Parameters.Substring( childFragment.Parameters.Length -1, 1 ) ) {
            throw new ParserException( string.Format( "Unmatched \" in {0}", childFragment.Header ), lineNumber );
          }
          childFragment.Parameters = childFragment.Parameters.Substring( 1, childFragment.Parameters.Length - 2 );
        }
      }

      return result;
    }
  }
}
