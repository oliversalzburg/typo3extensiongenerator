using System;

namespace Typo3ExtensionGenerator.Compatibility {
  /// <summary>
  /// Describes a given TYPO3 version
  /// </summary>
  public class Typo3Version {

    /// <summary>
    /// TYPO3 4.5.0 LTS
    /// </summary>
    public static readonly Typo3Version TYPO3_4_5_0 = new Typo3Version( "4.5.0" );
    
    /// <summary>
    /// TYPO3 4.6.0
    /// </summary>
    public static readonly Typo3Version TYPO3_4_6_0 = new Typo3Version( "4.6.0" );

    /// <summary>
    /// TYPO3 4.7.0
    /// </summary>
    public static readonly Typo3Version TYPO3_4_7_0 = new Typo3Version( "4.7.0" );

    /// <summary>
    /// TYPO3 6.0.0
    /// </summary>
    public static readonly Typo3Version TYPO3_6_0_0 = new Typo3Version( "6.0.0" );

    /// <summary>
    /// The version number for this TYPO3 version
    /// </summary>
    public Version Version { get; private set; }

    /// <summary>
    /// Default Constructor
    /// </summary>
    private Typo3Version( string version ) {
      Version = new Version( version );
    }
  }
}
