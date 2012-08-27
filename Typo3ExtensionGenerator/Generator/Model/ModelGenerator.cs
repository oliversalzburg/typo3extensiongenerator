using System;
using System.Collections.Generic;
using System.Text;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Generator.Model {
  public class ModelGenerator : AbstractGenerator, IGenerator {
    public ModelGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public string TargetFile {
      get { return "ext_tables.php, ext_tables.sql"; }
    }

    /// <summary>
    /// Retrieves a full model name for a data model.
    /// </summary>
    /// <example>
    /// tx_downloads_domain_model_download
    /// </example>
    /// <param name="extension">The extension this data model belongs to.</param>
    /// <param name="modelName">The name of this data model.</param>
    /// <returns></returns>
    public static string GetAbsoluteModelName( Extension extension, string modelName ) {
      return string.Format( "tx_{0}_domain_model_{1}", extension.Key, modelName );
    }

    public void Generate() {

      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );
      
      WriteFile( "ext_tables.sql", GenerateSql() );
    }

    /// <summary>
    /// Generates the SQL statements for the data model tables
    /// </summary>
    /// <returns></returns>
    private string GenerateSql( ) {
      string result = string.Empty;

      const string template = "CREATE TABLE {0} (\n{1}\n);";
      foreach( DataModel dataModel in Subject.Models ) {
        string modelName = GetAbsoluteModelName( Subject, dataModel.Name );
        string sqlMembers = GenerateSqlMembers( dataModel );
        result += string.Format( template, modelName, sqlMembers ) + "\n";
      }
      return result;
    }

    private string GenerateSqlMembers( DataModel dataModel ) {
      StringBuilder dataMembers = new StringBuilder();
      StringBuilder keys = new StringBuilder();
      foreach( KeyValuePair<string, string> member in dataModel.Members ) {
        // Is this a template request or a normal data member?
        if( Keywords.DataModelTemplate == member.Key ) {
          switch( member.Value ) {
            case Keywords.DataModelTemplates.T3ManagedFields:
              dataMembers.Append( T3ManagedFields.Content + ",\n" );
              keys.Append( T3ManagedFields.Keys + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3CommonFields:
              dataMembers.Append( T3CommonFields.Content + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3VersioningFields:
              dataMembers.Append( T3VersioningFields.Content + ",\n" );
              keys.Append( T3VersioningFields.Keys + ",\n" );
              break;

            case Keywords.DataModelTemplates.T3TranslationFields:
              dataMembers.Append( T3TranslationFields.Content + ",\n" );
              keys.Append( T3TranslationFields.Keys + ",\n" );
              break;

            default:
              throw new ParserException( string.Format( "Data model template '{0}' is unknown", member.Value ) );
          }
        } else {
          dataMembers.Append( string.Format( "{0} {1},\n", member.Value, TypeTranslator.ToSql( member.Key ) ) );
        }
      }
      String dataMembersAndKeys = string.Format(
        "{0}\n{1}", dataMembers.ToString().Substring( 0, dataMembers.Length - 2 ),
        keys.ToString().Substring( 0, keys.Length - 2 ) );

      return dataMembersAndKeys;
    }
    
  }
}
