using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// DataModel describes a generic data container inside the extension.
  /// From these models, we'll generate the SQL description, the ExtBase model, ...
  /// </summary>
  [Serializable]
  public class DataModel {
    /// <summary>
    /// The name of this data model.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The contained data members.
    /// </summary>
    public List<KeyValuePair<string, string>> Members { get; set; }

    public DataModel() {
      Members = new List<KeyValuePair<string, string>>();
    }
  }
}
