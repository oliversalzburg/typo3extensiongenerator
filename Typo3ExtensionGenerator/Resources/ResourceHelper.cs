using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Typo3ExtensionGenerator.Resources {
  public static class ResourceHelper {
    /// <summary>
    /// Flushes an icon to disk.
    /// </summary>
    /// <param name="name">The name of the icon resource.</param>
    /// <param name="targetDirectory">The target output directory of the extension.</param>
    /// <param name="path">The name the flushed icon should have.</param>
    public static void FlushIcon( string name, string targetDirectory, string path ) {
      using( Stream resourceStream =  Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format( "Typo3ExtensionGenerator.Resources.Icons.{0}", name ))) {
        Debug.Assert( resourceStream != null, "resourceStream != null" );
        string absolutePath = Path.Combine( targetDirectory, path );
        Directory.CreateDirectory( new FileInfo( absolutePath ).DirectoryName );
        Console.WriteLine( "Flushing {0}...", path );
        using( Stream fileStream = new FileStream( absolutePath, FileMode.Create ) ) {
          byte[] buffer = new byte[8 * 1024];
          int length;
          while( ( length = resourceStream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
            fileStream.Write( buffer, 0, length );
          }
        }
      } 
    }
  }
}
