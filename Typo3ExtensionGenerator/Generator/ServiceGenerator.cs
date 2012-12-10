using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartFormat;
using Typo3ExtensionGenerator.Generator.Class.Naming;
using Typo3ExtensionGenerator.Generator.Helper;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;
using Action = Typo3ExtensionGenerator.Model.Action;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates service classes
  /// </summary>
  public class ServiceGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Constructs a ServiceGenerator
    /// </summary>
    /// <param name="context">The generator context</param>
    /// <param name="extension">The extension.</param>
    public ServiceGenerator( Context context, Extension extension ) : base( context, extension ) {}

    /// <summary>
    /// Generates the services defined in the extension
    /// </summary>
    public void Generate() {
      if( null == Subject.Services || !Subject.Services.Any() ) return;

      Log.Info( "Generating services..." );

      foreach( Service service in Subject.Services ) {
        GenerateService( service );
      }
    }

    /// <summary>
    /// Generates a single service
    /// </summary>
    /// <param name="service">The service that should be generated.</param>
    private void GenerateService( Service service ) {
      ClassProxyGenerator classGenerator = new ClassProxyGenerator( GeneratorContext, Subject );
      classGenerator.GenerateClassProxy( service, new ServiceNamingStrategy(), "Classes/Service/", false );
    }
  }
}
