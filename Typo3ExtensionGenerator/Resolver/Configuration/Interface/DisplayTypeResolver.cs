using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Resolver.Configuration.Interface {
  public static class DisplayTypeResolver {
    public static void Resolve( ExtensionParser.ParsedPartial parsedPartial, Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface, string displayType ) {
      if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.FileReference == displayType ) {
        SpecializedDisplayType specializedDisplayType = new SpecializedDisplayType {
                                                                                     Name =
                                                                                       Keywords.ConfigurationDirectives.
                                                                                       InterfaceDirectives.
                                                                                       Representations.RecordGroup
                                                                                   };

        specializedDisplayType.Set( "internal_type", "'file_reference'" );
        specializedDisplayType.Set( "allowed", "'*'" );
        specializedDisplayType.Set( "disallowed", "'php'" );
        specializedDisplayType.Set( "size", "5" );
        
        @interface.DisplayType = specializedDisplayType;

      } else if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RecordGroup == displayType ) {
        SpecializedDisplayType specializedDisplayType = new SpecializedDisplayType {
                                                                                     Name =
                                                                                       Keywords.ConfigurationDirectives.
                                                                                       InterfaceDirectives.
                                                                                       Representations.RecordGroup
                                                                                   };

        specializedDisplayType.Set( "internal_type", "db" );
        
        @interface.DisplayType = specializedDisplayType;

      } else if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RichTextArea == displayType ) {
        SpecializedDisplayType specializedDisplayType = new SpecializedDisplayType() {
                                                                                       Name =
                                                                                         Keywords.
                                                                                         ConfigurationDirectives.
                                                                                         InterfaceDirectives.
                                                                                         Representations.TextArea
                                                                                     };
        specializedDisplayType.Set( "wizards.RTE.icon", "wizard_rte2.gif" );
        specializedDisplayType.Set( "wizards.RTE.notNewRecords", 1 );
        specializedDisplayType.Set( "wizards.RTE.RTEonly", 1 );
        specializedDisplayType.Set( "wizards.RTE.script", "wizard_rte.php" );
        specializedDisplayType.Set( "wizards.RTE.title", "LLL:EXT:cms/locallang_ttc.xml:bodytext.W.RTE" );
        specializedDisplayType.Set( "wizards.RTE.type", "script" );

        @interface.DisplayType = specializedDisplayType;

        // Add a new property to the interface itself, to enable rich text editing.
        @interface.Set( "defaultExtras", "'richtext[]'" );

      } else {
        // Could not determine proper display type, use user supplied intput
        @interface.DisplayType = new DisplayType {Name = @interface.DisplayTypeTarget};
      }
    }
  }
}
