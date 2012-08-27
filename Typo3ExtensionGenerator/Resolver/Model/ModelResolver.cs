using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Model {
  public static class ModelResolver {
    /// <summary>
    /// Resolves the models of an extension from a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The models of the extension</returns>
    public static List<DataModel> Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      IEnumerable<ExtensionParser.ParsedPartial> modelPartials = parsedPartial.Partials.Where( p => p.Keyword == Keywords.DeclareModel );
      if( !modelPartials.Any() ) return null;

      List<DataModel> dataModels = new List<DataModel>();
      foreach( ExtensionParser.ParsedPartial modelPartial in modelPartials ) {
        DataModel dataModel = new DataModel {Name = modelPartial.Parameters};
        dataModels.Add( dataModel );
        if( modelPartial.Partials.Any() ) {
          foreach( ExtensionParser.ParsedPartial dataMember in modelPartial.Partials ) {
            dataModel.Members.Add( new KeyValuePair<string, string>( dataMember.Keyword, dataMember.Parameters ) );
          }
        }
      }

      return dataModels;
    }
  }
}
