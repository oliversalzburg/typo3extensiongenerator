using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;
using log4net;

namespace Typo3ExtensionGenerator {
  internal static class Program {
    
    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    private static void Main( string[] args ) {
      if( !args.Any() || !File.Exists( args[ 0 ] ) ) {
        Log.Error( "No input file provided or file nonexistent." );
        return;
      }
      
      // I hate small console windows!
      Console.WindowWidth  *= 2;
      Console.WindowHeight *= 2;

      Log.InfoFormat( "Reading '{0}'...", args[ 0 ] );
      string markup = File.ReadAllText( args[ 0 ] );
      
      try {
        Log.Info( "Parsing..." );
        ExtensionParser parser = new ExtensionParser();
        Extension extension = parser.Parse( markup );
        
        Log.InfoFormat( "Found extension definition for '{0}'", extension.Key );

        ExtensionGenerator generator = new ExtensionGenerator {
                                                                TargetDirectory =
                                                                  Path.Combine(
                                                                    Environment.CurrentDirectory, "output" )
                                                              };
        generator.Generate( extension );

      } catch( ParserException parserException ) {
        Log.Error( parserException );

      } catch( GeneratorException generatorException ) {
        Log.Error( generatorException );
      }

      Console.WriteLine( "Press ENTER to exit." );
      Console.ReadLine();
    }
  }
}