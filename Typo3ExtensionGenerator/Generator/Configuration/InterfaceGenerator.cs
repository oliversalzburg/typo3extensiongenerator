using System;
using System.Text;
using Typo3ExtensionGenerator.Generator.Configuration.Interface;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public static class InterfaceGenerator {

    public static string Generate( AbstractGenerator parent, Extension extension, Typo3ExtensionGenerator.Model.Configuration.Interface.Interface subject, SimpleContainer.Format format ) {
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

      parent.WriteVirtual( "Resources/Private/Language/locallang_db.xml", string.Format( "<label index=\"{0}\">{1}</label>", labelLanguageConstant, subject.Title ) );

      // config
      string configuration = string.Empty;

      // Add foreign_table
      if( subject.ParentModel.ForeignModels.ContainsKey( subject.Target ) ) {
        DataModel foreignModel = subject.ParentModel.ForeignModels[ subject.Target ];
        string typename = ( String.IsNullOrEmpty( foreignModel.InternalType ) )
                            ? NameHelper.GetAbsoluteModelName( extension, foreignModel )
                            : foreignModel.InternalType;

        subject.DisplayType.ParentModel = foreignModel;

        if( format == SimpleContainer.Format.PhpArray ) {
          configuration += String.Format(
            propertyTemplate, "foreign_table", "'" + typename + "'" );
          configuration += String.Format(
            propertyTemplate, "foreign_table_where", "'AND " + typename + ".sys_language_uid=0'" );
          configuration += String.Format(
            propertyTemplate, "allowed", "'" + typename + "'" );

        } else {
          configuration += String.Format(
            propertyTemplate, "foreign_table", typename );
          configuration += String.Format(
            propertyTemplate, "foreign_table_where", "AND " + typename + ".sys_language_uid=0" );
          configuration += String.Format(
            propertyTemplate, "allowed", typename );
        }
      }

      if( null != subject.DisplayType ) {
        // Add any additional properties to the configuration
        configuration += DisplayTypeGenerator.GeneratePropertyArray( extension, subject.DisplayType, format );
        
        if( format == SimpleContainer.Format.PhpArray ) {
          configuration += String.Format( propertyTemplate, "type", "'" + subject.DisplayType.Name + "'" );

        } else {
          configuration += String.Format( propertyTemplate, "type", subject.DisplayType.Name );
        }
      } else {
        throw new GeneratorException( string.Format( "No display type given in interface for '{0}'!", subject.Target ), subject.SourceFragment.SourceDocument );
      }

      if( null != subject.DefaultValue ) {
        if( format == SimpleContainer.Format.PhpArray ) {
          configuration += String.Format( propertyTemplate, "default", "'" + subject.DefaultValue + "'" );

        } else {
          configuration += String.Format( propertyTemplate, "default", subject.DefaultValue );
        }
      }

      
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
