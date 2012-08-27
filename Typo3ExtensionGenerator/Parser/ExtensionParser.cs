using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;
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
    /// A partially parsed markup element
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

      string extensionKey = partial.Header.Substring(
        Keywords.DeclareExtension.Length, partial.Header.Length - Keywords.DeclareExtension.Length ).Trim();

      Extension result = new Extension {
                                         Author =
                                           new Person() {
                                                          Company = AuthorResolver.ResolveCompany( partial ),
                                                          Email = AuthorResolver.ResolveEmail( partial ),
                                                          Name = AuthorResolver.ResolveAuthor( partial )
                                                        },

                                         Description = DescriptionResolver.Resolve( partial ),
                                         Key = extensionKey,
                                         Models = ModelResolver.Resolve( partial ),
                                         Modules = ModuleResolver.Resolve( partial ),
                                         Plugins = PluginResolver.Resolve( partial ),
                                         Title = TitleResolver.Resolve( partial )
                                       };



      return result;
    }

    public ParsedPartial ParsePartial( string element ) {
      // The place where we're currently at in the markup
      int characterPointer = 0;

      // The currently collected scope body
      string body = String.Empty;

      // Sub-scopes which we'll find during parsing will be stored in this list.
      ParsedPartial result = new ParsedPartial() {
                                                   Body   = string.Empty,
                                                   Header = string.Empty
                                                 };
          
      // How deeply nested we are into scopes
      int scopeLevel  = 0;
      // Are we currently inside a strign?
      bool inString = false;
      // Is the current character escaped?
      bool isEscaped = false;

      // Iterate over the whole input string
      while( characterPointer < element.Length ) {
        // We only check for scopes while we're not parsing within a string.
        if( !inString ) {
          if( Syntax.ScopeTerminate == element.Substring( characterPointer, Syntax.ScopeTerminate.Length ) ) {
            // Did we find a scope terminator? (like ;)
            if( 1 == scopeLevel ) {
              // As long as we're on the first scope level, we can collect the body of scopes to parse them later.
              result.Partials.Add( new ParsedPartial{Body = body.Trim()} );
              result.Body += element.Substring( characterPointer, 1 );
              body = string.Empty;
              ++characterPointer;
              continue;
            }
            if( 0 == scopeLevel ) {
              ++characterPointer;

              continue;
            }
          }

          if( Syntax.ScopeStart == element.Substring( characterPointer, Syntax.ScopeStart.Length ) ) {
            // Did we find the start of a new scope?
            ++scopeLevel;

            if( 1 == scopeLevel ) {
              ++characterPointer;

              continue;
            }

          } else if( Syntax.ScopeEnd == element.Substring( characterPointer, Syntax.ScopeEnd.Length ) ) {
            --scopeLevel;
            // Did we find the end of a scope?
            if( 1 == scopeLevel ) {
              // Great! Another temporary scope we can store for later
              body += element.Substring( characterPointer, 1 );
              result.Partials.Add( new ParsedPartial{Body = body.Trim()} );
              result.Body += element.Substring( characterPointer, 1 );
              body = string.Empty;
              ++characterPointer;
              continue;
            }
            if( 0 == scopeLevel ) {
              ++characterPointer;
              continue;
            }
          }

          if( Syntax.StringDelimiter == element.Substring( characterPointer, Syntax.StringDelimiter.Length ) ) {
            // Did we hit a string delimiter? (like ")
            inString = true;
          }

        } else {
          // We're parsing within a string
          if( Syntax.StringEscape == element.Substring( characterPointer, Syntax.StringEscape.Length ) ) {
            // Did we find an escape sequence?
            isEscaped = true;
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
        ParsedPartial partial = ParsePartial( parsedPartial.Body );

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
          // Replace escaped quotes
          parsedPartial.Parameters = parsedPartial.Parameters.Replace( "\\\"", "\"" );

          if( "\"" != parsedPartial.Parameters.Substring( parsedPartial.Parameters.Length -1, 1 ) ) {
            throw new ParserException( string.Format( "Unmatched \" in {0}", parsedPartial.Header ) );
          }
          parsedPartial.Parameters = parsedPartial.Parameters.Substring( 1, parsedPartial.Parameters.Length - 2 );
        }
      }

      return result;
    }
  }
}
