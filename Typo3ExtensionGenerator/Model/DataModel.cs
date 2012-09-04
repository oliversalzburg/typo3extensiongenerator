using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// DataModel describes a generic data container inside the extension.
  /// From these models, we'll generate the SQL description, the ExtBase model, ...
  /// </summary>
  [Serializable]
  public class DataModel : IParserResult {
    public class DataModelMember {
      public string Name { get; set; }
      public string Value { get; set; }
      public int Line { get; set; }

      public override string ToString() {
        return string.Format( "{0}: {1}", Name, Value);
      }
    }

    /// <summary>
    /// The name of this data model.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The already existing type in TYPO3 that is implemented through this type
    /// </summary>
    public string InternalType { get; set; }

    /// <summary>
    /// The contained data members.
    /// </summary>
    public List<DataModelMember> Members { get; set; }

    /// <summary>
    /// The keys in this list are names of members, their values are the foreign data models they're referencing.
    /// </summary>
    public Dictionary<string, DataModel> ForeignModels { get; set; }

    /// <summary>
    /// Determine if this model uses a specific data model template
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public bool UsesTemplate( string template ) {
      return Members.Any( m => m.Name == Keywords.DataModelTemplate && m.Value == template );
    }

    public DataModel() {
      Members = new List<DataModelMember>();
      ForeignModels = new Dictionary<string, DataModel>();
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The parsed partial from which this object was generated.
    /// </summary>
    public ExtensionParser.ParsedPartial SourcePartial { get; set; }
    #endregion
  }
}
