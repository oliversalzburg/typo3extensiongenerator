namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3ManagedFields {
    public const string Content = "uid int(11) NOT NULL auto_increment,\n" +
                                  "pid int(11) DEFAULT '0' NOT NULL";

    public const string Keys = "PRIMARY KEY (uid),\n" +
                               "KEY parent (pid)";
  }
}
