using SmartFormat;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3TranslationFields {
    public const string Content = "t3_origuid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "sys_language_uid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "l10n_parent int(11) DEFAULT '0' NOT NULL,\n" +
                                  "l10n_diffsource mediumblob";

    public const string Keys = "KEY language (l10n_parent,sys_language_uid)";

    public const string TableControlFields = "    'origUid'                  => 't3_origuid',\n" +
                                             "    'languageField'            => 'sys_language_uid',\n" +
                                             "    'transOrigPointerField'    => 'l10n_parent',\n" +
                                             "    'transOrigDiffSourceField' => 'l10n_diffsource'";

    public const string InterfaceInfoFields = "sys_language_uid, l10n_parent, l10n_diffsource";
    public const string InterfaceTypeFields = "l10n_parent, l10n_diffsource";

    private const string Interfaces = "'sys_language_uid' => array(" +
                                      "	'exclude' => 1," +
                                      "	'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.language'," +
                                      "	'config' => array(" +
                                      "		'type' => 'select'," +
                                      "		'foreign_table' => 'sys_language'," +
                                      "		'foreign_table_where' => 'ORDER BY sys_language.title'," +
                                      "		'items' => array(" +
                                      "			array('LLL:EXT:lang/locallang_general.xml:LGL.allLanguages', -1)," +
                                      "			array('LLL:EXT:lang/locallang_general.xml:LGL.default_value', 0)" +
                                      "		)," +
                                      "	)," +
                                      ")," +
                                      "'l10n_parent' => array(" +
                                      "	'displayCond' => 'FIELD:sys_language_uid:>:0'," +
                                      "	'exclude' => 1," +
                                      "	'label' => 'LLL:EXT:lang/locallang_general.xml:LGL.l18n_parent'," +
                                      "	'config' => array(" +
                                      "		'type' => 'select'," +
                                      "		'items' => array(" +
                                      "			array('', 0)," +
                                      "		)," +
                                      "		'foreign_table' => '{_foreignTable}'," +
                                      "		'foreign_table_where' => 'AND {_foreignTable}.pid=###CURRENT_PID### AND {_foreignTable}.sys_language_uid IN (-1,0)'," +
                                      "	)," +
                                      ")," +
                                      "'l10n_diffsource' => array(" +
                                      "	'config' => array(" +
                                      "		'type' => 'passthrough'," +
                                      "	)," +
                                      ")";

    public static string GetInterfaces( Extension extension, DataModel model ) {
      var dataObject = new {
                             _foreignTable = NameHelper.GetAbsoluteModelName( extension, model )
                           };
      return Interfaces.FormatSmart( dataObject );
    }

  }
}
