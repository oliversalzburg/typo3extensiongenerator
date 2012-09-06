using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Typo3ExtensionGenerator.Generator;
using log4net;

namespace Typo3ExtensionGenerator.Resources {
  public static class ResourceHelper {
    private static readonly ILog Log =
      LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Flushes an icon to disk.
    /// </summary>
    /// <param name="name">The name of the icon resource.</param>
    /// <param name="path">The name the flushed icon should have.</param>
    /// <param name="generator"> </param>
    public static void FlushIcon( string name, AbstractGenerator generator, string path  ) {
      Log.InfoFormat( "Flushing '{0}'...", path );
      using(
        Stream resourceStream =
          Assembly.GetExecutingAssembly().GetManifestResourceStream(
            string.Format( "Typo3ExtensionGenerator.Resources.Icons.{0}", name ) ) ) {
        byte[] buffer = new byte[16 * 1024];
        using( MemoryStream ms = new MemoryStream() ) {
          int read;
          while( ( read = resourceStream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
            ms.Write( buffer, 0, read );
          }
          generator.WriteFile( path, ms.ToArray(), DateTime.UtcNow );
        }
      }
    }
  }
}