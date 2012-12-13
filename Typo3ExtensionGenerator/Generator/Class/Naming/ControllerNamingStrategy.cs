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
    /// <summary>
    /// Generate the class name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the given class template.</returns>
    public string GetExtbaseClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is IControllerTemplate  );
      return NameHelper.GetExtbaseControllerClassName( extension, classTemplate as IControllerTemplate );
    }

    /// <summary>
    /// Generate the file name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the given class template.</returns>
    public string GetExtbaseFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is IControllerTemplate  );
      return NameHelper.GetExtbaseControllerFileName( extension, classTemplate as IControllerTemplate );
    }

    /// <summary>
    /// Generate the class name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the implementation of given class template.</returns>
    public string GetExtbaseImplementationClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is IControllerTemplate  );
      return NameHelper.GetExtbaseControllerImplementationClassName( extension, classTemplate as IControllerTemplate );
    }

    /// <summary>
    /// Generate the file name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the implementation of given class template.</returns>
    public string GetExtbaseImplementationFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is IControllerTemplate  );
      return NameHelper.GetExtbaseControllerImplementationFileName( extension, classTemplate as IControllerTemplate );
    }

    /// <summary>
    /// A string that should be appended to all method names (like "Action")
    /// </summary>
    public string MethodSuffix { get { return "Action"; } }

    /// <summary>
    /// If the class should extend a given base class, this should be the string describing the extension.
    /// </summary>
    /// <example>extends Tx_Extbase_MVC_Controller_ActionController</example>
    public string BaseClassExtension { get { return "extends Tx_Extbase_MVC_Controller_ActionController"; } }
  }
}
