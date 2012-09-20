using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Parser.Document;
using log4net;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// The FragmentParser understands the TYPO3 Extension Generator syntax.
  /// It can parse the defined code style into an object tree that can later be translated.
  /// </summary>
  public static class FragmentParser {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Parses a virtual document into a parsed fragment.
    /// </summary>
    /// <param name="document">The virtual document that should be parsed.</param>
    /// <returns></returns>
    public static Fragment ParseFragment( VirtualDocument document ) {
      DocumentWalker walker = new DocumentWalker( document );

      // The currently collected scope body
      string body = String.Empty;

      // Sub-scopes which we'll find during parsing will be stored in this partial.
      Fragment result = new Fragment {
                                       Body   = string.Empty,
                                       Header = string.Empty
                                     };
          
      // How deeply nested we are into scopes.
      int scopeLevel  = 0;
      // Where the scope we're currently recording started.
      VirtualDocument.Character scopeStart = walker.CurrentCharacter;
      // Are we currently inside a string?
      bool inString = false;
      // Is the current character escaped?
      bool isEscaped = false;
      // Are we currently inside a comment?
      bool inComment = false;
      // The line where a comment was started
      int commentStart = 0;

      // Iterate over the whole input string
      // The whole point of this operation is to collect the full header of the partial,
      // as well as the full body of the partial.
      // If, while parsing the body of the partial, we find nested scopes, we'll store those to parse them later.
      while( walker.CanWalk ) {
        if( !inString ) {
          // Check for scope terminator
          if( walker.CurrentlyReads( Syntax.ScopeTerminate ) ) {
            // Did we find a scope terminator? Like: ;
            if( 1 == scopeLevel ) {
              // As long as we're on the first scope level, we can collect the body of scopes to parse them later.
              // If we're deeper nested, there's no point, we'll parse those when we recurse.

              // Construct a new document for the currently recorded scope.
              VirtualDocument documentFragment = VirtualDocument.FromDocument( document, scopeStart, walker.CurrentCharacter );
              // ...and store it.
              result.Fragments.Add( new Fragment {Body = body.Trim(), SourceDocument = documentFragment} );
              result.Body += walker.CurrentCharacter;

              // Clear buffer
              body = string.Empty;
              
              // We skip ahead until we see a character again. We need those as markers.
              try {
                walker.WalkToNext();
              } catch( ArgumentOutOfRangeException ) {
                // Things can always go wrong when running!
                break;
              }
              // Set the current location as the new recording start point
              scopeStart = walker.CurrentCharacter;

              continue;
            }
            if( 0 == scopeLevel ) {
              // If we're on the root level, just increase the scope pointer and skip.
              walker.Walk();
              continue;
            }
          }

          // Check for scopes
          if( walker.CurrentlyReads( Syntax.ScopeStart ) ) {
            // Did we find the start of a new scope?
            ++scopeLevel;

            if( 1 == scopeLevel ) {
              // If we're on the root level (we are, because we just increased the scopeLevel), we need to skip ahead.
              walker.Walk();
              scopeStart = walker.CurrentCharacter;
              continue;
            }
            if( 2 == scopeLevel ) {
              //scopeStart = walker.CurrentCharacter;
            }

          } else if( walker.CurrentlyReads( Syntax.ScopeEnd ) ) {
            --scopeLevel;
            // Did we find the end of a scope?
            if( 1 == scopeLevel ) {
              // Great! Another temporary scope we can store for later
              body += walker.CurrentCharacter;
              result.Fragments.Add(
                new Fragment {
                               Body = body.Trim(),
                               SourceDocument =
                                 VirtualDocument.FromDocument( document, scopeStart, walker.CurrentCharacter )
                             } );
              result.Body += walker.CurrentCharacter;
              // Clear buffer
              body = string.Empty;
              
              // We skip ahead until we see a character again. We need those as markers.
              try {
                walker.WalkToNext();
              } catch( ArgumentOutOfRangeException ) {
                // Things can always go wrong when running!
                break;
              }
              // Set the current location as the new recording start point
              scopeStart = walker.CurrentCharacter;

              continue;
            }
            if( 0 == scopeLevel ) {
              // If we're on the root level, just increase the scope pointer and skip.
              try {
                walker.Walk();
              } catch( ArgumentOutOfRangeException ){}

              continue;
            }
          }

          // Check for string delimiter
          if( walker.CurrentlyReads( Syntax.StringDelimiter ) ) {
            // Did we hit a string delimiter? Like: "
            inString = true;
          }

          // Check for comment
          try {
            //if( characterPointer + Syntax.CommentMultilineStart.Length <= element.Length && Syntax.CommentMultilineStart == element.Substring( characterPointer, Syntax.CommentMultilineStart.Length ) ) {
            if( walker.CurrentlyReads( Syntax.CommentMultilineStart ) ) {
              
              inComment    = true;
              commentStart = walker.CurrentLine.PhysicalLineIndex;
              // Skip ahead until comment is terminated
              while( walker.CanWalk && inComment ) {
                walker.Walk();
                if( walker.CurrentlyReads( Syntax.CommentMultilineEnd ) ) {
                  walker.Walk( Syntax.CommentMultilineEnd.Length );
                  
                  inComment    = false;
                  commentStart = 0;
                }

              }
            } else if( walker.CurrentlyReads( Syntax.CommentSinglelineStart ) ) {
              // Skip ahead until comment is terminated
              // Single line comments are terminated by newline.
              while( walker.CanWalkForward ) {
                walker.Walk();
              }
              // We walked to the end of the line, no we skip to the next one
              walker.Walk();
              commentStart = 0;
                
            }
          } catch( ArgumentOutOfRangeException ex ) {
            throw new ParserException( string.Format( "Hit end of input while looking for end of comment which started on line {0}.", commentStart ), walker.Document );
          }

        } else {

          // This is when we're parsing within a string.

          if( walker.CurrentlyReads( Syntax.StringEscape ) ) {
            // Did we find an escape sequence? Like: \"
            isEscaped = true;
            walker.Walk();
          }
          if( !isEscaped && walker.CurrentlyReads( Syntax.StringDelimiter ) ) {
            // Did the string end?
            inString = false;
          }
        }

        // Decide if we're currently parsing a header or a body and store accordingly.
        if( 0 == scopeLevel && walker.CanWalk ) {
          // Store in persitent result
          result.Header += walker.CurrentCharacter;
        } else {
          // Store in persistent result
          result.Body += walker.CurrentCharacter;
          // Also store in temporary buffer
          body += walker.CurrentCharacter;
        }

        isEscaped = false;
        Debug.Assert( walker.CanWalk, "Tried to walk when it's not possible" );
        walker.Walk();
      }

      result.Header = result.Header.Trim();
      result.Body   = result.Body.Trim();

      // Recurse to resolve all previously parsed fragments
      foreach( Fragment childFragment in result.Fragments ) {
        Fragment fragment = ParseFragment( childFragment.SourceDocument );

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
            throw new ParserException( string.Format( "Unmatched \" in {0}", childFragment.Header ), walker.Document );
          }
          childFragment.Parameters = childFragment.Parameters.Substring( 1, childFragment.Parameters.Length - 2 );
        }
      }

      return result;
    }
  }
}
