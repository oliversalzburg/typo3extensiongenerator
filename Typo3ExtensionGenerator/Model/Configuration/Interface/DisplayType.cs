using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration.Interface {
  /// <summary>
  /// Used to describe a user interface control in the backend
  /// </summary>
  [Serializable]
  public class DisplayType : IDisplayType {
     /// <summary>
    /// The name of this data model.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The contained data members.
    /// </summary>
    public List<KeyValuePair<string, string>> Parameters { get; set; }

    /// <summary>
    /// The type that will be supplied inside the config section to TYPO3.
    /// </summary>
    public string InternalType { get; set; }

    public DisplayType() {
      Parameters = new List<KeyValuePair<string, string>>();
    }

    public string GeneratePropertyArray() {
      return string.Empty;
    }
  }
}
