using System;
using System.Collections.Generic;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Plugin {
  /// <summary>
  /// A plugin for TYPO3.
  /// </summary>
  [Serializable]
  public class Plugin : IParserResult {
    /// <summary>
    /// The name of this plugin.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The speaking title that will be displayed in the backend.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The PHP class that implements the actions of our resulting ExtBase controller.
    /// </summary>
    public string Implementation { get; set; }

    /// <summary>
    /// The data model for the FlexForm settings in this plugin.
    /// </summary>
    public DataModel Model { get; set; }
    
    /// <summary>
    /// The interfaces to data model fields defined in this configuration.
    /// </summary>
    public List<Interface> Interfaces { get; set; }

    public List<Action> Actions { get; set; }

    /// <summary>
    /// ExtBase SignalSlot listeners that should be connected.
    /// </summary>
    public List<Listener> Listeners { get; set; }

    public Plugin() {
      Interfaces = new List<Interface>();
      Actions    = new List<Action>();
      Listeners  = new List<Listener>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
