using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
    /// Set a given key/value in the tree hierarchy.
    /// If the given value is NOT enclosed in 'single quotes', they will be added.
    /// </summary>
    /// <param name="path">For example: wizards.RTE.title</param>
    /// <param name="value">For example: 'LLL:EXT:cms/locallang_ttc.xml:bodytext.W.RTE'</param>
    public void Set( string path, string value ) {
      if( value.First() != '\'' && value.Last() != '\'' ) {
        value = '\'' + value + '\'';
      }
      if( value.Last() != '\'' ) value += '\'';

      Debug.Assert( !value.StartsWith( "''" ) );
      Debug.Assert( !value.EndsWith( "''" ) );
      Debug.Assert( value.StartsWith( "'" ) );
      Debug.Assert( value.EndsWith( "'" ) );

      InternalSet( path, value );
    }

    public void Set( string path, int value ) {
      InternalSet( path, value.ToString( CultureInfo.InvariantCulture ) );
    }

    private void InternalSet( string path, string value ) {
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
          string propertyValue = child.Value.Value;
          // For XML format, we ignore quotes
          if( format == Format.Xml ) {
            Regex quoteEnclosed = new Regex( "^'.*?'$" );
            if( quoteEnclosed.IsMatch( propertyValue ) ) {
              propertyValue = propertyValue.Substring( 1, propertyValue.Length - 3 );
            }
          }
          configuration.Append( String.Format( propertyTemplate, child.Key, propertyValue ) );
        }
      }
      return configuration.ToString();
    }

    public string GeneratePropertyArray( Format format ) {
      return GeneratePropertyArray( this, format );
    }
  }
}