using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Configuration;

namespace Typo3ExtensionGenerator.Generator.Configuration {
  public class InterfaceGenerator {
    public static string Generate( Extension extension, Interface subject ) {
      const string propertyTemplate = "'{0}' => {1},";
      
      StringBuilder interfaceDefinition = new StringBuilder();

      interfaceDefinition.Append( string.Format( propertyTemplate, "exclude", subject.Exclude ? "1" : "0" ) );
      

      string finalInterface = interfaceDefinition.ToString().TrimEnd( new[] {','} );
      return finalInterface;
    }
  }
}
