using System.Linq;
using Typo3ExtensionGenerator.Generator.Class;
using Typo3ExtensionGenerator.Generator.Class.Naming;
using Typo3ExtensionGenerator.Model;
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
      classGenerator.GenerateClassProxy( task, new TaskNamingStrategy(), "Classes/Tasks/", false );
    }
  }
}
