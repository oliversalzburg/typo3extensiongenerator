using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration.Interface {
  /// <summary>
  /// The SpecializedDisplayType derives from the SimpleContainer to allow for easy property collection.
  /// </summary>
  public class SpecializedDisplayType : SimpleContainer, IDisplayType, IParserResult {
    #region Implementation of IDisplayType
    /// <summary>
    /// The data model that is displayed through this display type.
    /// </summary>
    public DataModel ParentModel { get; set; }
    #endregion

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
