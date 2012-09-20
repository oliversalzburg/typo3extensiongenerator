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
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
    
  }
}
