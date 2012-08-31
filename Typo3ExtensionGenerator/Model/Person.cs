using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Parser;

namespace Typo3ExtensionGenerator.Model {
  public class Person : IParserResult {
    public string Name { get; set; }
    public string Email { get; set; }
    public string Company { get; set; }

    public static Person Someone { 
      get { return new Person {Company = "Some Company", Email = "user@example.com", Name = "John Doe"}; } 
    }

    #region Implementation of IParserResult
    /// <summary>
    /// The line on which this object was originally defined in the input.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// The parsed partial from which this object was generated.
    /// </summary>
    public ExtensionParser.ParsedPartial SourcePartial { get; set; }
    #endregion
  }
}
