namespace Typo3ExtensionGenerator.Parser {
  public static class Syntax {
    #region Scopes
    public const string ScopeStart = "{";
    public const string ScopeEnd = "}";

    public const string ScopeTerminate = ";";
    #endregion


    #region Strings
    public const string StringDelimiter = "\"";
    public const string StringEscape = "\\";
    #endregion


    #region Comments
    public const string CommentMultilineStart = "/*";
    public const string CommentMultilineEnd = "*/";

    public const string CommentSinglelineStart = "//";
    #endregion

  }
}
