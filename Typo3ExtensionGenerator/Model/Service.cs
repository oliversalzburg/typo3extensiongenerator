using System;
using System.Collections.Generic;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// A service class for TYPO3.
  /// </summary>
  [Serializable]
  public class Service : IParserResult  {
    /// <summary>
    /// The name of this plugin.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The PHP class that implements the actions of our resulting ExtBase controller.
    /// </summary>
    public string Implementation { get; set; }

    public List<Action> Actions { get; set; }

    /// <summary>
    /// ExtBase SignalSlot listeners that should be connected.
    /// </summary>
    // TODO: It's unclear if ExtBase signals can call into service classes
    //public List<Listener> Listeners { get; set; }

    public Service() {
      Actions    = new List<Action>();
      //Listeners  = new List<Listener>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
