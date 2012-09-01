using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Typo3ExtensionGenerator.Generator.PrettyPrint;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  public abstract class AbstractGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

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
    protected static readonly Dictionary<string,StringBuilder> VirtualFileSystem = new Dictionary<string, StringBuilder>();

    protected AbstractGenerator( string outputDirectory, Extension subject ) {
      OutputDirectory = outputDirectory;
      Subject         = subject;
    }

    /// <summary>
    /// Writes the given content to a file.
    /// Supports "virtual" files. Virtual files are not directly written to disk, but collect content.
    /// They will have to be flushed at some point in the process. If a file was marked virtual once,
    /// all future writes to that file will also be virtual.
    /// </summary>
    /// <param name="filename">The path of the file, relative to the extension root.</param>
    /// <param name="content">What should be written to the file.</param>
    /// <param name="useVirtual">Should this be written to a virtual file?</param>
    public void WriteFile( string filename, string content, bool useVirtual = false ) {
      if( useVirtual || IsVirtual( filename ) ) {
        WriteVirtual( filename, content );

      } else {
        string targetFilename = Path.Combine( OutputDirectory, filename );
        Directory.CreateDirectory( new FileInfo( targetFilename ).DirectoryName );
        File.WriteAllText( targetFilename, content, Encoding.Default );
      }
    }

    public void WritePhpFile( string filename, string content ) {
      string fileContent = string.Format( "<?php\n{0}\n?>", content );
      WriteFile( filename, fileContent );

#if DEBUG && FALSE
      fileContent = LudicrousPrettyPrinter.PrettyPrint( fileContent );
      WriteFile( filename + ".pp", fileContent );
#endif
    }

    private bool IsVirtual( string filename ) {
      return VirtualFileSystem.ContainsKey( filename );
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
    public static void FlushVirtual( string targetDirectory ) {
      foreach( KeyValuePair<string, StringBuilder> file in VirtualFileSystem ) {
        string absoluteFilename = Path.Combine( targetDirectory, file.Key );
        Directory.CreateDirectory( new FileInfo( absoluteFilename ).DirectoryName );
        Log.InfoFormat( "Flushing '{0}'...", file.Key );
        File.WriteAllText( absoluteFilename , file.Value.ToString() );
      }
    }

    /// <summary>
    /// Wraps a given virtual file with two strings.
    /// </summary>
    /// <param name="filename">The file that should be wrapped.</param>
    /// <param name="front">The part that should be placed in front.</param>
    /// <param name="back">The part that should be appended.</param>
    public static void WrapVirtual( string filename, string front, string back ) {
      if( !VirtualFileSystem.ContainsKey( filename ) ) {
        VirtualFileSystem[ filename ] = new StringBuilder();
      }
      VirtualFileSystem[ filename ].Insert( 0, front );
      VirtualFileSystem[ filename ].Append( back );
    }

    /// <summary>
    /// Wraps all matching files in given strings.
    /// </summary>
    /// <param name="filter">A regular expression.</param>
    /// <param name="front"></param>
    /// <param name="back"></param>
    public static void WrapAllVirtual( string filter, string front, string back ) {
      Regex fileFilter = new Regex( filter );
      foreach( KeyValuePair<string, StringBuilder> file in VirtualFileSystem ) {
        if( fileFilter.IsMatch( file.Key ) ) {
          file.Value.Insert( 0, front );
          file.Value.Append( back );
        }
      }
    }
  }
}
