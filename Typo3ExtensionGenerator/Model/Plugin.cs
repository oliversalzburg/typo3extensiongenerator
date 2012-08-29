using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration.Interface;

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
    
    /// <summary>
    /// The interfaces to data model fields defined in this configuration.
    /// </summary>
    public List<Interface> Interfaces { get; set; }

    public Plugin() {
      Interfaces = new List<Interface>();
    }
  }
}
