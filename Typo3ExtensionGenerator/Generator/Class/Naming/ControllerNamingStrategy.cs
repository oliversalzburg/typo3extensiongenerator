using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Class.Naming {
  /// <summary>
  /// The naming strategy for an ExtBase controller
  /// </summary>
  class ControllerNamingStrategy : INamingStrategy {
    public string GetExtbaseClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Typo3ExtensionGenerator.Model.Plugin.Plugin  );
      return NameHelper.GetExtbaseControllerClassName( extension, classTemplate as Typo3ExtensionGenerator.Model.Plugin.Plugin );
    }

    public string GetExtbaseFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Typo3ExtensionGenerator.Model.Plugin.Plugin  );
      return NameHelper.GetExtbaseControllerFileName( extension, classTemplate as Typo3ExtensionGenerator.Model.Plugin.Plugin );
    }

    public string GetExtbaseImplementationClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Typo3ExtensionGenerator.Model.Plugin.Plugin  );
      return NameHelper.GetExtbaseControllerImplementationClassName( extension, classTemplate as Typo3ExtensionGenerator.Model.Plugin.Plugin );
    }

    public string GetExtbaseImplementationFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is Typo3ExtensionGenerator.Model.Plugin.Plugin  );
      return NameHelper.GetExtbaseControllerImplementationFileName( extension, classTemplate as Typo3ExtensionGenerator.Model.Plugin.Plugin );
    }
  }
}
