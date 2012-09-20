using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  public class Requirement : IParserResult {
    
    public class RequiredFile {
      public string FullSourceName { get; set; }
      public string RelativeTargetName { get; set; }
    }

    /// <summary>
    /// The folder where to look for files.
    /// </summary>
    public string SourceFolder { get; set; }
    
    /// <summary>
    /// A selector for files that should be included in this requirement.
    /// </summary>
    public List<string> SourceFilter { get; set; }

    /// <summary>
    /// The resulting list of files.
    /// </summary>
    public List<RequiredFile> Files { get; set; }

    public Requirement() {
      SourceFilter = new List<string>();
      Files = new List<RequiredFile>();
    }

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
