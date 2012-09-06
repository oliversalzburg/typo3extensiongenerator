using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
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

    [Serializable]
    private class CacheEntry {
      public string Md5 { get; set; }
      public DateTime LastWriteTimeUtc { get; set; }
    }

    private static bool UsedCachedStorage { get; set; }
    private static Dictionary<string, CacheEntry> Cache { get; set; }

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
    /// <param name="lastWriteTimeUtc">The last modified timestamp that should be used for the file.</param>
    public void WriteFile( string filename, string content, DateTime lastWriteTimeUtc ) {
      if( IsVirtual( filename ) ) {
        WriteVirtual( filename, content );

      } else {
        InternalWrite( OutputDirectory, filename, content, lastWriteTimeUtc );
      }
    }

    /// <summary>
    /// Write a binary file
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="content">The content that should be written to the file.</param>
    /// <param name="lastWriteTimeUtc">The last modified timestamp that should be used for the file.</param>
    public void WriteFile( string filename, byte[] content, DateTime lastWriteTimeUtc ) {
      InternalWrite( OutputDirectory,filename,content,lastWriteTimeUtc );
    }

    public void WritePhpFile( string filename, string content, DateTime lastWriteTimeUtc ) {
      string fileContent = string.Format( "<?php\n{0}\n?>", content );
      WriteFile( filename, fileContent, lastWriteTimeUtc );

#if DEBUG && FALSE
      fileContent = LudicrousPrettyPrinter.PrettyPrint( fileContent );
      WriteFile( filename + ".pp", fileContent );
#endif
    }

    private bool IsVirtual( string filename ) {
      return VirtualFileSystem.ContainsKey( filename );
    }

    public void WriteVirtual( string filename, string content ) {
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
        Log.InfoFormat( "Flushing '{0}'...", file.Key );
        InternalWrite( targetDirectory, file.Key, file.Value.ToString(), DateTime.UtcNow );
      }
    }

    #region Internal Writes
    /// <summary>
    /// Writes the given string to the given file.
    /// </summary>
    /// <param name="targetDirectory">The root output directory.</param>
    /// <param name="filename">The name of the file to write to.</param>
    /// <param name="content">The content that should be written to the file.</param>
    /// <param name="lastWriteTimeUtc">The last write timestamp that should be set on the file.</param>
    private static void InternalWrite(
      string targetDirectory, string filename, string content, DateTime lastWriteTimeUtc ) {
      string absoluteFilename = Path.Combine( targetDirectory, filename );
      Directory.CreateDirectory( new FileInfo( absoluteFilename ).DirectoryName );

      if( UsedCachedStorage ) {
        lastWriteTimeUtc = UpdateCacheEntry( filename, CalculateMd5Hash( content ), lastWriteTimeUtc );
      }
      File.WriteAllText( absoluteFilename, content );

      // Set last modified time
      FileInfo fileInfo = new FileInfo( absoluteFilename );
      fileInfo.LastWriteTimeUtc = lastWriteTimeUtc;
    }

    /// <summary>
    /// Writes the given bytes to the given file.
    /// </summary>
    /// <param name="targetDirectory">The root output directory.</param>
    /// <param name="filename">The name of the file to write to.</param>
    /// <param name="content">The content that should be written to the file.</param>
    /// <param name="lastWriteTimeUtc">The last write timestamp that should be set on the file.</param>
    private static void InternalWrite(
      string targetDirectory, string filename, byte[] content, DateTime lastWriteTimeUtc ) {
      string absoluteFilename = Path.Combine( targetDirectory, filename );
      Directory.CreateDirectory( new FileInfo( absoluteFilename ).DirectoryName );

      if( UsedCachedStorage ) {
        lastWriteTimeUtc = UpdateCacheEntry( filename, CalculateMd5Hash( content ), lastWriteTimeUtc );
      }
      File.WriteAllBytes( absoluteFilename, content );

      // Set last modified time
      FileInfo fileInfo = new FileInfo( absoluteFilename );
      fileInfo.LastWriteTimeUtc = lastWriteTimeUtc;
    }

    private static DateTime UpdateCacheEntry( string filename, string contentHash, DateTime lastWriteTimeUtc ) {
      // Do we have a cache yet?
      if( null == Cache ) {
        // Create it.
        Cache = new Dictionary<string, CacheEntry>();
      }
      // Is this file in the cache?
      if( Cache.ContainsKey( filename ) ) {
        // If the file contents are still identical, we re-use the old timestamp
        if( Cache[ filename ].Md5 == contentHash ) {
          lastWriteTimeUtc = Cache[ filename ].LastWriteTimeUtc;
        } else {
          // Update cache entry
          Log.WarnFormat( "Updated cache entry '{0}'.", filename );
          Cache[ filename ].Md5 = contentHash;
          Cache[ filename ].LastWriteTimeUtc = lastWriteTimeUtc;
        }
      } else {
        // Add it to the cache
        Log.WarnFormat( "NEW cache entry '{0}'.", filename );
        Cache[ filename ] = new CacheEntry {Md5 = contentHash, LastWriteTimeUtc = lastWriteTimeUtc};
      }
      return lastWriteTimeUtc;
    }

    /// <summary>
    /// Calculates the MD5 hash for a given string.
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/csharpfaq/archive/2006/10/09/how-do-i-calculate-a-md5-hash-from-a-string_3f00_.aspx"/>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string CalculateMd5Hash( string input ) {
      byte[] inputBytes = Encoding.ASCII.GetBytes( input );
      return CalculateMd5Hash( inputBytes );
    }
    private static string CalculateMd5Hash( byte[] inputBytes ) {
      MD5 md5 = MD5.Create();
      byte[] hash = md5.ComputeHash( inputBytes );
 
      StringBuilder sb = new StringBuilder();
      for( int i = 0; i < hash.Length; i++ ){
          sb.Append( hash[ i ].ToString( "X2" ) );
      }
      return sb.ToString();
    }
    #endregion

    protected internal static void StartCachingSession( string cacheFile ) {
      UsedCachedStorage = true;

      try {
        Log.InfoFormat( "Reading cache file '{0}'...", cacheFile );
        using( FileStream filestream = new FileStream( cacheFile, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
          BinaryFormatter binaryFormatter = new BinaryFormatter();
          Cache = (Dictionary<string, CacheEntry>)binaryFormatter.Deserialize( filestream );
        }
      } catch( FileNotFoundException ) {
        Log.InfoFormat( "Requested cache file '{0}' was not found.", cacheFile );

      } catch( SerializationException ) {
        Log.WarnFormat( "Exception while trying to deserialize cache file '{0}'! Cache is lost!", cacheFile );
      }
    }

    protected internal static void EndCachingSession( string cacheFile ) {
      if( UsedCachedStorage ) {
        Log.InfoFormat( "Writing cache file '{0}'...", cacheFile );
      
        using( FileStream filestream = new FileStream( cacheFile, FileMode.Create,FileAccess.Write,FileShare.Read )) {
          BinaryFormatter binaryFormatter = new BinaryFormatter();
          binaryFormatter.Serialize( filestream, Cache );
        }
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
