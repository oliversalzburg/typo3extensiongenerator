using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// A module for TYPO3.
  /// </summary>
  [Serializable]
  public class Module : IParserResult {
    /// <summary>
    /// What main module is this module a child of?
    /// </summary>
    /// <example>The Tools menu, so "tools"</example>
    public string MainModuleName { get; set; }

    /// <summary>
    /// The name of this plugin.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The human-readable title for the module
    /// </summary>
    public string Title { get; set; }
    
    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
