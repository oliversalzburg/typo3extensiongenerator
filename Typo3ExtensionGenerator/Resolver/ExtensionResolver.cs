using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Resolver.Configuration;
using Typo3ExtensionGenerator.Resolver.Extension;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Module;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Resolver {
  public static class ExtensionResolver {
    public static Typo3ExtensionGenerator.Model.Extension Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      
      string extensionKey = parsedPartial.Header.Substring(
        Keywords.DeclareExtension.Length, parsedPartial.Header.Length - Keywords.DeclareExtension.Length ).Trim();

      Typo3ExtensionGenerator.Model.Extension extension = new Typo3ExtensionGenerator.Model.Extension {
                                                                                                        Key = extensionKey,
                                                                                                        Author = Person.Someone,
                                                                                                        State = "alpha",
                                                                                                        Version = "0.0.1"
                                                                                                      };

      foreach( ExtensionParser.ParsedPartial subPartial in parsedPartial.Partials ) {
        if( subPartial.Keyword == Keywords.ExtensionDirectives.DefineAuthor ) {
          extension.Author.Name = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.ExtensionDirectives.DefineAuthorCompany ) {
          extension.Author.Company = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.ExtensionDirectives.DefineAuthorEmail ) {
          extension.Author.Email = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.Category ) {
          extension.Category = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.ExtensionDirectives.Description ) {
          extension.Description = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.Title ) {
          extension.Title = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.ExtensionDirectives.State ) {
          extension.State = subPartial.Parameters;
        } else if( subPartial.Keyword == Keywords.ExtensionDirectives.Version ) {
          extension.Version = subPartial.Parameters;
        }
      }
      extension.Configurations = ConfigurationResolver.Resolve( parsedPartial );
      extension.Models = ModelResolver.Resolve( parsedPartial );
      extension.Modules = ModuleResolver.Resolve( parsedPartial );
      extension.Plugins = PluginResolver.Resolve( parsedPartial );
      extension.Repositories = RepositoryResolver.Resolve( parsedPartial );
      extension.Requirements = RequirementResolver.Resolve( parsedPartial );

      return extension;
    }
  }
}
