using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration {
  /// <summary>
  /// A palette is a collection of several TCEForms interface elements that should be rendered next to each other.
  /// </summary>
  [Serializable]
  public class Palette : IParserResult {

    /// <summary>
    /// Controls the visibility of a palette
    /// </summary>
    public enum PaletteVisibility {
      /// <summary>
      /// The default visibility - nothing will be added to the configuration
      /// </summary>
      Default,

      /// <summary>
      /// Always show this palette - sets canNotCollapse in the configuration
      /// </summary>
      ShowAlways,

      /// <summary>
      /// Never show this palette - sets isHiddenPalette in the configuration
      /// </summary>
      ShowNever
    }

    /// <summary>
    /// The name of this palette.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// If true, the palette will always be shown, even if the "show secondary options" checkbox is unticked.
    /// </summary>
    public bool CanNotCollapse { get; set; }

    /// <summary>
    /// The fields that are displayed inside this palette.
    /// </summary>
    public string Interface { get; set; }

    /// <summary>
    /// The visibility of this palette
    /// </summary>
    public PaletteVisibility Visibility { get; set; }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
