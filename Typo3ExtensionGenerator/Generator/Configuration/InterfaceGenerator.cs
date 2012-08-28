using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public static class InterfaceGenerator {
    public static string Generate( Extension extension, Interface subject ) {
      const string propertyTemplate = "'{0}' => {1},";
      
      StringBuilder interfaceDefinition = new StringBuilder();

      // exclude
      interfaceDefinition.Append( string.Format( propertyTemplate, "exclude", subject.Exclude ? "1" : "0" ) );
      // label
      interfaceDefinition.Append(
        string.Format(
          propertyTemplate, "label",
          string.Format(
            "'LLL:EXT:{0}/Resources/Private/Language/locallang_db.xml:{1}.{2}'", extension.Key,
            NameHelper.GetAbsoluteModelName( extension, subject.ParentModel ),
            NameHelper.LowerUnderscoredCase( subject.Target ) ) ) );

      string configuration = string.Empty;
      configuration += String.Format( propertyTemplate, "type", "'" + subject.DisplayType + "'" );
      configuration = configuration.TrimEnd( new[] {','} );

      interfaceDefinition.Append( string.Format( propertyTemplate, "config", "array(" + configuration + ")" ) );

      string finalInterface = interfaceDefinition.ToString().TrimEnd( new[] {','} );
      return finalInterface;
    }
  }
}
