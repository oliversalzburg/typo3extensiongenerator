using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Configuration.Interface {
  public static class InterfaceResolver {
    /// <summary>
    /// Resolves the model field interfaces of a data model configurationfrom a ParsedPartial.
    /// </summary>
    /// <param name="parsedFragment">The partially parsed extension.</param>
    /// <returns>The field interfaces of the data model configuration.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Interface.Interface Resolve( Fragment parsedFragment ) {
      Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface =
        new Typo3ExtensionGenerator.Model.Configuration.Interface.Interface {
                                                                              Target = parsedFragment.Parameters, SourceFragment = parsedFragment
                                                                            };

      if( parsedFragment.Fragments.Any() ) {
        foreach( Fragment setting in parsedFragment.Fragments ) {
          @interface.Settings.Add( new KeyValuePair<string, string>( setting.Keyword, setting.Parameters ) );
        }
      }

      // Parse any settings we know
      for( int settingIndex = 0; settingIndex < @interface.Settings.Count; settingIndex++ ) {
        KeyValuePair<string, string> setting = @interface.Settings[ settingIndex ];
        if( setting.Key == Keywords.ConfigurationDirectives.InterfaceDirectives.Exclude ) {
          @interface.Exclude = ParseHelper.ParseBool( setting.Value );

        } else if( setting.Key == Keywords.Title ) {
          @interface.Title = setting.Value;

        } else if( setting.Key == Keywords.ConfigurationDirectives.InterfaceDirectives.Representation ) {
          @interface.DisplayTypeTarget = setting.Value;
          DisplayTypeResolver.Resolve( parsedFragment, @interface, setting.Value );
        }
      }

      return @interface;
    }
  }
}