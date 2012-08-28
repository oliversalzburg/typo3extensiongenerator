using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  public class Extension {
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
    /// The creator of this extension.
    /// </summary>
    public Person Author { get; set; }

    /// <summary>
    /// The plugins defined in this extension.
    /// </summary>
    public List<Plugin> Plugins { get; set; }

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
    public List<Configuration> Configurations { get; set; }
  }
}
