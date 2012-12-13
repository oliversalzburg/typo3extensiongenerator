﻿using System;
using System.Collections.Generic;
using System.Linq;
using Typo3ExtensionGenerator.Model;
using Typo3ExtensionGenerator.Model.Plugin;
using Typo3ExtensionGenerator.Parser;
using Typo3ExtensionGenerator.Parser.Definitions;
using Typo3ExtensionGenerator.Resolver.Plugin;

namespace Typo3ExtensionGenerator.Resolver.Extension {
  /// <summary>
  /// Resolves scheduler tasks from extension markup
  /// </summary>
  public static class TaskResolver {
    /// <summary>
    /// Resolves all tasks defined in a given parsed fragment.
    /// </summary>
    /// <param name="parsedFragment"></param>
    /// <returns></returns>
    /// <exception cref="ParserException">Task has no name!</exception>
    public static List<Task> Resolve( Fragment parsedFragment ) {
      IEnumerable<Fragment> taskFragments = parsedFragment.Fragments.Where( p => p.Keyword == Keywords.ExtensionDirectives.DeclareTask );
      if( !taskFragments.Any() ) return null;

      List<Task> tasks = new List<Task>();
      foreach( Fragment taskFragment in taskFragments ) {
        // Construct the plugin with the given name
        Task task = new Task {Name = taskFragment.Parameters, SourceFragment = taskFragment};
        
        // Resolve task
        foreach( Fragment serviceParameter in taskFragment.Fragments ) {
          if( serviceParameter.Keyword == Keywords.Description ) {
            task.Description = serviceParameter.Parameters;

          } else if( serviceParameter.Keyword == Keywords.Title ) {
            task.Name = serviceParameter.Parameters;
        
          } else if( serviceParameter.Keyword == Keywords.Implementation ) {
            task.Implementation = serviceParameter.Parameters;

          } else if( serviceParameter.Keyword == Keywords.ServiceDirectives.AdditionalFields ) {
            task.AdditionalFieldsClass = serviceParameter.Parameters;
          }
        }

        // If no name was defined, throw
        if( string.IsNullOrEmpty( task.Name ) ) {
          throw new ParserException( "Service has no name!", parsedFragment.SourceDocument );
        }

        tasks.Add( task );
      }

      return tasks;
    }
  }
}
