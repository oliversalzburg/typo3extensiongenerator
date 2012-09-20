using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  public class Extension : IParserResult {
    /// <summary>
    /// The extension key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The display name of the extension.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A short description of the extension.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The category of this extension.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// The creator of this extension.
    /// </summary>
    public Person Author { get; set; }

    /// <summary>
    /// The state of the extension (alpha/beta/stable)
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// The version of the extension (1.2.3).
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// The file that contains the implementation of our label hooks.
    /// </summary>
    public string LabelHookImplementation { get; set; }

    /// <summary>
    /// The plugins defined in this extension.
    /// </summary>
    public List<Plugin.Plugin> Plugins { get; set; }

    /// <summary>
    /// The modules defined in this extension.
    /// </summary>
    public List<Module> Modules { get; set; }

    /// <summary>
    /// The data models defined in this extension.
    /// </summary>
    public List<DataModel> Models { get; set; }

    /// <summary>
    /// The configurations for the data models.
    /// </summary>
    public List<Configuration.Configuration> Configurations { get; set; }

    /// <summary>
    /// The ExtBase repositories used in this extension.
    /// </summary>
    public List<Repository> Repositories { get; set; }

    /// <summary>
    /// A list of files that need to be copied to the output directory to complete the extension.
    /// </summary>
    public List<Requirement> Requirements { get; set; }

    #region Implementation of IParserResult
    /// <summary>
    /// The file in which the line was located.
    /// </summary>
    public string SourceFile { get; set; }

    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
