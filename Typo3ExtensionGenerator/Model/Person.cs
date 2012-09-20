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
    /// The fragment from which this object was generated.
    /// </summary>
    public Fragment SourceFragment { get; set; }
    #endregion
  }
}
