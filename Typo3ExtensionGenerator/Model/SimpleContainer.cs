using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// An AbstractContainer implementation that uses only strings for storage
  /// </summary>
  public class SimpleContainer : AbstractContainer<Dictionary<string, SimpleContainer>, string> {
    public enum Format {
      PhpArray,
      Xml
    }

    public const string PropertyTemplatePhp = "'{0}' => {1},";
    public const string PropertyTemplateXml = "\n<{0}>{1}</{0}>";

    /// <summary>
    /// Set a given key/value in the tree hierarchy
    /// </summary>
    /// <param name="path">For example: wizards.RTE.title</param>
    /// <param name="value">For example: 'LLL:EXT:cms/locallang_ttc.xml:bodytext.W.RTE'</param>
    public void Set( string path, string value ) {
      string[] pathParts = path.Split( new[] {'.'} );
      SimpleContainer currentContainer = this;
      foreach( string pathPart in pathParts ) {
        if( !currentContainer.Children.ContainsKey( pathPart ) ) {
          currentContainer.Children[ pathPart ] = new SimpleContainer();
        }
        currentContainer = currentContainer.Children[ pathPart ];
      }
      currentContainer.Value = value;
    }

    private string GeneratePropertyArray( SimpleContainer forType, Format format ) {
      string propertyTemplate = ( format == Format.PhpArray ) ? PropertyTemplatePhp : PropertyTemplateXml;

      StringBuilder configuration = new StringBuilder();
      foreach( KeyValuePair<string, SimpleContainer> child in forType.Children ) {
        if( child.Value.Children.Any() ) {
          if( format == Format.PhpArray ) {
            configuration.Append(
              String.Format(
                propertyTemplate, child.Key, "array(" + GeneratePropertyArray( child.Value, format ) + ")" ) );

          } else {
            configuration.Append(
              String.Format(
                propertyTemplate, child.Key, GeneratePropertyArray( child.Value, format ) ) );
          }

        } else {
          configuration.Append( String.Format( propertyTemplate, child.Key, child.Value.Value ) );
        }
      }
      return configuration.ToString();
    }

    public string GeneratePropertyArray( Format format ) {
      return GeneratePropertyArray( this, format );
    }
  }
}