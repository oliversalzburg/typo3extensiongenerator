using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using log4net;

namespace Typo3ExtensionGenerator {
  internal static class Program {
    
    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    /// <summary>
    /// Should we just show the help and exit?
    /// </summary>
    private static bool ShowHelp { get; set; }

    /// <summary>
    /// The directory from where to take input files that should be merged with the extension.
    /// </summary>
    private static string RequireRoot { get; set; }

    /// <summary>
    /// Where should the resulting extension be placed.
    /// </summary>
    private static string OutputDirectory { get; set; }

    /// <summary>
    /// The file that contains the extension description.
    /// </summary>
    private static string InputFile { get; set; }

    private static void Main( string[] args ) {
      DateTime start = DateTime.Now;

      if( ParseCommandLine( args ) ) return;

      if( string.IsNullOrEmpty( OutputDirectory ) ) {
        OutputDirectory = "output";
      }
      if( string.IsNullOrEmpty( InputFile ) ) {
        Log.Fatal( "No input file given." );
        return;
      }
      if( string.IsNullOrEmpty( RequireRoot ) ) {
        RequireRoot = Path.Combine( new FileInfo( InputFile ).DirectoryName, "input" );
      }

      // Set the working directory to the directory name of the extension description
      Directory.SetCurrentDirectory( new FileInfo( InputFile ).DirectoryName );
      
      // I hate small console windows!
      try {
        Console.WindowWidth = 160;
        Console.WindowHeight = 50;
      } catch( IOException ) {
        // Maybe there is no console window (stream redirection)
      }

      Log.InfoFormat( "Reading '{0}'...", InputFile );
      string markup = File.ReadAllText( InputFile );
      
      try {
        Log.Info( "Parsing..." );
        ExtensionParser parser = new ExtensionParser();
        Extension extension = parser.Parse( markup );
        
        Log.InfoFormat( "Found extension definition for '{0}'", extension.Key );

        ExtensionGenerator generator = new ExtensionGenerator( OutputDirectory, extension ) {
                                                                                              TargetDirectory =
                                                                                                Path.Combine(
                                                                                                  Environment.
                                                                                                    CurrentDirectory,
                                                                                                  OutputDirectory )
                                                                                            };

        generator.Generate();
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
                                          {"require=", "The 'require' root directory.",                   v => RequireRoot = v},
                                          {"output=",  "Where the extension directory should be placed.", v => OutputDirectory = v },
                                          {"input=",   "The file we should parse.",                       v => InputFile = v},
                                          {"h|?|help", "Shows this help message",                         v => ShowHelp = v != null}
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