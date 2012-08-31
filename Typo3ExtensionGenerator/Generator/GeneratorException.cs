using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Generator {
  [Serializable]
  public class GeneratorException : Exception {
    public int Line { get; private set; }

    public GeneratorException( string message,int line ) : base( message ) {
      Line = line;
    }

    public override string ToString() {
      return string.Format( "Line: {1}: {0}", base.Message, Line );
    }
  }
}
