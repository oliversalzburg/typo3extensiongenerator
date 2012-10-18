using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates service classes
  /// </summary>
  public class ServiceGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ServiceGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {
      if( null == Subject.Services || !Subject.Services.Any() ) return;

      Log.Info( "Generating services..." );

      foreach( Service service in Subject.Services ) {
        GenerateService( service );
      }
    }

    private void GenerateService( Service service ) {
      string className = NameHelper.GetExtbaseServiceClassName( Subject, service );
      Log.InfoFormat( "Generating service '{0}'...", className );

    }
  }
}
