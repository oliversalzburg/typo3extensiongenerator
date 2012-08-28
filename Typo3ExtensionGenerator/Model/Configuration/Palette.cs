using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model.Configuration {
  /// <summary>
  /// A palette is a collection of several TCEForms interface elements that should be rendered next to each other.
  /// </summary>
  [Serializable]
  public class Palette {
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
  }
}
