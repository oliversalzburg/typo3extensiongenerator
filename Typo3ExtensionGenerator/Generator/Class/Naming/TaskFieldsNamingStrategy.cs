﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Task;

namespace Typo3ExtensionGenerator.Generator.Class.Naming {
  /// <summary>
  /// The naming strategy for the additional fields for a scheduler task.
  /// </summary>
  class TaskFieldsNamingStrategy : INamingStrategy {
    /// <summary>
    /// Generate the class name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the given class template.</returns>
    public string GetExtbaseClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is TaskFields );
      return NameHelper.GetTaskFieldsClassName( extension, classTemplate as TaskFields );
    }

    /// <summary>
    /// Generate the file name to be used for the given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the given class template.</returns>
    public string GetExtbaseFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is TaskFields );
      return NameHelper.GetTaskFieldsFileName( extension, classTemplate as TaskFields );
    }

    /// <summary>
    /// Generate the class name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The class name to be used for the implementation of given class template.</returns>
    public string GetExtbaseImplementationClassName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is TaskFields );
      return NameHelper.GetTaskFieldsImplementationClassName( extension, classTemplate as TaskFields );
    }

    /// <summary>
    /// Generate the file name to be used for the implementation of given class template.
    /// </summary>
    /// <param name="extension">The extension this class template is defined in.</param>
    /// <param name="classTemplate">The class template itself.</param>
    /// <returns>The file name to be used for the implementation of given class template.</returns>
    public string GetExtbaseImplementationFileName( Extension extension, IClassTemplate classTemplate ) {
      Debug.Assert( classTemplate is TaskFields );
      return NameHelper.GetTaskFieldsImplementationFileName( extension, classTemplate as TaskFields );
    }

    /// <summary>
    /// A string that should be appended to all method names (like "Action")
    /// </summary>
    public string MethodSuffix { get { return string.Empty; } }

    /// <summary>
    /// If the class should extend a given base class, this should be the string describing the extension.
    /// </summary>
    /// <example>extends Tx_Extbase_MVC_Controller_ActionController</example>
    public string Extends { get { return string.Empty; } }

    /// <summary>
    /// If the class should implement a given interface, this should be the string describing the extension.
    /// </summary>
    /// <example>extends tx_scheduler_AdditionalFieldProvider</example>
    public string Implements { get { return "implements tx_scheduler_AdditionalFieldProvider";  } }
  }
}
