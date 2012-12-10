using System;
using System.Collections.Generic;
using System.Linq;

namespace Typo3ExtensionGenerator.Compatibility {
  /// <summary>
  /// Keeps track of the deprecation state of TYPO3 methods.
  /// </summary>
  public static class Deprecated {

    /// <summary>
    /// Storage to keep track of method replacements.
    /// </summary>
    private static Dictionary<string,List<KeyValuePair<Typo3Version,string>>> Replacements { get; set; }

    /// <summary>
    /// Returns <see langword="true"/> if this signature is deprecated in any version of TYPO3.
    /// </summary>
    /// <param name="signature"></param>
    /// <returns></returns>
    public static bool Is( string signature ) {
      return Replacements.ContainsKey( signature );
    }

    /// <summary>
    /// Registers a method replacement
    /// </summary>
    /// <param name="signature">The source method that is deprecated. For example: t3lib_div::readLLXMLfile</param>
    /// <param name="targetVersion">The version since this method is deprecated. For example: <see cref="Typo3Version.TYPO3_4_6_0"/></param>
    /// <param name="replacement">The method that should be used instead. For example: t3lib_l10n_parser_Llxml::getParsedData</param>
    /// <returns>Always returns <see langword="null"/></returns>
    public static Object Register( string signature, Typo3Version targetVersion, string replacement ) {
      if( null == Replacements ) {
        Replacements = new Dictionary<string, List<KeyValuePair<Typo3Version, string>>>();
      }
      if( !Replacements.ContainsKey( signature ) ) {
        Replacements[ signature ] = new List<KeyValuePair<Typo3Version, string>>();
      }
      Replacements[ signature ].Add( new KeyValuePair<Typo3Version, string>( targetVersion, replacement ) );
      return null;
    }

    /// <summary>
    /// Retrieve a replacement method signature for a given method signature.
    /// </summary>
    /// <param name="signature">The signature to look up.</param>
    /// <param name="targetVersion">The target TYPO3 version we want to use.</param>
    /// <returns></returns>
    public static string Get( string signature, Typo3Version targetVersion ) {
      if( !Is( signature ) ) {
        return signature;
      }
      if( !Replacements[ signature ].Any( r => r.Key.Version <= targetVersion.Version ) ) {
        return signature;
      }

      // In case the given replacement is again already deprecated, run it through the system again.
      return Get( Replacements[ signature ].Single( r => r.Key.Version <= targetVersion.Version ).Value, targetVersion );
    }
  }
}
