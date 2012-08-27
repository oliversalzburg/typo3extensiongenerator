using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator {
  public abstract class AbstractGenerator {

    /// <summary>
    /// The directory where our generated extension should be placed.
    /// </summary>
    protected string OutputDirectory { get; set; }

    /// <summary>
    /// The extension definition we're working on.
    /// </summary>
    protected Extension Subject { get; set; }

    /// <summary>
    /// Provides a virtual file system for files that need to be written by multiple generators.
    /// The results will be flushed when generation has completed.
    /// </summary>
    protected static Dictionary<string,StringBuilder> VirtualFileSystem = new Dictionary<string, StringBuilder>();

    protected AbstractGenerator( string outputDirectory, Extension subject ) {
      OutputDirectory = outputDirectory;
      Subject         = subject;
    }

    public void WriteFile( string filename, string content, bool useVirtual = false ) {
      string targetFilename = Path.Combine( OutputDirectory, filename );

      if( useVirtual ) {
        WriteVirtual( targetFilename, content );

      } else {
        File.WriteAllText( targetFilename, content, Encoding.UTF8 );
      }
    }

    public void WritePhpFile( string filename, string content, bool useVirtual = false ) {
      string fileContent = string.Format( "<?php\n{0}\n?>", content );
      WriteFile( filename, fileContent );
    }

    private void WriteVirtual( string filename, string content ) {
      if( !VirtualFileSystem.ContainsKey( filename ) ) {
        VirtualFileSystem[ filename ] = new StringBuilder();
      }
      VirtualFileSystem[ filename ].Append( content + "\n" );
    }

    /// <summary>
    /// Flushes the virtual file system to disc.
    /// Should only be used after extension generation has completed.
    /// </summary>
    public static void FlushVirtual() {
      foreach( KeyValuePair<string, StringBuilder> file in VirtualFileSystem ) {
        File.WriteAllText( file.Key, file.Value.ToString() );
      }
    }

    /// <summary>
    /// Wraps a given virtual file with two strings.
    /// </summary>
    /// <param name="outputDirectory">The output directory of the extension.</param>
    /// <param name="filename">The file that should be wrapped.</param>
    /// <param name="front">The part that should be placed in front.</param>
    /// <param name="back">The part that should be appended.</param>
    public static void WrapVirtual( string outputDirectory, string filename, string front, string back ) {
      string targetFilename = Path.Combine( outputDirectory, filename );
      if( !VirtualFileSystem.ContainsKey( targetFilename ) ) {
        VirtualFileSystem[ targetFilename ] = new StringBuilder();
      }
      VirtualFileSystem[ targetFilename ].Insert( 0, front );
      VirtualFileSystem[ targetFilename ].Append( back );
    }
  }
}
