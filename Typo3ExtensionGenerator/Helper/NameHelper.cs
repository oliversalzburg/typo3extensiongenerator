using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;

namespace Typo3ExtensionGenerator.Helper {
  /// <summary>
  /// Generates names for different parts of the TYPO3 extension.
  /// lowerCamelCase is ENFORCED in all markup!
  /// </summary>
  public static class NameHelper {
    /// <summary>
    /// Generates the class name that must be used to implement the label hooks for this extension.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static string GetLabelHooksImplementationClassName( Extension extension ) {
      return String.Format( "{0}HooksLabelsImplementation", UpperCamelCase( extension.Key ) );
    }

    /// <summary>
    /// Generates the file name that must be used to implement the label hooks for this extension.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static string GetLabelHooksImplementationFileName( Extension extension ) {
      return String.Format( "{0}HooksLabelsImplementation.php", UpperCamelCase( extension.Key ) );
    }

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
    /// Generates the Extbase class name for a given data model.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelFileName( Extension extension, DataModel dataModel ) {
      return String.Format( "{0}.php", UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase domain model repository class name for a given data model repository.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelRepositoryClassName( Extension extension, DataModel dataModel ) {
      return String.Format( "Tx_{0}_Domain_Repository_{1}Repository", UpperCamelCase( extension.Key ), UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase domain model repository class name for a given data model that must be used for a specific implementation.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelRepositoryImplementationClassName( Extension extension, DataModel dataModel ) {
      return String.Format( "{0}{1}RepositoryImplementation", UpperCamelCase( extension.Key ), UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the Extbase class name for a given data model repository.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelRepositoryFileName( Extension extension, DataModel dataModel ) {
      return String.Format( "{0}Repository.php", UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the Extbase class name for a given data model repository implementation.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetExtbaseDomainModelRepositoryImplementationFileName( Extension extension, DataModel dataModel ) {
      return String.Format( "{0}RepositoryImplementation.php", UpperCamelCase( dataModel.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase controller class name for a given plugin.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static string GetExtbaseControllerClassName( Extension extension, IControllerTemplate plugin ) {
      return String.Format( "Tx_{0}_Controller_{1}Controller", UpperCamelCase( extension.Key ), UpperCamelCase( plugin.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase controller class name that must be used to implement a controller for a given plugin.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static string GetExtbaseControllerImplementationClassName( Extension extension, IControllerTemplate plugin ) {
      return String.Format( "{0}{1}ControllerImplementation", UpperCamelCase( extension.Key ), UpperCamelCase( plugin.Name ) );
    }

    /// <summary>
    /// Generates the Extbase controller class name for a given plugin.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static string GetExtbaseControllerFileName( Extension extension, IControllerTemplate plugin ) {
      return String.Format( "{0}Controller.php", UpperCamelCase( plugin.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase controller file name that must be used for the implementation of a controller for a given plugin.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static string GetExtbaseControllerImplementationFileName( Extension extension, IControllerTemplate plugin ) {
      return String.Format( "{0}{1}ControllerImplementation.php", UpperCamelCase( extension.Key ), UpperCamelCase( plugin.Name ) );
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
    /// Generates the ExtBase service class name for a given service.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static string GetExtbaseServiceClassName( Extension extension, Service service ) {
      return String.Format( "Tx_{0}_Service_{1}Service", UpperCamelCase( extension.Key ), UpperCamelCase( service.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase service class name that must be used to implement a service.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static string GetExtbaseServiceImplementationClassName( Extension extension, Service service ) {
      return String.Format( "{0}{1}ServiceImplementation", UpperCamelCase( extension.Key ), UpperCamelCase( service.Name ) );
    }

    /// <summary>
    /// Generates the Extbase service class name for a given service.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static string GetExtbaseServiceFileName( Extension extension, Service service ) {
      return String.Format( "{0}Service.php", UpperCamelCase( service.Name ) );
    }

    /// <summary>
    /// Generates the ExtBase service file name that must be used for the implementation of a service.
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static string GetExtbaseServiceImplementationFileName( Extension extension, Service service ) {
      return String.Format( "{0}{1}ServiceImplementation.php", UpperCamelCase( extension.Key ), UpperCamelCase( service.Name ) );
    }

    /// <summary>
    /// Generates the file name for a Fluid partial.
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public static string GetFluidPartialFileName( Extension subject, DataModel dataModel ) {
      return String.Format( "{0}.html", UpperCamelCase( dataModel.Name ) );
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
      return String.Format( "tx_{0}_domain_model_{1}", extension.Key.Replace( "_", string.Empty ), model.Name.ToLower() );
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
    /// Returns a plugin key. Like userdownloads_stats or news_pi1
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static string GetPluginSignature( Extension extension, Plugin plugin ) {
      return String.Format( "{0}_{1}", extension.Key.Replace( "_", string.Empty ), plugin.Name.ToLower() );
    }

    /// <summary>
    /// Returns a module key. Like userdownloads_import or news_m1
    /// </summary>
    /// <param name="extension"></param>
    /// <param name="module"></param>
    /// <returns></returns>
    public static string GetModuleSignature( Extension extension, Module module ) {
      return String.Format( "{0}_{1}", extension.Key.Replace( "_", string.Empty ), module.Name.ToLower() );
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
