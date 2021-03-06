﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// Represents an ExtBase repository
  /// </summary>
  public class Repository : IParserResult {
    /// <summary>
    /// The name of the data model for which this is the repository.
    /// </summary>
    public string TargetModelName { get; set; }

    /// <summary>
    /// The already existing repository type in TYPO3 that is implemented through this repository.
    /// </summary>
    public string InternalType { get; set; }

    /// <summary>
    /// The PHP class that implements the methods in our ExtBase repository.
    /// </summary>
    public string Implementation { get; set; }

    public List<Action> Methods { get; set; }

    public Repository() {
      Methods = new List<Action>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
