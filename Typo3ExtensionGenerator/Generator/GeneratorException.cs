using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Document;

namespace Typo3ExtensionGenerator.Generator {
  [Serializable]
  public class GeneratorException : Exception {
    public string File { get; private set; }
    public int Line { get; private set; }

    public GeneratorException( string message, VirtualDocument cause ) : base( message ) {
      if( !cause.Lines.Any() ) {
        Line = -1;
        File = "<unknown file>";
      } else {
        Line = cause.Lines.First().PhysicalLineIndex;
        File = cause.Lines.First().SourceFile;
      }
    }

    public override string ToString() {
      return string.Format( "{0} ({2}:{1})", base.Message, Line, File );
    }
  }
}
