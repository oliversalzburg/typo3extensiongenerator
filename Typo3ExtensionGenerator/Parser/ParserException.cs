using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Parser {
  class ParserException : Exception {
    public int Line { get; private set; }

    public ParserException( string message ) : base( message ) {}
    public ParserException( string message, int line ) : base( message ) {
      Line = line;
    }
  }
}
