using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Resolver;
using Typo3ExtensionGenerator.Resolver.Configuration;
using Typo3ExtensionGenerator.Resolver.Extension;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Module;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// Entry point for parsing operations
  /// </summary>
  public class ExtensionParser {

    /// <summary>
    /// A partially parsed markup element. This is the core element of the grammar.
    /// Elements always consist of a header and an optional body.
    /// The basic syntax is:
    /// keyword parameters {
    ///   body
    /// }
    /// 
    /// keyword + parameters = header
    /// </summary>
    public class ParsedPartial {
      /// <summary>
      /// The header part of the partial.
      /// </summary>
      /// <remarks>
      /// The header is always the full part that stood before the body (the body being indicated by a scope begin).
      /// </remarks>
      public string Header { get; set; }

      /// <summary>
      /// The keyword that identifies this partial.
      /// </summary>
      public string Keyword { get; set; }

      /// <summary>
      /// The body part of the partial.
      /// </summary>
      /// <remarks>
      /// The body is always the full part that stood after the header (the body being indicated by a scope begin).
      /// </remarks>
      public string Body { get; set; }

      /// <summary>
      /// The part that stood after the keyword.
      /// </summary>
      public string Parameters { get; set; }

      /// <summary>
      /// A list of parsed child elements
      /// </summary>
      public List<ParsedPartial> Partials { get; set; }

      /// <summary>
      /// The line on which this partial was defined.
      /// </summary>
      public int Line { get; set; }

      public ParsedPartial() {
        Partials = new List<ParsedPartial>();
      }

      public override string ToString() {
        return string.Format( "{0} ( {1} )", Keyword, Parameters );
      }
    }

    public Extension Parse( string markup ) {
      // Remove whitespace
      markup = markup.Trim();

      ParsedPartial parsedPartial = ParsePartial( markup );

      Extension result = Parse( parsedPartial );

      // Do we have a valid title?
      if( string.IsNullOrEmpty( result.Title ) ) {
        result.Title = result.Key;
      }

      // Do we have a valid author?
      if( null == result.Author ) {
        result.Author = Person.Someone;
      }

      return result;
    }

    /// <summary>
    /// Parses a ParsedPartial that should contain an extension definition.
    /// </summary>
    /// <param name="partial"></param>
    /// <returns></returns>
    public Extension Parse( ParsedPartial partial ) {
      // The partial MUST be an extension definition
      if( Keywords.DeclareExtension != partial.Header.Substring( 0, Keywords.DeclareExtension.Length ) ) {
        throw new ParserException( "Missing extension declaration.", 1 );
      }

      Extension result = ExtensionResolver.Resolve( partial );

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element">The element that should be parsed.</param>
    /// <param name="lineNumber">From where to start counting line numbers.</param>
    /// <returns></returns>
    public ParsedPartial ParsePartial( string element, int lineNumber = 1 ) {
      // The place where we're currently at in the markup
      int characterPointer = 0;

      // The currently collected scope body
      string body = String.Empty;

      // Sub-scopes which we'll find during parsing will be stored in this partial.
      ParsedPartial result = new ParsedPartial() {
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
              result.Partials.Add( new ParsedPartial {Body = body.Trim(), Line = lineNumber} );
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
              result.Partials.Add(
                new ParsedPartial {Body = body.Trim(), Line = line} );
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

      // Parse all the partials
      foreach( ParsedPartial parsedPartial in result.Partials ) {
        ParsedPartial partial = ParsePartial( parsedPartial.Body, parsedPartial.Line );

        parsedPartial.Header   = partial.Header;
        parsedPartial.Body     = partial.Body;
        parsedPartial.Partials = partial.Partials;

        // Pull keyword from header
        int delimiterPosition = parsedPartial.Header.IndexOf( ' ' );
        if( 0 > delimiterPosition ) delimiterPosition = parsedPartial.Header.Length;
        parsedPartial.Keyword = parsedPartial.Header.Substring( 0, delimiterPosition ).Trim();

        // The remaining part of the header would now be the parameters
        parsedPartial.Parameters = parsedPartial.Header.Substring( parsedPartial.Keyword.Length ).Trim();

        // Is the parameter set in ""?
        if( !string.IsNullOrEmpty( parsedPartial.Parameters ) && "\"" == parsedPartial.Parameters.Substring( 0, 1 ) ) {
          if( "\"" != parsedPartial.Parameters.Substring( parsedPartial.Parameters.Length -1, 1 ) ) {
            throw new ParserException( string.Format( "Unmatched \" in {0}", parsedPartial.Header ), lineNumber );
          }
          parsedPartial.Parameters = parsedPartial.Parameters.Substring( 1, parsedPartial.Parameters.Length - 2 );
        }
      }

      return result;
    }
  }
}
