using System.Collections.Generic;
using System.IO;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Parser.Document;
using log4net;

namespace Typo3ExtensionGenerator.PreProcess {
  /// <summary>
  /// Resolves #include statements in the given markup.
  /// #include can only appear at the start of a line. Whitespace in front of it is ignored.
  /// </summary>
  public static class ResolveIncludes {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Resolves the include statements in the given document.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static VirtualDocument Resolve( VirtualDocument document ) {
      // We'll build a dictionary of replacement actions to perform them after initial enumeration
      Dictionary<VirtualDocument.Line, VirtualDocument> substitutions = new Dictionary<VirtualDocument.Line, VirtualDocument>();

      // Try to find lines that start with optional whitespace and then an #include statement
      foreach( VirtualDocument.Line line in document.Lines ) {
        if( line.VirtualLine.Trim().StartsWith( Keywords.PreProcessInclude ) ) {
          // Grab the file name from the statement
          string filename = ExtractFilename( line );
          if( !File.Exists( filename ) ) {
            throw new ParserException( string.Format( "Given include file '{0}' does not exist.", filename ), line );
          }
          Log.InfoFormat( "Including '{0}'.", filename );

          // Remember this substitution
          substitutions.Add( line, VirtualDocument.FromFile( filename ) );
        }
      }

      // Perform substitutions
      foreach( KeyValuePair<VirtualDocument.Line, VirtualDocument> substitution in substitutions ) {
        document.SubstituteLine( substitution.Key, substitution.Value );
      }
      document.UpdateVirtualLineCount();

      return document;
    }

    /// <summary>
    /// Grabs the filename from an #include statement.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    /// <example>#include "foo/bar.txt"</example>
    private static string ExtractFilename( VirtualDocument.Line line ) {
      string buffer = line.VirtualLine;

      // Remove whitespace
      buffer = buffer.Trim();

      // Remove #include
      buffer = buffer.Substring( Keywords.PreProcessInclude.Length );

      // Consume optional whitespace (between #include and "")
      buffer = buffer.Trim();

      // Remainder is expected to be filename wrapped in ""
      if( !buffer.StartsWith( Syntax.StringDelimiter ) || !buffer.EndsWith( Syntax.StringDelimiter ) ) {
        throw new ParserException( string.Format( "Unterminated string in preprocessor directive '{0}'.", buffer.Trim() ), line );
      }

      // Now we trim those "" away.
      buffer = buffer.Trim( new[] {'"'} );

      // Now we should be left with the filename
      return buffer;
    }
  }
}
