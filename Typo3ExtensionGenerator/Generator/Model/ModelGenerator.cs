using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator.Model.Templates;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Generator.Model {
  public class ModelGenerator : AbstractGenerator, IGenerator {
    public ModelGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      WriteFile( "ext_tables.sql", GenerateSql() );

      foreach( DataModel dataModel in Subject.Models ) {
        const string path = "Classes/Domain/Model";
        string content = GenerateModelFile( dataModel );
        
        string targetFilename = Path.Combine( path, NameHelper.GetExtbaseFileName( Subject, dataModel ) );
        Console.WriteLine( string.Format( "Generating '{0}'...", targetFilename ) );
        WritePhpFile( targetFilename, content );
      }
    }

    private string GenerateModelFile( DataModel dataModel ) {
      const string template =
        "class {1} extends Tx_Extbase_DomainObject_AbstractEntity {{\n" +
        "{0}" +
        "}}";
        

      StringBuilder dataMembers = new StringBuilder();
      foreach( KeyValuePair<string, string> member in dataModel.Members ) {
        dataMembers.Append( string.Format( "  protected ${0};\n", member.Value ) );
      }

      return string.Format( template, dataMembers, NameHelper.GetExtbaseDomainModelClassName( Subject, dataModel ) );
    }

    /// <summary>
    /// Generates the SQL statements for the data model tables
    /// </summary>
    /// <returns></returns>
    private string GenerateSql( ) {
      string result = string.Empty;

      Console.WriteLine( "Generating SQL tables..." );

      const string template = "CREATE TABLE {0} (\n{1}\n);";
      foreach( DataModel dataModel in Subject.Models ) {
        string modelName = NameHelper.GetAbsoluteModelName( Subject, dataModel );
        Console.WriteLine( "Generating SQL table '{0}'...", modelName );

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
              throw new GeneratorException( string.Format( "Data model template '{0}' is unknown", member.Value ) );
          }
        } else {
          if( dataModel.ForeignModels.ContainsKey( member.Key ) ) {
            // For a foreign key, we just insert the default uint
              dataMembers.Append(
              string.Format(
                "{0} {1},\n", NameHelper.GetSqlColumnName( Subject, member.Value ), TypeTranslator.ToSql( Keywords.Types.UnsignedInt) ) );

          } else if( TypeTranslator.CanTranslate( member.Key ) ) {
            // If it is a POD type, just translate it
            dataMembers.Append(
              string.Format(
                "{0} {1},\n", NameHelper.GetSqlColumnName( Subject, member.Value ), TypeTranslator.ToSql( member.Key ) ) );
          
          } else {
            throw new GeneratorException( string.Format( "Data model field type '{0}' is unknown", member.Key ) );
          }
        }
      }
      String dataMembersAndKeys = string.Format(
        "{0},\n{1}", dataMembers.ToString().Substring( 0, dataMembers.Length - 2 ),
        keys.ToString().Substring( 0, keys.Length - 2 ) );

      return dataMembersAndKeys;
    }
    
  }
}
