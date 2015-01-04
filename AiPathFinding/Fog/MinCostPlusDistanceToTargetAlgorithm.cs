using System;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Uses the sum of cost of the node and distance to the target as metric.
    /// </summary>
    public class MinCostPlusDistanceToTargetAlgorithm : AbstractFogExploreAlgorithm
    {
        #region Methods

        protected override int GetMetric(Node candidate, Func<Node, int> getDistanceToTarget)
        {
            return getDistanceToTarget(candidate) + candidate.Cost;
        }

        #endregion
    }
}
