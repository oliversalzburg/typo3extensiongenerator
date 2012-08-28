using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Helper {
  /// <summary>
  /// Generates names for different parts of the TYPO3 extension.
  /// lowerCamelCase is ENFORCED in all markup!
  /// </summary>
  public static class NameHelper {
    /// <summary>
    /// Generates the ExtBase domain model class name for a given data model.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelClassName( Extension extension, DataModel dataModel ) {
      return String.Format( "Tx_{0}_Domain_Model_{1}", UpperCamelCase( extension.Key ), UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the name of a class that will be used to supply hooks.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    public static string GetExtbaseHookClassName( Extension extension, string category ) {
      return String.Format( "Tx_{0}_Hooks_{1}", UpperCamelCase( extension.Key ), UpperCamelCase( category ) );
    }

    /// <summary>
    /// Generates the Extbase class name for a given data model.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseFileName( Extension extension, DataModel dataModel ) {
      return String.Format( "{0}.php", UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Retrieves a full model name for a data model.
    /// </summary>
    /// <example>
    /// tx_downloads_domain_model_download
    /// </example>
    /// <param name="extension">The extension this data model belongs to.</param>
    /// <param name="model">The data model.</param>
    /// <returns></returns>
    public static string GetAbsoluteModelName( Extension extension, DataModel model ) {
      return String.Format( "tx_{0}_domain_model_{1}", extension.Key, LowerUnderscoredCase( model.Name ) );
    }

    /// <summary>
    /// Returns the name of a property as it should appear in a SQL column name.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static string GetSqlColumnName( Extension extension, string column ) {
      return LowerUnderscoredCase( column );
    }

    /// <summary>
    /// Converts a lowerCamelCase string to UpperCamelCase
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string UpperCamelCase( string input ) {
      return input.Substring( 0, 1 ).ToUpper()
             + Regex.Replace( input.Substring( 1 ), "_(.)", match => match.Groups[ 1 ].Captures[ 0 ].Value.ToUpper() );
    }

    /// <summary>
    /// Converts a lowerCamelCase string to lower_underscored_case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string LowerUnderscoredCase( string input ) {
      return Regex.Replace( input, "([A-Z])", match => "_" + match.Groups[ 1 ].Captures[ 0 ].Value.ToLower() );
    }
  }
}
