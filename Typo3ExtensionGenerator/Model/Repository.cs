using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// Represents an ExtBase repository
  /// </summary>
  public class Repository : IParserResult {
    /// <summary>
    /// The name of the data model for which this is the repository.
    /// </summary>
    public string TargetModelName { get; set; }

    /// <summary>
    /// The already existing repository type in TYPO3 that is implemented through this repository.
    /// </summary>
    public string InternalType { get; set; }

    /// <summary>
    /// The PHP class that implements the methods in our ExtBase repository.
    /// </summary>
    public string Implementation { get; set; }

    public List<Plugin.Action> Methods { get; set; }

    public Repository() {
      Methods = new List<Plugin.Action>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The file in which the line was located.
    /// </summary>
    public string SourceFile { get; set; }

    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
