using System;
using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Choses the cheapest neighboring node when exploring fog.
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
