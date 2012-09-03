using System;
using System.Collections.Generic;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration {
  /// <summary>
  /// A configuration for a data model.
  /// </summary>
  [Serializable]
  public class Configuration : IParserResult {
    /// <summary>
    /// The data model that should be configured by this configuration.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// The target data model.
    /// </summary>
    public DataModel Model { get; set; }

    /// <summary>
    /// The readable name for the configured data model.
    /// </summary>
    public string Title { get; set; }

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
    public bool LabelHook { get; set; }

    /// <summary>
    /// What field should be used as the thumbnail for the data model?
    /// </summary>
    public string Thumbnail { get; set; }

    /// <summary>
    /// What fields in the data model are searchable.
    /// </summary>
    public string SearchFields { get; set; }

    /// <summary>
    /// The fields that will be displayed in the Info/View Item dialog
    /// </summary>
    public string InterfaceInfo { get; set; }

    /// <summary>
    /// Is the configured model hidden from backend users?
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// The types defined in this data model configuration.
    /// </summary>
    public List<Type> Types { get; set; }

    /// <summary>
    /// The palettes defined  in this data model configuration.
    /// </summary>
    public List<Palette> Palettes { get; set; }

    /// <summary>
    /// The interfaces to data model fields defined in this configuration.
    /// </summary>
    public List<Interface.Interface> Interfaces { get; set; }

    public Configuration() {
      Types      = new List<Type>();
      Palettes   = new List<Palette>();
      Interfaces = new List<Interface.Interface>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The parsed partial from which this object was generated.
    /// </summary>
    public ExtensionParser.ParsedPartial SourcePartial { get; set; }
    #endregion
  }
}
