using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Task {
  /// <summary>
  /// A class template for a class that describes additional fields for a scheduler task
  /// </summary>
  public class TaskFields : IClassTemplate {
    /// <summary>
    /// The name of this object.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of a PHP class that implements the proxies defined in the generated class.
    /// </summary>
    public string Implementation { get; set; }

    /// <summary>
    /// The actions that are defined in this object.
    /// These will later be available through methods in the class.
    /// </summary>
    public List<Action> Actions { get; private set; }

    /// <summary>
    /// Constructs a Task model
    /// </summary>
    public TaskFields() {
      Actions = new List<Action>() {
        new Action {Name = "getAdditionalFields", Requirements = new List<string> {"array &taskInfo", "task", "tx_scheduler_Module parentObject"}},
        new Action {Name = "validateAdditionalFields", Requirements = new List<string> {"array &submittedData", "tx_scheduler_Module parentObject"}},
        new Action {Name = "saveAdditionalFields", Requirements = new List<string> {"array submittedData", "tx_scheduler_Task task"}}
      };
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
