using System;
using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Parser.Document;

namespace Typo3ExtensionGenerator.Parser {
  /// <summary>
  /// A partially parsed markup element. This is the core element of the grammar.
  /// Elements always consist of a header and an optional body.
  /// The basic syntax is:
  /// keyword parameters {
  ///   body
  /// }
  /// 
  /// keyword + parameters = header
  /// </summary>
  public class Fragment {
    /// <summary>
    /// The header part of the fragment.
    /// </summary>
    /// <remarks>
    /// The header is always the full part that stood before the body (the body being indicated by a scope begin).
    /// </remarks>
    public string Header { get; set; }

    /// <summary>
    /// The keyword that identifies this fragment.
    /// </summary>
    public string Keyword { get; set; }

    /// <summary>
    /// The body part of the fragment.
    /// </summary>
    /// <remarks>
    /// The body is always the full part that stood after the header (the body being indicated by a scope begin).
    /// </remarks>
    public string Body { get; set; }

    /// <summary>
    /// The part that stood after the keyword.
    /// </summary>
    public string Parameters { get; set; }

    /// <summary>
    /// A list of parsed child elements
    /// </summary>
    public List<Fragment> Fragments { get; set; }

    public VirtualDocument SourceDocument { get; set; }

    public Fragment() {
      Fragments = new List<Fragment>();
    }

    public override string ToString() {
      return String.Format( "{0} ( {1} )", Keyword, Parameters );
    }
  }
}