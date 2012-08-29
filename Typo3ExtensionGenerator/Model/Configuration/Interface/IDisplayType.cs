using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model.Configuration.Interface {
  public interface IDisplayType {
    /// <summary>
    /// The name of this data model.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Generates the PHP array control structure for TYPO3
    /// </summary>
    /// <returns></returns>
    string GeneratePropertyArray();
  }
}
