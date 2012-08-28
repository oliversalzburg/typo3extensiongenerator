﻿using System;
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
    public static string GetExtbaseClassName( Extension extension, DataModel dataModel ) {
      return String.Format( "Tx_{0}_Domain_Model_{1}", UpperCamelCase( extension.Key ), UpperCamelCase( dataModel.Name ) );
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

    private static string UpperCamelCase( string input ) {
      return input.Substring( 0, 1 ).ToUpper() + Regex.Replace( input.Substring( 1 ), "_(.)", delegate( Match match ) {
                                                                                                return
                                                                                                  match.Groups[ 1 ].
                                                                                                    Captures[ 0 ].Value.
                                                                                                    ToUpper();
                                                                                              } );
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
      return String.Format( "tx_{0}_domain_model_{1}", extension.Key, model.Name );
    }
  }
}
