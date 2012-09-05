using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Plugin {
  public class Action : IParserResult {
    /// <summary>
    /// The name of the action.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A human-readable title for the action.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Should this action be marked as uncachable?
    /// </summary>
    public bool Uncachable { get; set; }

    /// <summary>
    /// A list of parameter names that will be required to call this action.
    /// </summary>
    public List<string> Requirements { get; set; }

    public Action() {
      Requirements = new List<string>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The parsed partial from which this object was generated.
    /// </summary>
    public ExtensionParser.ParsedPartial SourcePartial { get; set; }
    #endregion
  }
}
