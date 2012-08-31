using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// An IParserResult describes an object that was produced by parsing an exception.
  /// </summary>
  public interface IParserResult {
    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    int SourceLine { get; set; }

    /// <summary>
    /// The parsed partial from which this object was generated.
    /// </summary>
    ExtensionParser.ParsedPartial SourcePartial { get; set; }
  }
}
