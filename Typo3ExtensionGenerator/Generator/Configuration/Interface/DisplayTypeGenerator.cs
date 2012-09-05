using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration.Interface;

namespace Typo3ExtensionGenerator.Generator.Configuration.Interface {
  public static class DisplayTypeGenerator {
    public static string GeneratePropertyArray( Extension extension, IDisplayType displayType, SimpleContainer.Format format ) {
      
      if( displayType is RecordGroupDisplayType ) {
        RecordGroupDisplayType recordGroupDisplayType = (RecordGroupDisplayType)displayType;
        if( null != displayType.ParentModel ) {
          string absoluteModelName = NameHelper.GetAbsoluteModelName( extension, displayType.ParentModel );
          recordGroupDisplayType.Set( string.Format( "wizards.suggest.{0}.searchWholePhrase", absoluteModelName ), 1 );
          recordGroupDisplayType.Set( string.Format( "wizards.suggest.{0}.maxItemsInResultList", absoluteModelName ), 10 );
          recordGroupDisplayType.Set( string.Format( "wizards.suggest.{0}.addWhere", absoluteModelName ), string.Format( "AND {0}.sys_language_uid=0", absoluteModelName ) );
        }
        recordGroupDisplayType.Set( "internal_type", "db" );
        recordGroupDisplayType.Set( "show_thumbs", recordGroupDisplayType.ShowThumbnails );
        recordGroupDisplayType.Set( "size", recordGroupDisplayType.Lines );
        recordGroupDisplayType.Set( "selectedListStyle", recordGroupDisplayType.SelectedListStyle );
        recordGroupDisplayType.Set( "multiple", recordGroupDisplayType.AllowDuplicates );
        recordGroupDisplayType.Set( "minitems", recordGroupDisplayType.MinItems );
        recordGroupDisplayType.Set( "maxitems", recordGroupDisplayType.MaxItems );
      }

      return displayType.GeneratePropertyArray( format );
    }
  }
}
