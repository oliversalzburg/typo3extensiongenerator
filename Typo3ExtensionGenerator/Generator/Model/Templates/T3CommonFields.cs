﻿namespace Typo3ExtensionGenerator.Generator.Model.Templates {
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

    public const string TableControlFields = "    'tstamp'                   => 'tstamp',\n" +
                                             "    'crdate'                   => 'crdate',\n" +
                                             "    'cruser_id'                => 'cruser_id',\n" +
                                             "    'delete'                   => 'deleted',\n" +
                                             "    'enablecolumns'            => array(\n" +
                                             "      'disabled'  => 'hidden',\n" +
                                             "      'starttime' => 'starttime',\n" +
                                             "      'endtime'   => 'endtime',\n" +
                                             "      'fe_group'  => 'fe_group'\n" +
                                             "    ),\n" +
                                             "    'editlock'                 => 'editlock'";

    public const string InterfaceInfoFields = "hidden, starttime, endtime, fe_group";


    public const string Palette = "'paletteAccess' => array(\n" +
                                  "  'showitem' => 'starttime;LLL:EXT:cms/locallang_ttc.xml:starttime_formlabel,\n" +
                                  "      endtime;LLL:EXT:cms/locallang_ttc.xml:endtime_formlabel,\n" +
                                  "      --linebreak--, fe_group;LLL:EXT:cms/locallang_ttc.xml:fe_group_formlabel,\n" +
                                  "      --linebreak--,editlock,hidden',\n" +
                                  "  'canNotCollapse' => TRUE\n" +
                                  ")";
  }
}
