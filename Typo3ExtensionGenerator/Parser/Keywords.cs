using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Parser {
  public static class Keywords {
    /// <summary>
    /// interface - Used to define an interface to a data field.
    /// </summary>
    public const string DefineInterface = "interface";
    
    /// <summary>
    /// as - Used to apply a human-readable name to an object.
    /// </summary>
    public const string Title = "as";

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

    public static class PluginDirectives {
      public const string Action = "action";
    }

    /// <summary>
    /// Keywords of data model template definitions.
    /// </summary>
    public static class DataModelTemplates {
      /// <summary>
      /// t3ManagedFields
      /// </summary>
      public const string T3ManagedFields = "t3ManagedFields";

      /// <summary>
      /// t3CommonFields
      /// </summary>
      public const string T3CommonFields = "t3CommonFields";

      /// <summary>
      /// t3VersioningFields
      /// </summary>
      public const string T3VersioningFields = "t3VersioningFields";

      /// <summary>
      /// t3TranslationFields
      /// </summary>
      public const string T3TranslationFields = "t3TranslationFields";
    }

    /// <summary>
    /// Keywords used in data model configurations.
    /// </summary>
    public static class ConfigurationDirectives {
      /// <summary>
      /// label
      /// </summary>
      public const string Label = "label";

      /// <summary>
      /// labelAlt
      /// </summary>
      public const string LabelAlternative = "labelAlt";

      /// <summary>
      /// labelHook
      /// </summary>
      public const string LabelHook = "labelHook";

      /// <summary>
      /// thumbnail
      /// </summary>
      public const string Thumbnail = "thumbnail";

      /// <summary>
      /// searchFields
      /// </summary>
      public const string SearchFields = "searchFields";

      /// <summary>
      /// interfaceInfo
      /// </summary>
      public const string InterfaceInfo = "interfaceInfo";

      public static class InterfaceDirectives {
        /// <summary>
        /// exclude
        /// </summary>
        public const string Exclude = "exclude";

        /// <summary>
        /// through
        /// </summary>
        public const string Representation = "through";

        /// <summary>
        /// from
        /// </summary>
        //public const string Foreign        = "from";
        public static class Representations {
          /// <summary>
          /// input
          /// </summary>
          public const string Textbox = "input";

          /// <summary>
          /// text
          /// </summary>
          public const string TextArea = "text";

          /// <summary>
          /// richtext
          /// </summary>
          public const string RichTextArea = "richtext";

          /// <summary>
          /// group
          /// </summary>
          public const string RecordGroup = "group";

          /// <summary>
          /// select
          /// </summary>
          public const string Dropdown = "select";

          /// <summary>
          /// fileReference
          /// </summary>
          public const string FileReference = "fileReference";
        }
      }

      public const string TypeDeclaration = "type";
      public const string InterfaceType = "interface";

      public const string PaletteDeclaration = "palette";
      public const string InterfacePalette = "interface";
    }
  }
}