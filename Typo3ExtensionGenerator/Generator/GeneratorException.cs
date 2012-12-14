using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Document;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Usually thrown by generators when the input data doesn't match up.
  /// </summary>
  [Serializable]
  public class GeneratorException : Exception {
    /// <summary>
    /// The file that generated the exception.
    /// </summary>
    private string File { get; set; }
    /// <summary>
    /// The line that generated the exception.
    /// </summary>
    private int Line { get; set; }

    /// <summary>
    /// Constructs a GeneratorException.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cause"></param>
    public GeneratorException( string message, VirtualDocument cause ) : base( message ) {
      if( !cause.Lines.Any() ) {
        Line = -1;
        File = "<unknown file>";
      } else {
        Line = cause.Lines.First().PhysicalLineIndex;
        File = cause.Lines.First().SourceFile;
      }
    }

    /// <summary>
    /// Creates and returns a string representation of the current exception.
    /// </summary>
    /// <returns>
    /// A string representation of the current exception.
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
    public override string ToString() {
      return string.Format( "{0} ({2}:{1})", base.Message, Line, File );
    }
  }
}
