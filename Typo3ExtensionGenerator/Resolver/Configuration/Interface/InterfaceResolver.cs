using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration.Interface {
  public static class InterfaceResolver {
    /// <summary>
    /// Resolves the model field interfaces of a data model configurationfrom a ParsedPartial.
    /// </summary>
    /// <param name="parsedPartial">The partially parsed extension.</param>
    /// <returns>The field interfaces of the data model configuration.</returns>
    public static Typo3ExtensionGenerator.Model.Configuration.Interface.Interface Resolve( ExtensionParser.ParsedPartial parsedPartial ) {
      Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface = new Typo3ExtensionGenerator.Model.Configuration.Interface.Interface {Target = parsedPartial.Parameters};
      if( parsedPartial.Partials.Any() ) {
        foreach( ExtensionParser.ParsedPartial setting in parsedPartial.Partials ) {
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
          DisplayTypeResolver.Resolve( parsedPartial, @interface, setting.Value );
        }
      }

      return @interface;
    }
  }
}