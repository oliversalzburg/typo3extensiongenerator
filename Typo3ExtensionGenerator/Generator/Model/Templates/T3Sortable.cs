namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3Sortable {
    public const string Content = "sorting int(11) DEFAULT '0' NOT NULL";

    public const string TableControlFields = "'default_sortby'           => 'ORDER BY sorting',\n" +
                                             "'sortby'                   => 'sorting'";

    public const string Interfaces = "		'sorting' => array(\n" +
                                     "      'label' => 'sorting',\n" +
                                     "      'config' => array(\n" +
                                     "        'type' => 'input'\n" +
                                     "      )\n" +
                                     "    )";
  }
}
