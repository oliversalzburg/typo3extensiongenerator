using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Typo3ExtensionGenerator.Parser.Document {
  /// <summary>
  /// The VirtualDocument provides an interface to handle a hierarchy of files as if they were a flat text file.
  /// Additionally, it provides helper constructs to navigate the logical elements of the file.
  /// </summary>
  public class VirtualDocument {

    /// <summary>
    /// A single character in a virtual document.
    /// </summary>
    public class Character {

      /// <summary>
      /// This is the character on the parent line for which this wrapper is instantiated.
      /// </summary>
      private char PhysicalCharacter { get; set; }

      internal int Index { get; set; }

      /// <summary>
      /// The line on which this character lives
      /// </summary>
      public Line ParentLine { get; protected set; }

      /// <summary>
      /// From which file did this character originate?
      /// </summary>
      internal string SourceFile { get; set; }

      /// <summary>
      /// Constructs a character wrapper
      /// </summary>
      /// <param name="physicalCharacter"></param>
      /// <param name="parentLine"></param>
      public Character( char physicalCharacter, Line parentLine ) {
        PhysicalCharacter = physicalCharacter;
        ParentLine = parentLine;
      }

      public override string ToString() {
        return string.Format( "{0}", PhysicalCharacter );
      }
    }

    /// <summary>
    /// A single line in a virtual document
    /// </summary>
    public class Line : ICloneable {

      /// <summary>
      /// From which file did this line originate?
      /// </summary>
      public string SourceFile { get; private set; }

      /// <summary>
      /// The literal line for which this wrapper was instantiated.
      /// </summary>
      private string PhysicalLine { get; set; }

      /// <summary>
      /// Returns the part of the physical line to which this line was virtually trimmed.
      /// </summary>
      public string VirtualLine { 
        get { return PhysicalLine.Substring( VirtualWindowBegin, VirtualWindowEnd - VirtualWindowBegin ); }
      }

      /// <summary>
      /// Where does the virtual window for this line begin?
      /// </summary>
      /// <see cref="VirtualLine"/>
      public int VirtualWindowBegin { get; private set; }
      /// <summary>
      /// Where does the virtual window for this line end?
      /// </summary>
      /// <see cref="VirtualLine"/>
      public int VirtualWindowEnd { get; private set; }

      /// <summary>
      /// The index of the PhysicalLine in the file it was originally retrieved from.
      /// This does not relate to the position of the line within the virtual document.
      /// </summary>
      public int PhysicalLineIndex { get; internal set; }

      /// <summary>
      /// The index of the line inside the virtual document.
      /// </summary>
      public int VirtualLineIndex { get; internal set; }

      /// <summary>
      /// The virtual characters that live on this line.
      /// </summary>
      public IEnumerable<Character> Characters { get; private set; }

      /// <summary>
      /// Constructs a line wrapper.
      /// </summary>
      /// <param name="physicalLine">The physical line that should be wrapped by this line.</param>
      /// <param name="sourceFile">From which file did this line originate?</param>
      /// <param name="virtualBegin">If the line should be virtually trimmed, this is the first visible character.</param>
      /// <param name="virtualEnd">If the line should be virtually trimmed, this is the last visible character.</param>
      public Line( string physicalLine, string sourceFile, int virtualBegin = 0, int virtualEnd = -1 ) {
        PhysicalLine = physicalLine;
        SourceFile = sourceFile;

        PhysicalLineIndex = -1;
        VirtualLineIndex  = -1;

        // Set virtual trimming
        Trim( virtualBegin, virtualEnd );
      }

      /// <summary>
      /// Virtually trims the line.
      /// </summary>
      /// <param name="virtualBegin">If the line should be virtually trimmed, this is the first visible character.</param>
      /// <param name="virtualEnd">If the line should be virtually trimmed, this is the last visible character.</param>
      /// <see cref="VirtualLine"/>
      public void Trim( int virtualBegin = 0, int virtualEnd = -1 ) {
        if( -1 == virtualEnd ) {
          virtualEnd = PhysicalLine.Length;
        }
        VirtualWindowBegin = virtualBegin;
        VirtualWindowEnd   = virtualEnd;

        // Construct Character instances which reference this line as their parent.
        char[] charArray = VirtualLine.ToCharArray();
        Characters = charArray.Select( c => new Character( c, this ) ).ToList();
        // Now give them indexes
        int index = 0;
        foreach( Character character in Characters ) {
          character.Index      = index++;
          character.SourceFile = SourceFile;
        }
      }

      public override string ToString() {
        return string.Format( "({2}){1}: {0}", PhysicalLine, PhysicalLineIndex, VirtualLineIndex );
      }

      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>
      /// A new object that is a copy of this instance.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public object Clone() {
        return MemberwiseClone();
      }
    }

    /// <summary>
    /// The lines contained within this document.
    /// </summary>
    public List<Line> Lines { get; private set; }

    /// <summary>
    /// Writes incremeting line index to each contained line.
    /// </summary>
    private void ReIndexLines() {
      for( int lineIndex = 0; lineIndex < Lines.Count; lineIndex++ ) {
        Lines[ lineIndex ].PhysicalLineIndex = lineIndex;
      }
    }

    /// <summary>
    /// Updates the virtual line count in all contained lines.
    /// </summary>
    public void UpdateVirtualLineCount() {
      for( int lineIndex = 0; lineIndex < Lines.Count; lineIndex++ ) {
        Lines[ lineIndex ].VirtualLineIndex = lineIndex;
      }
    }

    /// <summary>
    /// Replaces a given line with another virtual document.
    /// </summary>
    /// <param name="line">The line that should be replaced.</param>
    /// <param name="document">The document that should be inserted.</param>
    public void SubstituteLine( Line line, VirtualDocument document ) {
      int position = Lines.IndexOf( line );
      Lines.RemoveAt( position );
      Lines.InsertRange( position, document.Lines );
    }

    /// <summary>
    /// Construct a new virtual document.
    /// </summary>
    private VirtualDocument() {
      Lines = new List<Line>();
    }

    /// <summary>
    /// Construct a new virtual document from the given string.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="sourceFileName">The name of the file this text originated from.</param>
    /// <returns></returns>
    public static VirtualDocument FromText( string text, string sourceFileName = "<memory>" ) {
      VirtualDocument document = new VirtualDocument();
      document.Lines = Regex.Split( text, "\r\n|\r|\n" ).Select( l => new Line( l, sourceFileName ) ).ToList();
      // Lay down the first
      document.ReIndexLines();
      document.UpdateVirtualLineCount();

      return document;
    }

    /// <summary>
    /// Construct a new virtual document from the contents of a file.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static VirtualDocument FromFile( string filename ) {
      string allText = File.ReadAllText( filename );
      return FromText( allText, filename );
    }

    /// <summary>
    /// Construct a new virtual document from a subsection of another virtual document.
    /// Assumes begin &lt; end
    /// </summary>
    /// <param name="document"></param>
    /// <param name="begin">The character that marks the beginning of the new, virtual document.</param>
    /// <param name="end">The character that marks the end of the new, virtual document.</param>
    /// <returns></returns>
    public static VirtualDocument FromDocument( VirtualDocument document, Character begin, Character end ) {
      // Store references to lines.
      Line first = begin.ParentLine;
      Line last  = end.ParentLine;

      VirtualDocument section = new VirtualDocument();

      // Find the index of the lines within the document.
      int firstLineIndex = document.Lines.FindIndex( l => l.Characters == first.Characters );
      int lastLineIndex = document.Lines.FindIndex( l => l.Characters == last.Characters );
      Debug.Assert( firstLineIndex != -1 && lastLineIndex != -1 );

      // Clone the lines into the new document
      foreach( Line line in document.Lines.GetRange( firstLineIndex, lastLineIndex - firstLineIndex + 1 ) ) {
        section.Lines.Add( (Line)line.Clone() );
      }
      // Now we can adjust the line numbers for this new virtual document.
      section.UpdateVirtualLineCount();

      // Trim start and end sections
      int startIndex = first.Characters.ToList().IndexOf( begin ) + first.VirtualWindowBegin;
      section.Lines.First().Trim( startIndex );

      int endIndex = last.Characters.ToList().IndexOf( end );
      section.Lines.Last().Trim( ( first != last ) ? 0 : startIndex, endIndex + first.VirtualWindowBegin + 1 );

      return section;
    }
  }
}
