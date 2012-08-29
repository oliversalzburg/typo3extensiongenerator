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


    public static string Generate( AbstractGenerator parent, Extension extension, Interface subject, SimpleContainer.Format format ) {
      string propertyTemplate = ( format == SimpleContainer.Format.PhpArray )
                                  ? SimpleContainer.PropertyTemplatePhp
                                  : SimpleContainer.PropertyTemplateXml;
      
      StringBuilder interfaceDefinition = new StringBuilder();

      // exclude
      interfaceDefinition.Append( string.Format( propertyTemplate, "exclude", subject.Exclude ? "1" : "0" ) );
      // label
      string labelLanguageConstant = string.Format(
        "{0}.{1}", NameHelper.GetAbsoluteModelName( extension, subject.ParentModel ),
        NameHelper.LowerUnderscoredCase( subject.Target ) );

      if( format == SimpleContainer.Format.PhpArray ) {
        interfaceDefinition.Append(
          string.Format(
            propertyTemplate, "label",
            string.Format(
              "'LLL:EXT:{0}/Resources/Private/Language/locallang_db.xml:{1}'", extension.Key,
              labelLanguageConstant ) ) );
      } else {
        interfaceDefinition.Append(
          string.Format(
            propertyTemplate, "label",
            string.Format(
              "LLL:EXT:{0}/Resources/Private/Language/locallang_db.xml:{1}", extension.Key,
              labelLanguageConstant ) ) );
      }

      parent.WriteFile( "Resources/Private/Language/locallang_db.xml", string.Format( "<label index=\"{0}\">{1}</label>", labelLanguageConstant, subject.Title ), true );

      // config
      string configuration = string.Empty;
      if( format == SimpleContainer.Format.PhpArray ) {
        configuration += String.Format( propertyTemplate, "type", "'" + subject.DisplayType.Name + "'" );

      } else {
        configuration += String.Format( propertyTemplate, "type", subject.DisplayType.Name );
      }

      // Add foreign_table
      if( subject.ParentModel.ForeignModels.ContainsKey( subject.Target ) ) {
        if( format == SimpleContainer.Format.PhpArray ) {
          configuration += String.Format(
            propertyTemplate, "foreign_table",
            "'" + NameHelper.GetAbsoluteModelName( extension, subject.ParentModel.ForeignModels[ subject.Target ] )
            + "'" );
        } else {
          configuration += String.Format(
            propertyTemplate, "foreign_table",
            NameHelper.GetAbsoluteModelName( extension, subject.ParentModel.ForeignModels[ subject.Target ] ) );
        }
      }
      // Add any additional properties to the configuration
      configuration += subject.DisplayType.GeneratePropertyArray( SimpleContainer.Format.PhpArray) ;
      // Trim trailing comma
      configuration = configuration.TrimEnd( new[] {','} );

      // Add the 'config' array to the interface
      if( format == SimpleContainer.Format.PhpArray ) {
        interfaceDefinition.Append( string.Format( propertyTemplate, "config", "array(" + configuration + ")" ) );
      } else {
        interfaceDefinition.Append( string.Format( propertyTemplate, "config", configuration ) );
      }
      // Add any additional parameters set in the interface
      interfaceDefinition.Append( subject.GeneratePropertyArray( SimpleContainer.Format.PhpArray)  );

      string finalInterface = interfaceDefinition.ToString().TrimEnd( new[] {','} );
      return finalInterface;
    }
  }
}
