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
  public class DisplayType : IDisplayType, IParserResult {
     /// <summary>
    /// The name of this data model.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The contained data members.
    /// </summary>
    public List<KeyValuePair<string, string>> Parameters { get; set; }

    #region Implementation of IDisplayType
    /// <summary>
    /// The data model that is displayed through this display type.
    /// </summary>
    public DataModel ParentModel { get; set; }
    #endregion

    public DisplayType() {
      Parameters = new List<KeyValuePair<string, string>>();
    }

    public string GeneratePropertyArray( SimpleContainer.Format format ) {
      return string.Empty;
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The file in which the line was located.
    /// </summary>
    public string SourceFile { get; set; }

    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
