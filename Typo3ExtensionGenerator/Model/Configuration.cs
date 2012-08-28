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

    /// <summary>
    /// What field in the data model should be the label for the data model?
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// What field in the data model should be the alternative label for the data model?
    /// </summary>
    public string LabelAlternative { get; set; }

    /// <summary>
    /// Should a label hook be generated for the data model?
    /// </summary>
    public string LabelHook { get; set; }

    /// <summary>
    /// What field should be used as the thumbnail for the data model?
    /// </summary>
    public string Thumbnail { get; set; }

    /// <summary>
    /// What fields in the data model are searchable.
    /// </summary>
    public string SearchFields { get; set; }
  }
}
