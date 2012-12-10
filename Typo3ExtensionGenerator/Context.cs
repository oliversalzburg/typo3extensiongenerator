using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Compatibility;

namespace Typo3ExtensionGenerator {
  /// <summary>
  /// The Context holds parameters that affect large parts of how TYPO3 Extension Generator operates.
  /// </summary>
  public class Context {
    /// <summary>
    /// Where should the resulting extension be placed.
    /// </summary>
    public string OutputDirectory { get; set; }

    /// <summary>
    /// The file that contains the extension description.
    /// </summary>
    public string InputFile { get; set; }
    
    /// <summary>
    /// The target TYPO3 version on which our extension should run.
    /// </summary>
    public Typo3Version TargetVersion = Typo3Version.TYPO3_4_7_0;

    /// <summary>
    /// Retrieves a temporary generator context.
    /// </summary>
    /// <returns></returns>
    public static Context GetTemporaryContext() {
      return new Context() {
        OutputDirectory = Path.GetTempPath()
      };
    }
  }
}
