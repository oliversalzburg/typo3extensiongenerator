using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Class.Naming {
  /// <summary>
  /// Describes a strategy to generate names for classes and files for a specific type of ExtBase entity (or anything else that fits the pattern).
  /// </summary>
  interface INamingStrategy {
    /// <summary>
    /// Generate the class name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the given class template.</returns>
    string GetExtbaseClassName( Extension extension, IClassTemplate classTemplate );

    /// <summary>
    /// Generate the file name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the given class template.</returns>
    string GetExtbaseFileName( Extension extension, IClassTemplate classTemplate );

    /// <summary>
    /// Generate the class name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the implementation of given class template.</returns>
    string GetExtbaseImplementationClassName( Extension extension, IClassTemplate classTemplate );

    /// <summary>
    /// Generate the file name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the implementation of given class template.</returns>
    string GetExtbaseImplementationFileName( Extension extension, IClassTemplate classTemplate );

    /// <summary>
    /// A string that should be appended to all method names (like "Action")
    /// </summary>
    string MethodSuffix { get; }

    /// <summary>
    /// If the class should extend a given base class, this should be the string describing the extension.
    /// </summary>
    /// <example>extends Tx_Extbase_MVC_Controller_ActionController</example>
    string BaseClassExtension { get; }
  }
}
