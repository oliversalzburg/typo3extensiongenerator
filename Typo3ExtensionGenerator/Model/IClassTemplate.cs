using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// The IClassTemplate describes an object that contains properties that are commonly used to generate a class.
  /// </summary>
  public interface IClassTemplate : IParserResult {
    /// <summary>
    /// The name of this object.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The name of a PHP class that implements the proxies defined in the generated class.
    /// </summary>
    string Implementation { get; }

    /// <summary>
    /// The actions that are defined in this object.
    /// These will later be available through methods in the class.
    /// </summary>
    List<Action> Actions { get; }
  }
}
