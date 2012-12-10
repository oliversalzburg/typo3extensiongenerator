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
  public class Module : IParserResult, IControllerTemplate {
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

    /// <summary>
    /// The name of a PHP class that implements the proxies defined in the generated class.
    /// </summary>
    public string Implementation { get; set; }

    /// <summary>
    /// The actions that are defined in this object.
    /// These will later be available through an ExtBase controller
    /// </summary>
    public List<Action> Actions { get; set; }

    /// <summary>
    /// Constructs a Module
    /// </summary>
    public Module() {
      Actions = new List<Action>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
