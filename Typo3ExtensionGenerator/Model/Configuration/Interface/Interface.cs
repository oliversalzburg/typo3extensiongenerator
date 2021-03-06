﻿using System;
using System.Collections.Generic;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model.Configuration.Interface {
  /// <summary>
  /// An interface describes a visualization of some data and/or means to manipulate it.
  /// </summary>
  [Serializable]
  public class Interface : SimpleContainer, IParserResult {
    /// <summary>
    /// The data model field that should be accessed by this interface.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// This is the model our target is contained in.
    /// </summary>
    public DataModel ParentModel { get; set; }

    /// <summary>
    /// The name of the ParentModel.
    /// Used so we can later look up the reference to the parsed instance.
    /// </summary>
    public string ParentModelTarget { get; set; }

    /// <summary>
    /// The contained settings as defined by the user.
    /// </summary>
    public List<KeyValuePair<string, string>> Settings { get; set; }

    /// <summary>
    /// Should the 'exclude' field be set?
    /// Excluded fields can only be edited by admins and specifically configured users.
    /// </summary>
    public bool Exclude { get; set; }

    /// <summary>
    /// A readable title for the described field.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The default value for this interface. The type and value is strongly dependent on the used display type.
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// This is the display type that will be used in the interface.
    /// </summary>
    public IDisplayType DisplayType { get; set; }

    /// <summary>
    /// The TCEForms user interface element that will be used to edit the target.
    /// </summary>
    public string DisplayTypeTarget { get; set; }

    /// <summary>
    /// Construct an interface
    /// </summary>
    public Interface() {
      Settings   = new List<KeyValuePair<string, string>>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
