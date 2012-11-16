using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  public interface IControllerTemplate {

    /// <summary>
    /// The name of this object.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The actions that are defined in this object.
    /// These will later be available through an ExtBase controller
    /// </summary>
    List<Action> Actions { get; }

  }
}
