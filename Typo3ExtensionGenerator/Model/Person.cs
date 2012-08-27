using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Model {
  public class Person {
    public string Name { get; set; }
    public string Email { get; set; }
    public string Company { get; set; }

    public static Person Someone { 
      get { return new Person {Company = "Some Company", Email = "user@example.com", Name = "John Doe"}; } 
    }
  }
}
