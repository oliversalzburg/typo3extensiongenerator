﻿namespace Typo3ExtensionGenerator.Generator.Model.Templates {
  public static class T3TranslationFields {
    public const string Content = "t3_origuid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "sys_language_uid int(11) DEFAULT '0' NOT NULL,\n" +
                                  "l10n_parent int(11) DEFAULT '0' NOT NULL,\n" +
                                  "l10n_diffsource mediumblob";

    public const string Keys = "KEY language (l10n_parent,sys_language_uid)";
  }
}
