using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model.Plugin {
  public class Listener {
    /// <summary>
    /// The action that should be wrapped by the listener.
    /// </summary>
    public Action TargetAction { get; set; }

    /// <summary>
    /// The name of the class that will dispatch the signal.
    /// </summary>
    /// <example>Tx_Extbase_Persistence_Backend</example>
    /// <example>Tx_Powermail_Controller_FormsController</example>
    public string Host { get; set; }

    /// <summary>
    /// The name of the slot this listener should connect to.
    /// </summary>
    /// <example>afterUpdateObject</example>
    /// <example>createActionAfterSubmitView</example>
    public string Signal { get; set; }
  }
}

