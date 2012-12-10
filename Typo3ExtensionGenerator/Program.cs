using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using Typo3ExtensionGenerator.Compatibility;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.PreProcess;
using log4net;

namespace Typo3ExtensionGenerator {
  internal static class Program {
    
    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Should we just show the help and exit?
    /// </summary>
    private static bool ShowHelp { get; set; }

    private static Context GeneratorContext = new Context();

    private static void Main( string[] args ) {
      DateTime start = DateTime.Now;

      if( ParseCommandLine( args ) ) return;

      // Was a directory provided as input?
      if( !File.Exists( GeneratorContext.InputFile ) && Directory.Exists( GeneratorContext.InputFile ) ) {
        DirectoryInfo inputDirectory = new DirectoryInfo( GeneratorContext.InputFile );
        FileInfo[] extensionFiles = inputDirectory.GetFiles( "*.extgen" );
        
        if( 0 == extensionFiles.Length ) {
          Log.Error( "A directory was provided as input, but it contained no .extgen files." );
          return;
        }

        if( 1 < extensionFiles.Length ) {
          Log.Warn( "A directory was provided as input, but it contained more than 1 .extgen file." );
          return;
        }

        GeneratorContext.InputFile = extensionFiles.First().FullName;

      } else if( !File.Exists( GeneratorContext.InputFile ) ) {
        Log.ErrorFormat( "The given input file '{0}' does not exist.", GeneratorContext.InputFile );
        return;
      }

      if( string.IsNullOrEmpty( GeneratorContext.OutputDirectory ) ) {
        GeneratorContext.OutputDirectory = "output";
      }
      if( string.IsNullOrEmpty( GeneratorContext.InputFile ) ) {
        Log.Fatal( "No input file given." );
        return;
      }
      
      // Set the working directory to the directory name of the extension description
      Directory.SetCurrentDirectory( new FileInfo( GeneratorContext.InputFile ).DirectoryName );
      
      // I hate small console windows!
      try {
        Console.WindowWidth  = 160;
        Console.WindowHeight = 50;
      } catch( IOException ) {
        // Maybe there is no console window (stream redirection)
      }

      Log.InfoFormat( "Reading '{0}'...", GeneratorContext.InputFile );
      string markup = File.ReadAllText( GeneratorContext.InputFile );

      try {
        Log.Info( "Parsing..." );
        Extension extension = ExtensionParser.Parse( markup, GeneratorContext.InputFile );

        Log.InfoFormat( "Found extension definition for '{0}'", extension.Key );
        Log.InfoFormat( "Compatibility level '{0}'", GeneratorContext.TargetVersion.Version );

        string cacheFile = Path.Combine( Environment.CurrentDirectory, extension.Key + ".cache" );
        GeneratorContext.OutputDirectory = Path.Combine( Environment.CurrentDirectory, GeneratorContext.OutputDirectory );
        ExtensionGenerator generator = new ExtensionGenerator( GeneratorContext, extension );

        AbstractGenerator.StartCachingSession( cacheFile );
        generator.Generate();
        AbstractGenerator.EndCachingSession( cacheFile );

        Console.WriteLine( "Finished after {0}", DateTime.Now.Subtract( start ) );

      } catch( ParserException parserException ) {
        Log.Error( parserException );

      } catch( GeneratorException generatorException ) {
        Log.Error( generatorException );
      }
    }

    /// <summary>
    /// Parses command line parameters.
    /// </summary>
    /// <param name="args">The command line parameters passed to the program.</param>
    /// <returns><see langword="true"/> if the application should exit; <see langword="false"/> otherwise.</returns>
    private static bool ParseCommandLine( IEnumerable<string> args ) {
      OptionSet options = new OptionSet {
                                          { "output=",  "Where the extension directory should be placed.", v => GeneratorContext.OutputDirectory = v },
                                          { "input=",   "The file we should parse.",                       v => GeneratorContext.InputFile = v },
                                          { "h|?|help", "Shows this help message",                         v => ShowHelp = v != null }
                                        };

      try {
        options.Parse( args );

      } catch( OptionException ex ) {
        Console.Write( "Controller.exe:" );
        Console.WriteLine( ex.Message );
        Console.WriteLine( "Try 'Controller --help' for more information." );
        return true;
      }

      if( ShowHelp ) {
        Console.WriteLine( "Usage: Controller [OPTIONS]" );
        Console.WriteLine();
        Console.WriteLine( "Options:" );
        options.WriteOptionDescriptions( Console.Out );
        return true;
      }

      return false;
    }
  }
}