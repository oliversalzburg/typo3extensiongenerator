using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration.Interface {
  /// <summary>
  /// The SpecializedDisplayType derives from the SimpleContainer to allow for easy property collection.
  /// </summary>
  public class RecordGroupDisplayType : SpecializedDisplayType, IDisplayType, IParserResult {
    /// <summary>
    /// Should thumbnails be shown for the selected records?
    /// </summary>
    public bool ShowThumbnails { get; set; }
    
    /// <summary>
    /// How many items high should the selector be?
    /// </summary>
    public int Lines { get; set; }

    /// <summary>
    /// Sets a style directive on the resulting HTML element.
    /// </summary>
    public string SelectedListStyle { get; set; }

    /// <summary>
    /// Allow items to appear multiple times in the selection.
    /// </summary>
    public bool AllowDuplicates { get; set; }

    /// <summary>
    /// The maximum amount of items that can be selected.
    /// </summary>
    public int MaxItems { get; set; }

    /// <summary>
    /// The minimum required amount of selected items.
    /// </summary>
    public int MinItems { get; set; }

    public RecordGroupDisplayType() {
      ShowThumbnails  = false;
      Lines           = 5;
      AllowDuplicates = false;
      MinItems        = 0;
      MaxItems        = 99;
    }

    public new string GeneratePropertyArray( Format format ) {
      


      return base.GeneratePropertyArray( format );
    }
  }
}
