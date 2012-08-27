using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Generator {
  [Serializable]
  public class GeneratorException : Exception {
    public GeneratorException( string message ) : base( message ) {}
  }
}
