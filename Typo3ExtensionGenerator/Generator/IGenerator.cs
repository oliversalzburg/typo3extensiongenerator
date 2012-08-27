using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator {
  public interface IGenerator {
    /// <summary>
    /// The name of the file this generator wants to create.
    /// </summary>
    string TargetFile { get; }

    void Generate();
  }
}
