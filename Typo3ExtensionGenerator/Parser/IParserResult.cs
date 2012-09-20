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
    /// The parsed fragment from which this object was generated.
    /// </summary>
    Fragment SourceFragment { get; set; }
  }
}
