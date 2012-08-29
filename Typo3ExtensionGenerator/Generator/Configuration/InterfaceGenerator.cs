using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public static class InterfaceGenerator {
    public static string Generate( AbstractGenerator parent, Extension extension, Interface subject ) {
      const string propertyTemplate = "'{0}' => {1},";
      
      StringBuilder interfaceDefinition = new StringBuilder();

      // exclude
      interfaceDefinition.Append( string.Format( propertyTemplate, "exclude", subject.Exclude ? "1" : "0" ) );
      // label
      string labelLanguageConstant = string.Format(
        "{0}.{1}", NameHelper.GetAbsoluteModelName( extension, subject.ParentModel ),
        NameHelper.LowerUnderscoredCase( subject.Target ) );

      interfaceDefinition.Append(
        string.Format(
          propertyTemplate, "label",
          string.Format(
            "'LLL:EXT:{0}/Resources/Private/Language/locallang_db.xml:{1}'", extension.Key,
            labelLanguageConstant ) ) );
      parent.WriteFile( "Resources/Private/Language/locallang_db.xml", string.Format( "<label index=\"{0}\">{1}</label>", labelLanguageConstant, subject.Title ), true );

      // config
      string configuration = string.Empty;
      configuration += String.Format( propertyTemplate, "type", "'" + subject.DisplayType.Name + "'" );
      // Add foreign_table
      if( subject.ParentModel.ForeignModels.ContainsKey( subject.Target ) ) {
        configuration += String.Format(
          propertyTemplate, "foreign_table",
          "'" + NameHelper.GetAbsoluteModelName( extension, subject.ParentModel.ForeignModels[ subject.Target ] ) + "'" );
      }
      // Add any additional properties to the configuration
      configuration += subject.DisplayType.GeneratePropertyArray();
      // Trim trailing comma
      configuration = configuration.TrimEnd( new[] {','} );

      // Add the 'config' array to the interface
      interfaceDefinition.Append( string.Format( propertyTemplate, "config", "array(" + configuration + ")" ) );
      // Add any additional parameters set in the interface
      interfaceDefinition.Append( subject.GeneratePropertyArray() );

      string finalInterface = interfaceDefinition.ToString().TrimEnd( new[] {','} );
      return finalInterface;
    }
  }
}
