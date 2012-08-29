﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Parser {
  public static class Keywords {
    /// <summary>
    /// Used to declare the extension.
    /// </summary>
    public const string DeclareExtension = "extension";

    /// <summary>
    /// Used to define the name of the author of the extension.
    /// </summary>
    public const string DefineAuthor = "author";

    /// <summary>
    /// Used to define the email address of the author of the extension.
    /// </summary>
    public const string DefineAuthorEmail = "authorEmail";

    /// <summary>
    /// Used to define the name of company of the author of the extension.
    /// </summary>
    public const string DefineAuthorCompany = "authorCompany";

    /// <summary>
    /// Used to declare a new plugin.
    /// </summary>
    public const string DeclarePlugin = "plugin";

    /// <summary>
    /// Used to declare a new module.
    /// </summary>
    public const string DeclareModule = "module";

    /// <summary>
    /// Used to declare a new data model.
    /// </summary>
    public const string DeclareModel = "model";

    /// <summary>
    /// Keyword to request a template inside a data model.
    /// </summary>
    public const string DataModelTemplate = "template";

    /// <summary>
    /// Used to define a model configuration.
    /// </summary>
    public const string DeclareConfiguration = "configure";

    public static class Types {
      public const string UnsignedInt = "uint";
      public const string String = "string";
      public const string CharacterArray = "char";
    }

    /// <summary>
    /// Keywords of data model template definitions.
    /// </summary>
    public static class DataModelTemplates {
      public const string T3ManagedFields     = "t3ManagedFields";
      public const string T3CommonFields      = "t3CommonFields";
      public const string T3VersioningFields  = "t3VersioningFields";
      public const string T3TranslationFields = "t3TranslationFields";
    }

    /// <summary>
    /// Keywords used in data model configurations.
    /// </summary>
    public static class ConfigurationDirectives {
      public const string Title = "as";

      public const string Label            = "label";
      public const string LabelAlternative = "labelAlt";
      public const string LabelHook        = "labelHook";

      public const string Thumbnail    = "thumbnail";
      public const string SearchFields = "searchFields";

      public const string InterfaceInfo       = "interfaceInfo";
      public const string InterfaceModelField = "interface";

      public static class InterfaceDirectives {
        public const string Exclude        = "exclude";
        public const string Title          = "as";
        public const string Representation = "through";

        public const string Foreign        = "from";

        public static class Representations {
          public const string Dropdown = "select";
        }
      }

      public const string TypeDeclaration = "type";
      public const string InterfaceType   = "interface";

      public const string PaletteDeclaration = "palette";
      public const string InterfacePalette   = "interface";
    }
  }
}
