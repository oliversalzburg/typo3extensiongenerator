using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model.Configuration.Interface;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;

namespace Typo3ExtensionGenerator.Resolver.Configuration.Interface {
  public static class DisplayTypeResolver {
    public static void Resolve( Fragment parsedFragment, Typo3ExtensionGenerator.Model.Configuration.Interface.Interface @interface, string displayType ) {
      if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.FileReference == displayType ) {
        SpecializedDisplayType specializedDisplayType = new SpecializedDisplayType {
                                                                                     Name = Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RecordGroup,
                                                                                     SourceFragment = parsedFragment
                                                                                   };

        specializedDisplayType.Set( "internal_type", "file_reference" );
        specializedDisplayType.Set( "allowed", "*" );
        specializedDisplayType.Set( "disallowed", "php" );
        specializedDisplayType.Set( "size", 5 );
        specializedDisplayType.Set( "maxitems", 99 );
        
        // If this field requires anything, it should have at least 1 item.
        if( @interface.Settings.Any( s => s.Key == Keywords.Requirement ) ) {
          specializedDisplayType.Set( "minitems", 1 );
        } else {
          specializedDisplayType.Set( "minitems", 0 );
        }
        
        @interface.DisplayType = specializedDisplayType;

      } else if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RecordGroup == displayType ) {
        RecordGroupDisplayType recordGroupDisplayType = new RecordGroupDisplayType {
                                                                                     Name = Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RecordGroup,
                                                                                     SourceFragment = parsedFragment
                                                                                   };

        // Show records next to select box
        recordGroupDisplayType.ShowThumbnails = true;
        // Increase list length
        recordGroupDisplayType.Lines = 10;
        // Increase list width
        recordGroupDisplayType.SelectedListStyle = "width:400px";
        // Don't allow duplicates
        recordGroupDisplayType.AllowDuplicates = false;
        // Set item limit
        recordGroupDisplayType.MaxItems = 99;

        // If this field requires anything, it should have at least 1 item.
        if( @interface.Settings.Any( s => s.Key == Keywords.Requirement ) ) {
          recordGroupDisplayType.MinItems = 1;
        } else {
          recordGroupDisplayType.MinItems = 0;
        }

        // The model type for the suggest wizard is resolved during generation later.
        recordGroupDisplayType.Set( "wizards.RTE.type", "suggest" );
        
        @interface.DisplayType = recordGroupDisplayType;

      } else if( Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.RichTextArea == displayType ) {
        SpecializedDisplayType specializedDisplayType = new SpecializedDisplayType() {
                                                                                       Name = Keywords.ConfigurationDirectives.InterfaceDirectives.Representations.TextArea,
                                                                                       SourceFragment = parsedFragment
                                                                                     };

        specializedDisplayType.Set( "wizards.RTE.icon", "wizard_rte2.gif" );
        specializedDisplayType.Set( "wizards.RTE.notNewRecords", 1 );
        specializedDisplayType.Set( "wizards.RTE.RTEonly", 1 );
        specializedDisplayType.Set( "wizards.RTE.script", "wizard_rte.php" );
        specializedDisplayType.Set( "wizards.RTE.title", "LLL:EXT:cms/locallang_ttc.xml:bodytext.W.RTE" );
        specializedDisplayType.Set( "wizards.RTE.type", "script" );

        @interface.DisplayType = specializedDisplayType;

        // Add a new property to the interface itself, to enable rich text editing.
        @interface.Set( "defaultExtras", "richtext[]" );

      } else {
        // Could not determine proper display type, use user supplied intput
        @interface.DisplayType = new DisplayType {Name = @interface.DisplayTypeTarget};
      }
    }
  }
}
