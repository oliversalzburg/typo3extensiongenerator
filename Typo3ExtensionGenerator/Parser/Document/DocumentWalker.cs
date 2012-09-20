using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Typo3ExtensionGenerator.Parser.Document {
  /// <summary>
  /// Provides an interface to walk through a virtual document.
  /// </summary>
  internal class DocumentWalker {
    public VirtualDocument Document { get; set; }

    /// <summary>
    /// The index of the current character we're currently looking at.
    /// </summary>
    private int CharacterPointer = 0;

    /// <summary>
    /// The index of the line we're currently looking at.
    /// </summary>
    private int LinePointer = 0;

    /// <summary>
    /// The character we're currently looking at.
    /// </summary>
    public VirtualDocument.Character CurrentCharacter {
      get {
        if( LinePointer >= Document.Lines.Count || CharacterPointer >= CurrentLine.VirtualLine.Length ) return null;
        return CurrentLine.Characters.ElementAt( CharacterPointer );
      }
    }

    /// <summary>
    /// The line in the virtual document we're currently looking at.
    /// </summary>
    public VirtualDocument.Line CurrentLine {
      get { return Document.Lines[ LinePointer ]; }
    }

    /// <summary>
    /// Can we walk forward, on the current line (without skipping into the next line)?
    /// </summary>
    public bool CanWalkForward {
      get { return LinePointer < Document.Lines.Count && CurrentLine.Characters.Count() - 1 > CharacterPointer; }
    }

    /// <summary>
    /// Can we walk? Only fails at the end of the document
    /// </summary>
    public bool CanWalk {
      get {
        return LinePointer < Document.Lines.Count && ( CurrentLine != Document.Lines.Last()
               || CurrentLine.Characters.Count() > CharacterPointer );
      }
    }

    /// <summary>
    /// Are we currently looking at the specified string.
    /// </summary>
    /// <param name="marker">The marker we should be checking for.</param>
    /// <returns></returns>
    public bool CurrentlyReads( string marker ) {
      Debug.Assert( !marker.Contains( "\n" ) );

      string currentLine = CurrentLine.VirtualLine;
      // If we hit the end of the line, we don't need to pull in the content from the next line, a line break is never expected in the marker.
      if( currentLine.Length < CharacterPointer + marker.Length ) {
        return false;
      }

      return currentLine.Substring( CharacterPointer, marker.Length ) == marker;
    }

    /// <summary>
    /// Step ahead in the virtual document.
    /// </summary>
    public void Walk() {
      if( CurrentLine.Characters.Count() <= CharacterPointer + 1 ) {
        CharacterPointer = 0;
        
        if( LinePointer < Document.Lines.Count ) {
          ++LinePointer;
        }
      } else {
        ++CharacterPointer;
      }
    }

    /// <summary>
    /// Walks to the next non-whitespace character, but at least once.
    /// </summary>
    public void WalkToNext() {
      do {
        Walk();
      } while( null == CurrentCharacter || Regex.IsMatch( CurrentCharacter.ToString(), "\t|\r|\n| " ) );
    }

    /// <summary>
    /// Step the given amount of steps ahead.
    /// </summary>
    /// <param name="amount"></param>
    public void Walk( int amount ) {
      while( amount-- > 0 ) {
        Walk();
      }
    }

    /// <summary>
    /// Constructs a DocumentWalker for the given virtual document.
    /// </summary>
    /// <param name="document"></param>
    public DocumentWalker( VirtualDocument document ) {
      Document = document;
    }

    public override string ToString() {
      return CurrentLine.VirtualLine;
    }
  }
}