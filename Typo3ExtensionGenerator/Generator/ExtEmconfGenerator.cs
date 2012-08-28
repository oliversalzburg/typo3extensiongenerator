﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typo3ExtensionGenerator.Model;

namespace Typo3ExtensionGenerator.Generator {
  public class ExtEmconfGenerator : AbstractGenerator, IGenerator {
    public ExtEmconfGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public string TargetFile {
      get { return "ext_emconf.php"; }
    }

    public void Generate() {

      Console.WriteLine( string.Format( "Generating {0}...", TargetFile ) );

      string template = "$EM_CONF[$_EXTKEY] = array(\n" +
                        "  'title'            => '{0}',\n" +
                        "  'description'      => '{1}',\n" +
                        "  'category'         => 'fe',\n" +
                        "  'author'           => '{2}',\n" +
                        "  'author_email'     => '{3}',\n" +
                        "  'author_company'   => '{4}',\n" +
                        "  'shy'              => '',\n" +
                        "  'priority'         => '',\n" +
                        "  'module'           => '',\n" +
                        "  'state'            => 'alpha',\n" +
                        "  'internal'         => '',\n" +
                        "  'uploadfolder'     => '0',\n" +
                        "  'createDirs'       => '',\n" +
                        "  'modify_tables'    => '',\n" +
                        "  'clearCacheOnLoad' => 0,\n" +
                        "  'lockType'         => '',\n" +
                        "  'version'          => '0.0.1',\n" +
                        "  'constraints'      => array(\n" +
                        "    'depends'   => array(\n" +
                        "      'extbase' => '1.3.0',\n" +
                        "      'fluid'   => '1.3.0',\n" +
                        "    ),\n" +
                        "    'conflicts' => array(\n" +
                        "    ),\n" +
                        "    'suggests'  => array(\n" +
                        "    ),\n" +
                        "  ),\n" +
                        ");";
      string result = string.Format(
        template, Subject.Title, Subject.Description, Subject.Author.Name, Subject.Author.Email,
        Subject.Author.Company );

      WritePhpFile( TargetFile, result );
    }
  }
}