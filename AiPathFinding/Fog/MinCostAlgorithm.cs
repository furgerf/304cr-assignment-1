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

        protected override Node ChooseNextNode(Node position, Node[] ignoreNodes, Node[] visitedNodes)
        {
            // prepare data
            var clearNeighborsCost = new List<Node>();
            var foggyNeighborsCost = new List<Node>();

            // iterate over all edges and add them to proper lists
            foreach (var e in position.Edges)
            {
                if (e == null)
                    continue;

                var neighbor = e.GetOtherNode(position);

                // if the neighbor is the target, move there
                if (neighbor.EntityOnNode == Entity.Target)
                    return neighbor;

                // ignore some neighbors...
                if (neighbor.Cost == int.MaxValue || ignoreNodes.Contains(neighbor))
                    continue;

                // add neighbor to proper list
                if (neighbor.KnownToPlayer)
                    clearNeighborsCost.Add(neighbor);
                else
                    if (!visitedNodes.Contains(neighbor))
                        foggyNeighborsCost.Add(neighbor);
            }

            // choose randomly among cheapest nodes
            var rnd = new Random();
            Node[] possibilities;
            
            // if possible, choose among non-foggy nodes
            if (!ignoreNodes.Contains(position) && clearNeighborsCost.Count > 0)
            {
                possibilities =
                    clearNeighborsCost.Where(n => n.Cost == clearNeighborsCost.Min(nn => nn.Cost)).ToArray();
                return possibilities[rnd.Next(possibilities.Length)];
            }

            // if there are not even foggy nodes, we are stuck
            if (foggyNeighborsCost.Count == 0)
                return null;

            // chose among foggy nodes
            possibilities = foggyNeighborsCost.Where(n => n.Cost == foggyNeighborsCost.Min(nn => nn.Cost)).ToArray();
            return possibilities[rnd.Next(possibilities.Length)];
        }

        #endregion
    }
}
