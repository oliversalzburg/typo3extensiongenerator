namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3CommonFields {
    public const string Content = "tstamp int(11) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "crdate int(11) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "cruser_id int(11) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "deleted tinyint(4) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "hidden tinyint(4) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "starttime int(11) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "endtime int(11) unsigned DEFAULT '0' NOT NULL,\n" +
                                  "fe_group varchar(100) DEFAULT '0' NOT NULL,\n" +
                                  "editlock tinyint(4) DEFAULT '0' NOT NULL";
  }
}
