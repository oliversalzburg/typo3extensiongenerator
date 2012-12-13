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
  /// <summary>
  /// Resolves an extension from markup.
  /// </summary>
  public static class ExtensionResolver {
    /// <summary>
    /// Resolves an extension model from a parsed fragment.
    /// </summary>
    /// <param name="parsedFragment"></param>
    /// <returns></returns>
    public static Typo3ExtensionGenerator.Model.Extension Resolve( Fragment parsedFragment ) {
      
      string extensionKey = parsedFragment.Header.Substring(
        Keywords.DeclareExtension.Length, parsedFragment.Header.Length - Keywords.DeclareExtension.Length ).Trim();

      Typo3ExtensionGenerator.Model.Extension extension = new Typo3ExtensionGenerator.Model.Extension {
                                                                                                        Key     = extensionKey,
                                                                                                        Author  = Person.Someone,
                                                                                                        State   = "alpha",
                                                                                                        Version = "0.0.1"
                                                                                                      };

      foreach( Fragment extensionFragment in parsedFragment.Fragments ) {
        if( extensionFragment.Keyword == Keywords.ExtensionDirectives.DefineAuthor ) {
          extension.Author.Name = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.ExtensionDirectives.DefineAuthorCompany ) {
          extension.Author.Company = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.ExtensionDirectives.DefineAuthorEmail ) {
          extension.Author.Email = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.Category ) {
          extension.Category = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.Description ) {
          extension.Description = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.Title ) {
          extension.Title = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.ExtensionDirectives.State ) {
          extension.State = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.ExtensionDirectives.Version ) {
          extension.Version = extensionFragment.Parameters;

        } else if( extensionFragment.Keyword == Keywords.ConfigurationDirectives.LabelHook ) {
          extension.LabelHookImplementation = extensionFragment.Parameters;
        }
      }
      extension.Configurations = ConfigurationResolver.Resolve( parsedFragment );
      extension.Models         = ModelResolver.Resolve( parsedFragment );
      extension.Modules        = ModuleResolver.Resolve( parsedFragment );
      extension.Plugins        = PluginResolver.Resolve( parsedFragment );
      extension.Repositories   = RepositoryResolver.Resolve( parsedFragment );
      extension.Requirements   = RequirementResolver.Resolve( parsedFragment );
      extension.Services       = ServiceResolver.Resolve( parsedFragment );
      extension.Tasks          = TaskResolver.Resolve( parsedFragment );

      return extension;
    }
  }
}
