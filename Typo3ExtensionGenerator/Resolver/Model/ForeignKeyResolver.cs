using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Model {
  /// <summary>
  /// Resolves foreign key references between data models
  /// </summary>
  public static class ForeignKeyResolver {
    public static void Resolve( List<DataModel> models ) {
      // Iterate over all defined data models
      foreach( DataModel dataModel in models ) {

        // Iterate over all defined fields
        foreach( KeyValuePair<string, string> field in dataModel.Members ) {

          // Check if any of the fields references the name of another model
          DataModel foreignModel = models.SingleOrDefault( m => m.Name == field.Key );
          if( null != foreignModel ) {
            if( !foreignModel.UsesTemplate( Keywords.DataModelTemplates.T3ManagedFields ) ) {
              throw new GeneratorException(
                string.Format(
                  "Referenced foreign model '{0}' does not include TYPO3 Managed Fields template.", foreignModel.Name ) );
            }

            dataModel.ForeignModels.Add( field.Value, foreignModel );
          }
        }
      }
    }
  }
}
