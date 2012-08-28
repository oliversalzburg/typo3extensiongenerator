using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator {
  class Program {
    static void Main( string[] args ) {
      if( !args.Any() || !File.Exists( args[ 0 ] ) ) {
        Console.Error.WriteLine( "No input file provided or file nonexistent." );
        return;
      }

      Console.Write( "Reading '{0}'...", args[ 0 ] );
      string markup = File.ReadAllText( args[ 0 ] );
      Console.WriteLine( "Done." );

      Console.Write( "Parsing..." );
      ExtensionParser parser = new ExtensionParser();
      Extension extension = parser.Parse( markup );
      Console.WriteLine( "Done." );

      Console.WriteLine( "Found extension definition for '{0}'", extension.Key );

      ExtensionGenerator generator = new ExtensionGenerator() {
                                                                TargetDirectory = Path.Combine( Environment.CurrentDirectory, "output" )
                                                              };
      generator.Generate( extension );
      Console.WriteLine( "Done." );
      
      Console.ReadLine();
    }
  }
}
