using System;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
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
