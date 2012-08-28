using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// A configuration for a data model.
  /// </summary>
  [Serializable]
  public class Configuration {
    /// <summary>
    /// The data model that should be configured by this configuration.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// The target data model.
    /// </summary>
    public DataModel Model { get; set; }
  }
}
