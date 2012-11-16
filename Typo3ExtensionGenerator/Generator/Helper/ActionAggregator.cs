using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartFormat;
using Typo3ExtensionGenerator.Helper;
using Typo3ExtensionGenerator.Model;
using Action = Typo3ExtensionGenerator.Model.Action;

namespace Typo3ExtensionGenerator.Generator.Helper {
  /// <summary>
  /// The ActionAggregator helps to collect cachable and uncachable actions from controllers.
  /// </summary>
  public static class ActionAggregator {

    /// <summary>
    /// The result of aggregating actions
    /// </summary>
    public class AggregationResult {
      public string Cachable { get; set; }
      public string Uncachable { get; set; }
    }

    public const string ActionTemplate = "    '{controllerName}' => '{actionList}'";

    /// <summary>
    /// Aggregates all actions defined for a controller.
    /// </summary>
    /// <param name="seed">The object that conains the Action definitions.</param>
    /// <param name="ignoreCachable">If set to true, all actions will be treated equally. The result will then contain the same lists for cachable and uncachable.</param>
    /// <returns></returns>
    public static AggregationResult Aggregate( IControllerTemplate seed, bool ignoreCachable = false ) {

      StringBuilder actions           = new StringBuilder();
      StringBuilder uncachableActions = new StringBuilder();

      // If ignoreCachable is set, we'll aggregate them all into "actions"
      
      foreach( Action action in seed.Actions ) {
        if( action.Uncachable && ignoreCachable ) {
          uncachableActions.Append(  action.Name + "," );
        } else {
          actions.Append( action.Name + "," );
        }
      }
        
      string actionsCachable = actions.ToString();
      actionsCachable = ( actionsCachable.Length > 0 )
                          ? actionsCachable.Substring( 0, actionsCachable.Length - 1 )
                          : actionsCachable;

      string actionsUncachable = uncachableActions.ToString();
      actionsUncachable = ( actionsUncachable.Length > 0 )
                          ? actionsUncachable.Substring( 0, actionsUncachable.Length - 1 )
                          : actionsUncachable;
      var controllerData =
          new {controllerName = NameHelper.UpperCamelCase( seed.Name ), actionList = actionsCachable };
      var uncachableControllerData =
          new {controllerName = NameHelper.UpperCamelCase( seed.Name ), actionList = actionsUncachable };

      AggregationResult result = new AggregationResult {
                                                         Cachable = ActionTemplate.FormatSmart( controllerData ),
                                                         Uncachable = ActionTemplate.FormatSmart( uncachableControllerData )
                                                       };

      if( ignoreCachable ) {
        result.Uncachable = result.Cachable;
      }

      return result;
    }
  }
}
