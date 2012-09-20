using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// An IParserResult describes an object that was produced by parsing an element.
  /// It is primarily used to enforce implementation of the source line and file attributes.
  /// </summary>
  public interface IParserResult {
    /// <summary>
    /// The file in which the line was located.
    /// </summary>
    string SourceFile { get; set; }

    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    int SourceLine { get; set; }

    /// <summary>
    /// The parsed fragment from which this object was generated.
    /// </summary>
    Fragment SourceFragment { get; set; }
  }
}
