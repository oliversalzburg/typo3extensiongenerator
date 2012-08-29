using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// An AbstractContainer implementation that uses only strings for storage
  /// </summary>
  public class SimpleContainer : AbstractContainer<Dictionary<string, SimpleContainer>, string> {
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

    private string GeneratePropertyArray( SimpleContainer forType ) {
      const string propertyTemplate = "'{0}' => {1},";
      StringBuilder configuration = new StringBuilder();
      foreach( KeyValuePair<string, SimpleContainer> child in forType.Children ) {
        if( child.Value.Children.Any() ) {
          configuration.Append( String.Format( propertyTemplate, child.Key, "array(" + GeneratePropertyArray( child.Value ) + ")" ) );
        } else {
          configuration.Append( String.Format( propertyTemplate, child.Key, child.Value.Value ) );
        }
      }
      return configuration.ToString();
    }

    public string GeneratePropertyArray() {
      return GeneratePropertyArray( this );
    }
  }

  public class AbstractContainer<TCollectionType,TMemberType> where TCollectionType : new() {
    public TCollectionType Children { get; set; }

    public string Name { get; set; }
    public TMemberType Value { get; set; }

    public AbstractContainer() {
      Children = new TCollectionType();
    }
  }
}
