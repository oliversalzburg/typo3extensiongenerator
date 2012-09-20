using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Configuration;
using Typo3ExtensionGenerator.Resolver.Extension;
using Typo3ExtensionGenerator.Resolver.Model;
using Typo3ExtensionGenerator.Resolver.Module;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Resolver {
  public static class ExtensionResolver {
    public static Typo3ExtensionGenerator.Model.Extension Resolve( Fragment parsedFragment ) {
      
      string extensionKey = parsedFragment.Header.Substring(
        Keywords.DeclareExtension.Length, parsedFragment.Header.Length - Keywords.DeclareExtension.Length ).Trim();

      Typo3ExtensionGenerator.Model.Extension extension = new Typo3ExtensionGenerator.Model.Extension {
                                                                                                        Key = extensionKey,
                                                                                                        Author = Person.Someone,
                                                                                                        State = "alpha",
                                                                                                        Version = "0.0.1"
                                                                                                      };

      foreach( Fragment subPartial in parsedFragment.Fragments ) {
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

        } else if( subPartial.Keyword == Keywords.ConfigurationDirectives.LabelHook ) {
          extension.LabelHookImplementation = subPartial.Parameters;
        }
      }
      extension.Configurations = ConfigurationResolver.Resolve( parsedFragment );
      extension.Models = ModelResolver.Resolve( parsedFragment );
      extension.Modules = ModuleResolver.Resolve( parsedFragment );
      extension.Plugins = PluginResolver.Resolve( parsedFragment );
      extension.Repositories = RepositoryResolver.Resolve( parsedFragment );
      extension.Requirements = RequirementResolver.Resolve( parsedFragment );

      return extension;
    }
  }
}
