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
    /// The data model that is displayed through this display type.
    /// </summary>
    DataModel ParentModel { get; set; }

    /// <summary>
    /// Generates the PHP array control structure for TYPO3
    /// </summary>
    /// <param name="format">Should the result be in XML or a be a PHP array?</param>
    /// <returns></returns>
    string GeneratePropertyArray( SimpleContainer.Format format );
  }
}
