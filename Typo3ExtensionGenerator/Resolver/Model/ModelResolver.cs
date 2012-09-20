using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Model {
  public static class ModelResolver {
    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    public static List<DataModel> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> modelPartials = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareModel );
      if( !modelPartials.Any() ) return null;

      List<DataModel> dataModels = new List<DataModel>();
      foreach( Fragment modelPartial in modelPartials ) {
        DataModel dataModel = new DataModel {Name = modelPartial.Parameters, SourceFragment = modelPartial};
        dataModels.Add( dataModel );
        if( modelPartial.Fragments.Any() ) {
          foreach( Fragment dataMember in modelPartial.Fragments ) {
            if( dataMember.Keyword == Keywords.InternalType ) {
              dataModel.InternalType = dataMember.Parameters;

            } else {
              dataModel.Members.Add(
                new DataModel.DataModelMember {Name = dataMember.Keyword, Value = dataMember.Parameters} );
            }
          }
        }
      }

      // Resolve any foreign key references
      ForeignKeyResolver.Resolve( dataModels );

      return dataModels;
    }
  }
}
