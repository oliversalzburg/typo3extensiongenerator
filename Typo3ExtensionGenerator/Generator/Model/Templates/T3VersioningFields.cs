namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3VersioningFields {
    public const string Content = "t3ver_oid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_id int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_wsid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_label varchar(255) DEFAULT '' NOT NULL,\n" +
                                  "t3ver_state tinyint(4) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_stage int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_count int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_tstamp int(11) DEFAULT '0' NOT NULL,\n" +
                                  "t3ver_move_id int(11) DEFAULT '0' NOT NULL";

    public const string Keys = "KEY t3ver_oid (t3ver_oid,t3ver_wsid)";

    public const string TableControlFields = "    'versioningWS'             => 2,\n" +
                                             "    'versioning_followPages'   => TRUE";

  }
}
