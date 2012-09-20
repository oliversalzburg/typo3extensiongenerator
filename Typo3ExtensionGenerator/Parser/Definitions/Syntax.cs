namespace Typo3ExtensionGenerator.Parser.Definitions {
  /// <summary>
  /// Defines the syntax element of the TYPO3 Extension Generator
  /// </summary>
  public static class Syntax {
    #region Scopes
    public const string ScopeStart = "{";
    public const string ScopeEnd = "}";

    public const string ScopeTerminate = ";";
    #endregion


    #region Strings
    /// <summary>
    /// " - Marks beginning and end of string
    /// </summary>
    public const string StringDelimiter = "\"";

    /// <summary>
    /// \ - Allows for escaping syntax elements within string.
    /// </summary>
    public const string StringEscape = "\\";
    #endregion


    #region Comments
    public const string CommentMultilineStart = "/*";
    public const string CommentMultilineEnd = "*/";

    public const string CommentSinglelineStart = "//";
    #endregion

  }
}
