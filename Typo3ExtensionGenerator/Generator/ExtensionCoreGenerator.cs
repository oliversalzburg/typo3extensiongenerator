using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartFormat;
using Typo3ExtensionGenerator.Model;
using log4net;

namespace Typo3ExtensionGenerator.Generator {
  /// <summary>
  /// Generates the core extension files like ext_emconf and TypoScript.
  /// </summary>
  public class ExtensionCoreGenerator : AbstractGenerator, IGenerator {

    private static readonly ILog Log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

    public ExtensionCoreGenerator( string outputDirectory, Extension extension ) : base( outputDirectory, extension ) {}

    public void Generate() {

      Log.Info( "Generating ext_emconf.php..." );

      const string template = "$EM_CONF[$_EXTKEY] = array(\n" +
                              "  'title'            => '{title}',\n" +
                              "  'description'      => '{description}',\n" +
                              "  'category'         => '{category}',\n" +
                              "  'author'           => '{authorName}',\n" +
                              "  'author_email'     => '{authorEmail}',\n" +
                              "  'author_company'   => '{authorCompany}',\n" +
                              "  'shy'              => '',\n" +
                              "  'priority'         => '',\n" +
                              "  'module'           => '',\n" +
                              "  'state'            => '{state}',\n" +
                              "  'internal'         => '',\n" +
                              "  'uploadfolder'     => '0',\n" +
                              "  'createDirs'       => '',\n" +
                              "  'modify_tables'    => '',\n" +
                              "  'clearCacheOnLoad' => 0,\n" +
                              "  'lockType'         => '',\n" +
                              "  'version'          => '{version}',\n" +
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

      string result = template.FormatSmart(
        new {
              title = Subject.Title,
              description = Subject.Description,
              authorName = Subject.Author.Name,
              authorEmail = Subject.Author.Email,
              authorCompany = Subject.Author.Company,
              category = Subject.Category,
              state = Subject.State,
              version = Subject.Version
            } );

      WritePhpFile( "ext_emconf.php", result );
    }

  }
}
