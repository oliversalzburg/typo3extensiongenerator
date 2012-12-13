using System;
using System.Linq;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Class;
using Typo3ExtensionGenerator.Generator.Class.Naming;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Task;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates scheduler tasks
  /// </summary>
  public class TaskGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Constructs a TaskGenerator
    /// </summary>
    /// <param name="context">The generator context</param>
    /// <param name="extension">The extension.</param>
    public TaskGenerator( Context context, Extension extension ) : base( context, extension ) {}

    /// <summary>
    /// Generates the tasks defined in the extension.
    /// </summary>
    public void Generate() {
      if( null == Subject.Tasks || !Subject.Tasks.Any() ) return;

      Log.Info( "Generating scheduler tasks..." );

      foreach( Task task in Subject.Tasks ) {
        GenerateTask( task );
      }
    }

    /// <summary>
    /// Generates a single task
    /// </summary>
    /// <param name="task">The task that should be generated.</param>
    private void GenerateTask( Task task ) {
      ClassProxyGenerator classGenerator = new ClassProxyGenerator( GeneratorContext, Subject );
      TaskNamingStrategy taskNamingStrategy = new TaskNamingStrategy();
      TaskFieldsNamingStrategy taskFieldsNamingStrategy = new TaskFieldsNamingStrategy();
      
      // Generate the task class itself
      classGenerator.GenerateClassProxy( task, taskNamingStrategy, "Classes/Tasks/", false );

      if( null != task.TaskFields ) {
        classGenerator.GenerateClassProxy( task.TaskFields, taskFieldsNamingStrategy, "Classes/Tasks/", false );

        // Add the autoloader for our fields class
        WriteVirtual(
          "ext_autoload.php",
          String.Format( "'{0}' => $extensionPath . 'Classes/Tasks/{1}',", taskFieldsNamingStrategy.GetExtbaseClassName( Subject, task.TaskFields ).ToLower(), taskFieldsNamingStrategy.GetExtbaseFileName( Subject, task.TaskFields ) ) );
      }

      // Add the autoloader for our class
      WriteVirtual(
        "ext_autoload.php", 
        String.Format( "'{0}' => $extensionPath . 'Classes/Tasks/{1}',", taskNamingStrategy.GetExtbaseClassName( Subject, task ).ToLower(), taskNamingStrategy.GetExtbaseFileName( Subject, task ) ) );

      // Register the scheduler task itself
      string registerTaskTemplate = "$GLOBALS['TYPO3_CONF_VARS']['SC_OPTIONS']['scheduler']['tasks']['{_taskClassName}'] = array(" +
                                    "	'extension'        => '{_extensionKey}'," +
                                    "	'title'            => 'LLL:EXT:{_extensionKey}/Resources/Private/Language/locallang_be.xml:{_taskName}'," +
                                    "	'description'      => 'LLL:EXT:{_extensionKey}/Resources/Private/Language/locallang_be.xml:{_taskDescription}'" +
                                    ( ( null != task.TaskFields ) ? ",'additionalFields' => '" + taskFieldsNamingStrategy.GetExtbaseClassName( Subject, task.TaskFields ).ToLower() + "'" : string.Empty ) +
                                    ");";

      string taskName        = String.Format( "{0}.name",        task.Name.ToLower() );
      string taskDescription = String.Format( "{0}.description", task.Name.ToLower() );

      WriteVirtual( "ext_localconf.php", registerTaskTemplate.FormatSmart( 
        new {
          _extensionKey     = Subject.Key,
          _taskClassName    = taskNamingStrategy.GetExtbaseClassName( Subject, task ), 
          _taskName         = taskName, 
          _taskDescription  = taskDescription
        } 
      ) );

      // Write language constants
      WriteVirtual( "Resources/Private/Language/locallang_be.xml", string.Format( "<label index=\"{0}\">{1}</label>", taskName, task.Title ) );
      WriteVirtual( "Resources/Private/Language/locallang_be.xml", string.Format( "<label index=\"{0}\">{1}</label>", taskDescription, task.Description ) );
    }
  }
}
