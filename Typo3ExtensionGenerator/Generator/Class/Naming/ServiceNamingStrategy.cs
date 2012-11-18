using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Class.Naming {
  /// <summary>
  /// The naming strategy for an ExtBase service.
  /// </summary>
  class ServiceNamingStrategy : INamingStrategy {
    public string GetExtbaseClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Service );
      return NameHelper.GetExtbaseServiceClassName( extension, classTemplate as Service );
    }

    public string GetExtbaseFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Service );
      return NameHelper.GetExtbaseServiceFileName( extension, classTemplate as Service );
    }

    public string GetExtbaseImplementationClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Service );
      return NameHelper.GetExtbaseServiceImplementationClassName( extension, classTemplate as Service );
    }

    public string GetExtbaseImplementationFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Service );
      return NameHelper.GetExtbaseServiceImplementationFileName( extension, classTemplate as Service );
    }
  }
}
