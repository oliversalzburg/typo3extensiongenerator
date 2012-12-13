using Typo3ExtensionGenerator.Resolver.Configuration;

namespace Typo3ExtensionGenerator.Parser.Definitions {
  /// <summary>
  /// Defines the keywords that are used in the TYPO3 Extension Generator grammar
  /// </summary>
  public static class Keywords {
    /// <summary>
    /// Used to pull in another source file at the current location.
    /// </summary>
    public const string PreProcessInclude = "#include";

    /// <summary>
    /// interface - Used to define an interface to a data field.
    /// </summary>
    public const string DefineInterface = "interface";
    
    /// <summary>
    /// as - Used to apply a human-readable name to an object.
    /// </summary>
    public const string Title = "as";

    /// <summary>
    /// description - Gives a description to an entity.
    /// </summary>
    public const string Description = "description";

    /// <summary>
    /// in - Defines a category for the given item.
    /// </summary>
    public const string Category = "in";

    /// <summary>
    /// require - This object requires a certain condition to be met.
    /// </summary>
    public const string Requirement = "require";

    /// <summary>
    /// Used to declare the extension.
    /// </summary>
    public const string DeclareExtension = "extension";

    /// <summary>
    /// is - This element implements an internal type
    /// </summary>
    public const string InternalType = "is";

    /// <summary>
    /// represents - Used to point to a file that implements the interface of a controller.
    /// </summary>
    public const string Implementation = "represents";

    /// <summary>
    /// Directives that are in use in an extension markup
    /// </summary>
    public static class ExtensionDirectives {
      /// <summary>
      /// author - Used to define the name of the author of the extension.
      /// </summary>
      public const string DefineAuthor = "author";

      /// <summary>
      /// authorEmail - Used to define the email address of the author of the extension.
      /// </summary>
      public const string DefineAuthorEmail = "authorEmail";

      /// <summary>
      /// authorCompany - Used to define the name of company of the author of the extension.
      /// </summary>
      public const string DefineAuthorCompany = "authorCompany";

      /// <summary>
      /// state - Defines the state of the extension (alpha/beta/stable)
      /// </summary>
      public const string State = "state";

      /// <summary>
      /// version - Defines the version of the extension (1.2.3)
      /// </summary>
      public const string Version = "version";

      /// <summary>
      /// plugin - Used to declare a new plugin.
      /// </summary>
      public const string DeclarePlugin = "plugin";

      /// <summary>
      /// module - Used to declare a new module.
      /// </summary>
      public const string DeclareModule = "module";

      /// <summary>
      /// model - Used to declare a new data model.
      /// </summary>
      public const string DeclareModel = "model";

      /// <summary>
      /// configure - Used to define a model configuration.
      /// </summary>
      public const string DeclareConfiguration = "configure";

      /// <summary>
      /// repository - Used to define the implementation details of an ExtBase repository.
      /// </summary>
      public const string DeclareRepository = "repository";

      /// <summary>
      /// service - Used to define a service class.
      /// </summary>
      public const string DeclareService = "service";

      /// <summary>
      /// task - Used to define a scheduler task.
      /// </summary>
      public const string DeclareTask = "task";
    }

    /// <summary>
    /// template - Keyword to request a template inside a data model.
    /// </summary>
    public const string DataModelTemplate = "template";

    /// <summary>
    /// Data types
    /// </summary>
    public static class Types {
      /// <summary>
      /// An unsigned integer.
      /// </summary>
      public const string UnsignedInt = "uint";

      /// <summary>
      /// A string
      /// </summary>
      public const string String = "string";

      /// <summary>
      /// A character array
      /// </summary>
      public const string CharacterArray = "char";
    }

    /// <summary>
    /// Directives primarily used in a plugin context.
    /// </summary>
    public static class PluginDirectives {
      /// <summary>
      /// action - Declares an action in a plugin.
      /// </summary>
      public const string Action = "action";

      /// <summary>
      /// listener - A listener is an action that connects to an ExtBase SignalSlot
      /// </summary>
      public const string Listener = "listener";

      /// <summary>
      /// Directives used primarily inside an action context.
      /// </summary>
      public static class ActionDirectives {
        /// <summary>
        /// uncachable - Marks a controller action as uncachable.
        /// </summary>
        public const string Uncachable = "uncachable";

        /// <summary>
        /// host - The ExtBase SignalSlot host.
        /// Used when describing a listener action for an ExtBase SignalSlot
        /// </summary>
        public const string Host = "host";

        /// <summary>
        /// slot - The ExtBase SignalSlot slot.
        /// Used when describing a listener action for an ExtBase SignalSlot
        /// </summary>
        public const string Slot = "slot";
      }
    }

    /// <summary>
    /// Directives that are primarily used in a service context.
    /// </summary>
    public static class ServiceDirectives {
      /// <summary>
      /// fields - Define a class that should be used to describe additional parameters for a scheduler task
      /// </summary>
      public const string AdditionalFields = "fields";
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

      /// <summary>
      /// t3Sortable
      /// </summary>
      public const string T3Sortable = "t3Sortable";
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

      /// <summary>
      /// hide - This configuration is hidden. Users will not be able to create records for this model through the backend.
      /// </summary>
      public const string Hidden = "hide";

      /// <summary>
      /// Directives primarily used inside an interface context
      /// </summary>
      public static class InterfaceDirectives {
        /// <summary>
        /// exclude - This interface element is 'excluded' it can only be edited by certain people.
        /// </summary>
        public const string Exclude = "exclude";

        /// <summary>
        /// through
        /// </summary>
        public const string Representation = "through";

        /// <summary>
        /// default - Used to specify a default value for an interface.
        /// </summary>
        public const string DefaultValue = "default";

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

      /// <summary>
      /// type - Used to define a type in a configuration.
      /// </summary>
      /// <see cref="TypeResolver"/>
      public const string TypeDeclaration = "type";

      /// <summary>
      /// interface - Used to define an interface to a data model member in a configuration.
      /// </summary>
      public const string InterfaceType = "interface";

      /// <summary>
      /// palette - Describes a palette. A palette is a group of interface members that can be dynamically shown/hidden in the BE.
      /// </summary>
      public const string PaletteDeclaration = "palette";
    }
  }
}