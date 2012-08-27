using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// A module for TYPO3.
  /// </summary>
  [Serializable]
  public class Module {
    /// <summary>
    /// What main module is this module a child of?
    /// </summary>
    /// <example>The Tools menu, so "tools"</example>
    public string MainModuleName { get; set; }

    /// <summary>
    /// The name of this plugin.
    /// </summary>
    public string Name { get; set; }
    
    /*
    /// <summary>
    /// The speaking title that will be displayed in the backend.
    /// </summary>
    public string Title { get; set; }
    */
  }
}
