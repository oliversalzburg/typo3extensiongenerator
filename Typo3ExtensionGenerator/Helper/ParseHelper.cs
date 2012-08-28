using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typo3ExtensionGenerator.Helper {
  public static class ParseHelper {
    public static bool ParseBool( string input ) {
      string trimmed = input.Trim().ToLower();
      return !( "off" == trimmed ||
                "false" == trimmed ||
                "no" == trimmed ||
                "0" == trimmed );
    }
  }
}
