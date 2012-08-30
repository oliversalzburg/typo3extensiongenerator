using System;
using System.IO;
using System.Threading;

namespace Typo3ExtensionGenerator.Helper {
  public static class DirectoryHelper {
    public static void DeleteDirectory( string targetDirectory, bool preserveRoot = false ) {
      string[] files = Directory.GetFiles( targetDirectory );
      string[] dirs = Directory.GetDirectories( targetDirectory );

      foreach( string file in files ) {
        File.SetAttributes( file, FileAttributes.Normal );
        File.Delete( file );
      }

      foreach( string dir in dirs ) {
        DeleteDirectory( dir );
      }

      if( !preserveRoot ) {
        try {
          Directory.Delete( targetDirectory, false );

        } catch( IOException ) {
          Thread.Sleep( 0 );
          Directory.Delete( targetDirectory, false );
        }
      }
    }

    /// <summary>
    /// Recursively copy one directory's contents to another one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <exception cref="ArgumentException">Source or target path does not exist.</exception>
    public static void CopyDirectory( string source, string target ) {
      DirectoryInfo sourcePath = new DirectoryInfo( source );
      DirectoryInfo targetPath = new DirectoryInfo( target );
      if( !sourcePath.Exists ) throw new ArgumentException( "Source path does not exist.", "source" );
      if( !targetPath.Exists ) throw new ArgumentException( "Target path does not exist.", "target" );

      foreach( DirectoryInfo directory in sourcePath.GetDirectories() ) {
        if( ( directory.Attributes & FileAttributes.Hidden ) > 0 ) continue;
        if( directory.Name.StartsWith( "." ) ) continue;
        string newTarget = Path.Combine( target, directory.Name );
        DirectoryInfo newTargetPath = new DirectoryInfo( newTarget );
        if( !newTargetPath.Exists ) newTargetPath.Create();

        CopyDirectory( directory.FullName, newTarget );
      }

      foreach( FileInfo file in sourcePath.GetFiles() ) {
        file.CopyTo( Path.Combine( target, file.Name ), true );
      }
    }
  }
}
