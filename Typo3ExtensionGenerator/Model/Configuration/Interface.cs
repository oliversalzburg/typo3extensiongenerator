using System;
using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration {
  /// <summary>
  /// DataModel describes a generic data container inside the extension.
  /// From these models, we'll generate the SQL description, the ExtBase model, ...
  /// </summary>
  [Serializable]
  public class Interface {
    /// <summary>
    /// The data model field that should be accessed by this interface.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// The contained settings.
    /// </summary>
    public List<KeyValuePair<string, string>> Settings { get; set; }

    /// <summary>
    /// Should the 'exclude' field be set?
    /// Excluded fields can only be edited by admins and specifically configured users.
    /// </summary>
    public bool Exclude { get; set; }

    public Interface() {
      Settings = new List<KeyValuePair<string, string>>();
    }
  }
}
