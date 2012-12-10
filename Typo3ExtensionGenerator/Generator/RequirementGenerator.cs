using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Merges user-specified files that were marked as requirements.
  /// </summary>
  public class RequirementGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Constructs a new RequirementGenerator.
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="extension">The extension.</param>
    public RequirementGenerator( Context context, Extension extension ) : base( context, extension ) {}

    /// <summary>
    /// Pulls in all requirements defined in the extension.
    /// </summary>
    public void Generate() {
      if( null == Subject.Requirements || !Subject.Requirements.Any() ) return;

      foreach( Requirement requirement in Subject.Requirements ) {
        foreach( Requirement.RequiredFile file in requirement.Files ) {
          DateTime lastWriteTimeUtc = new FileInfo( file.FullSourceName ).LastWriteTimeUtc;
          byte[] bytes = File.ReadAllBytes( file.FullSourceName );
          Log.InfoFormat( "Merging required file '{0}'...", file.RelativeTargetName );
          WriteFile( file.RelativeTargetName, bytes, lastWriteTimeUtc );
        }
      }
    }
  }
}
