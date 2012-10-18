using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typo3ExtensionGenerator.Generator;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Parser;

namespace Tests {
  [TestClass]
  public class ParserExceptionsTest {
    /// <summary>
    /// Helper construct to parse a given markup
    /// </summary>
    /// <param name="markup"></param>
    private void ParseMarkup( string markup ) {
      ExtensionParser parser = new ExtensionParser();
      Extension extension = parser.Parse( markup, "<unit test>" );

      ExtensionGenerator generator = new ExtensionGenerator( Path.GetTempPath(), extension );
      generator.Generate();
    }

    /// <summary>
    /// Extension keys can not contain whitespace
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Providing an extension key with whitespace was allowed.")]
    public void TestWhitespaceInExtensionKey() {
      const string markup = "extension a b {}";
      ParseMarkup( markup );
    }

    /// <summary>
    /// All "types" have to define an interface.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Providing a type without an interface was allowed.")]
    public void TestTypeWithoutInterface() {
      const string markup = "extension test { model foo {} configure foo { type { /* no inteface here */ } } }";
      ParseMarkup( markup );
    }

    /// <summary>
    /// All "palettes" have to define an interface.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Providing a palette without an interface was allowed.")]
    public void TestPaletteWithoutInterface() {
      const string markup = "extension test { model foo {} configure foo { palette bar { /* no inteface here */ } } }";
      ParseMarkup( markup );
    }

    /// <summary>
    /// When #including a file, you can't forget that closing quote.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Unclosed string was allowed.")]
    public void TestUnterminatedPreProcessorString() {
      const string markup = "#include \"foo";
      ParseMarkup( markup );
    }

    /// <summary>
    /// When #including a file, it must exist.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Non-existent #include target was allowed.")]
    public void TestNonExistentIncludeFile() {
      const string markup = "#include \"foo\"";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Strings need to be properly closed
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Unclosed string was allowed.")]
    public void TestUnmatchedQuote() {
      const string markup = "extension test { model \"a {} }";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Strings need to be properly closed
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Unclosed string was allowed.")]
    public void TestUnmatchedQuoteInParameters() {
      const string markup = "extension test {\n" +
                            "  model a {\n" +
                            "    foo \"bar';\n" +
                            "  }\n" +
                            "}";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Comments need to be properly closed
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Unclosed comment was allowed.")]
    public void TestUnmatchedComment() {
      const string markup = "extension test {\n" +
                            "  model a { /*\n" +
                            "    foo \"bar\";\n" +
                            "  }\n" +
                            "}";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Markup must contain a definition of an extension
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Markup without extension definition was allowed.")]
    public void TestExtensionDefinition() {
      const string markup = "model a {\n" +
                            "  foo \"bar\";\n" +
                            "}\n";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Listeners must refer to a signal host.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Listener without signal host was allowed.")]
    public void TestListenerWithoutHost() {
      const string markup = "model a {\n" +
                            "  listener onUpdate {\n" +
                            "    slot foo;\n" +
                            "  }\n" +
                            "}\n";
      ParseMarkup( markup );
    }

    /// <summary>
    /// Listeners must refer to a signal slot.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ParserException), "Listener without signal slot was allowed.")]
    public void TestListenerWithoutSignal() {
      const string markup = "model a {\n" +
                            "  listener onUpdate {\n" +
                            "    host foo;\n" +
                            "  }\n" +
                            "}\n";
      ParseMarkup( markup );
    }
  }
}
