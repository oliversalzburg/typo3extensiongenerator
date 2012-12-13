using System;
using System.Collections.Generic;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Task {
  /// <summary>
  /// A scheduler task for TYPO3.
  /// </summary>
  [Serializable]
  public class Task : IParserResult, IClassTemplate  {
    /// <summary>
    /// The name of this task.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The readable title of this task.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A longer description for this task.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The PHP class that implements our task.
    /// </summary>
    public string Implementation { get; set; }

    /// <summary>
    /// The PHP class that describes the additional fields for the task.
    /// </summary>
    public string AdditionalFieldsClass { get; set; }

    /// <summary>
    /// The data model used to generate the class for the additional task fields.
    /// </summary>
    public TaskFields TaskFields { get; set; }

    /// <summary>
    /// The actions that are defined in this object.
    /// These will later be available through methods in the class.
    /// For a task, there's usually only 1 action - execute.
    /// </summary>
    public List<Action> Actions { get; private set; }

    /// <summary>
    /// Constructs a Task model
    /// </summary>
    public Task() {
      Actions = new List<Action>() {new Action() {Name = "execute"}};
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
