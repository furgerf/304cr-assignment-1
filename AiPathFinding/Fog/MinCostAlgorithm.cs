using System;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Uses the cost of the node as metric.
    /// </summary>
    public class MinCostAlgorithm : AbstractFogExploreAlgorithm
    {
        #region Methods

        protected override int GetMetric(Node candidate, Func<Node, int> getDistanceToTarget)
        {
            return candidate.Cost;
        }

        #endregion
    }
}
