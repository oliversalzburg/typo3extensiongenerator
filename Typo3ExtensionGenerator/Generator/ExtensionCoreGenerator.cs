using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

      WritePhpFile( "ext_emconf.php", result );
    }

  }
}
