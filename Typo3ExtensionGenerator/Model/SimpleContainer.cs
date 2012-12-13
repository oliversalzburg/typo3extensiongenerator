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
    /// <summary>
    /// The format of exported data
    /// </summary>
    public enum Format {
      /// <summary>
      /// Format as a PHP array
      /// </summary>
      PhpArray,

      /// <summary>
      /// Format as XML
      /// </summary>
      XML
    }

    /// <summary>
    /// How properties should be formatted in PHP.
    /// </summary>
    public const string PropertyTemplatePHP = "'{0}' => {1},";
    
    /// <summary>
    /// How properties should be formatted in XML.
    /// </summary>
    public const string PropertyTemplateXML = "\n<{0}>{1}</{0}>";

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

      Debug.Assert( !value.StartsWith( "''" ) );
      Debug.Assert( !value.EndsWith( "''" ) );
      Debug.Assert( value.StartsWith( "'" ) );
      Debug.Assert( value.EndsWith( "'" ) );

      InternalSet( path, value );
    }

    /// <summary>
    /// Set a value in the simple container.
    /// </summary>
    /// <param name="path">The variable to set.</param>
    /// <param name="value">The value of the variable</param>
    public void Set( string path, int value ) {
      InternalSet( path, value.ToString( CultureInfo.InvariantCulture ) );
    }

    /// <summary>
    /// Set a value in the simple container.
    /// </summary>
    /// <param name="path">The variable to set.</param>
    /// <param name="value">The value of the variable</param>
    public void Set( string path, bool value ) {
      InternalSet( path, ( ( value ) ? "1" : "0" ).ToString( CultureInfo.InvariantCulture ) );
    }

    /// <summary>
    /// Set a value in the simple container.
    /// </summary>
    /// <param name="path">The variable to set.</param>
    /// <param name="value">The value of the variable</param>
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

    /// <summary>
    /// Generate the formatted array of for all contained values.
    /// </summary>
    /// <param name="forType">The container that should be exported.</param>
    /// <param name="format">The format that should be exported.</param>
    /// <returns></returns>
    private static string GeneratePropertyArray( SimpleContainer forType, Format format ) {
      string propertyTemplate = ( format == Format.PhpArray ) ? PropertyTemplatePHP : PropertyTemplateXML;

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
          if( format == Format.XML ) {
            Regex quoteEnclosed = new Regex( "^'.*?'$" );
            if( quoteEnclosed.IsMatch( propertyValue ) ) {
              propertyValue = propertyValue.Substring( 1, propertyValue.Length - 2 );
            }
          }
          configuration.Append( String.Format( propertyTemplate, child.Key, propertyValue ) );
        }
      }
      return configuration.ToString();
    }

    /// <summary>
    /// Generate the formatted array of for all contained values.
    /// </summary>
    /// <param name="format">The format that should be exported.</param>
    public string GeneratePropertyArray( Format format ) {
      return GeneratePropertyArray( this, format );
    }
  }
}