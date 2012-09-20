using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Model {
  /// <summary>
  /// Resolves foreign key references between data models
  /// </summary>
  public static class ForeignKeyResolver {
    /// <summary>
    /// Resolves references to foreign data models
    /// </summary>
    /// <param name="models">The data models in which the references should be resolved.</param>
    /// <param name="additional">Additional data models to search in for targets. Data models in this collection will not be modified.</param>
    public static void Resolve( List<DataModel> models, List<DataModel> additional = null ) {
      if( null == additional ) additional = new List<DataModel>();

      // Iterate over all defined data models
      foreach( DataModel dataModel in models ) {

        // Iterate over all defined fields
        foreach( DataModel.DataModelMember field in dataModel.Members ) {

          string searchTerm = field.Name;

          // Check if this is an array of references 
          Regex isArray = new Regex( @"\w+\[\d*\]" );
          if( isArray.IsMatch( searchTerm ) ) {
            // We only care about the type, so we cut that part off the search term
            searchTerm = searchTerm.Substring( 0, searchTerm.IndexOf( '[' ) );
          }

          // Check if any of the fields references the name of another model
          DataModel foreignModel = additional.Concat( models ).SingleOrDefault( m => m.Name == searchTerm );
          if( null != foreignModel ) {
            // Does the referenced type implement the "TYPO3 managed fields"? If not, this a problem (foreign fields are referenced through the uid field that is part of those fields).
            // If the type is "internally implemented" we ignore this (it is assumed that the internal type implements the field).
            if( !foreignModel.UsesTemplate( Keywords.DataModelTemplates.T3ManagedFields ) && string.IsNullOrEmpty( foreignModel.InternalType ) ) {
              throw new GeneratorException(
                string.Format(
                  "Referenced foreign model '{0}' does not include TYPO3 Managed Fields template.", foreignModel.Name ),
                dataModel.SourceFragment.SourceDocument );
            }

            // Store the foreign model for this field
            dataModel.ForeignModels.Add( field.Value, foreignModel );
          }
        }
      }
    }
  }
}
