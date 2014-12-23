using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    public class DepthFirstAlgorithm : AbstractFogExploreAlgorithm
    {
        protected override Node ChooseNextNode(Node position, Graph graph, Node[] ignoreNodes)
        {
            var clearNeighborsCost = new List<Node>();
            var foggyNeighborsCost = new List<Node>();
            
            foreach (var e in position.Edges)
            {
                if (e == null)
                    continue;

                var neighbor = e.GetOtherNode(position);

                if (neighbor.EntityOnNode == Entity.Target)
                    return neighbor;

                if (neighbor.Cost == int.MaxValue || ignoreNodes.Contains(neighbor))
                    continue;

                if (neighbor.KnownToPlayer)
                    clearNeighborsCost.Add(neighbor);
                else
                    foggyNeighborsCost.Add(neighbor);
            }

            if (!ignoreNodes.Contains(position) && clearNeighborsCost.Count > 0)
                return clearNeighborsCost.OrderBy(n => n.Cost).First();

            if (foggyNeighborsCost.Count > 0)
                return foggyNeighborsCost.OrderBy(n => n.Cost).First();

            return null;
        }
    }
}
