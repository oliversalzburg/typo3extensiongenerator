using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// A plugin for TYPO3.
  /// </summary>
  [Serializable]
  public class Plugin {
    /// <summary>
    /// The name of this plugin.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The speaking title that will be displayed in the backend.
    /// </summary>
    public string Title { get; set; }
    
  }
}
