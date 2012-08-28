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

    public const string Interfaces = "'starttime' => array(" +
                                     "	'exclude' => 1," +
                                     "	'l10n_mode' => 'mergeIfNotBlank'," +
                                     "	'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.starttime'," +
                                     "	'config' => array(" +
                                     "		'type' => 'input'," +
                                     "		'size' => 13," +
                                     "		'max' => 20," +
                                     "		'eval' => 'datetime'," +
                                     "		'checkbox' => 0," +
                                     "		'default' => 0," +
                                     "		'range' => array(" +
                                     "			'lower' => mktime(0, 0, 0, date('m'), date('d'), date('Y'))" +
                                     "		)," +
                                     "	)," +
                                     ")," +
                                     "'endtime' => array(" +
                                     "	'exclude' => 1," +
                                     "	'l10n_mode' => 'mergeIfNotBlank'," +
                                     "	'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.endtime'," +
                                     "	'config' => array(" +
                                     "		'type' => 'input'," +
                                     "		'size' => 13," +
                                     "		'max' => 20," +
                                     "		'eval' => 'datetime'," +
                                     "		'checkbox' => 0," +
                                     "		'default' => 0," +
                                     "		'range' => array(" +
                                     "			'lower' => mktime(0, 0, 0, date('m'), date('d'), date('Y'))" +
                                     "		)," +
                                     "	)," +
                                     ")," +
                                     "'fe_group' => array(" +
                                     "  'exclude' => 1," +
                                     "  'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.fe_group'," +
                                     "  'config' => array(" +
                                     "    'type' => 'select'," +
                                     "    'size' => 5," +
                                     "    'maxitems' => 20," +
                                     "    'items' => array(" +
                                     "      array(" +
                                     "        'LLL:EXT:lang/locallang_general.xml:LGL.hide_at_login'," +
                                     "        -1," +
                                     "      )," +
                                     "      array(" +
                                     "        'LLL:EXT:lang/locallang_general.xml:LGL.any_login'," +
                                     "        -2," +
                                     "      )," +
                                     "      array(" +
                                     "        'LLL:EXT:lang/locallang_general.xml:LGL.usergroups'," +
                                     "        '--div--'," +
                                     "      )," +
                                     "    )," +
                                     "    'exclusiveKeys' => '-1,-2'," +
                                     "    'foreign_table' => 'fe_groups'," +
                                     "    'foreign_table_where' => 'ORDER BY fe_groups.title'," +
                                     "  )," +
                                     ")," +
                                     "'editlock' => array(" +
                                     "  'exclude' => 1," +
                                     "  'l10n_mode' => 'mergeIfNotBlank'," +
                                     "  'label' => 'LLL:EXT:lang/locallang_tca.xml:editlock'," +
                                     "  'config' => array(" +
                                     "    'type' => 'check'" +
                                     "  )" +
                                     ")," +
                                     "'hidden' => array(" +
                                     "	'exclude' => 1," +
                                     "	'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.hidden'," +
                                     "	'config' => array(" +
                                     "		'type' => 'check'," +
                                     "	)," +
                                     ")";



  }
}
