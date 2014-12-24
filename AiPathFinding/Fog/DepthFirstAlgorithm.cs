using System;
using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    public class DepthFirstAlgorithm : AbstractFogExploreAlgorithm
    {
        protected override Node ChooseNextNode(Node position, Graph graph, Node[] ignoreNodes, Node[] visitedNodes)
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
                    if (!visitedNodes.Contains(neighbor))
                        foggyNeighborsCost.Add(neighbor);
            }

            var rnd = new Random();
            Node[] possibilities;
            if (!ignoreNodes.Contains(position) && clearNeighborsCost.Count > 0)
            {
                possibilities =
                    clearNeighborsCost.Where(n => n.Cost == clearNeighborsCost.Min(nn => nn.Cost)).ToArray();
                return possibilities[rnd.Next(possibilities.Length)];
            }

            if (foggyNeighborsCost.Count == 0)
                return null;

            possibilities = foggyNeighborsCost.Where(n => n.Cost == foggyNeighborsCost.Min(nn => nn.Cost)).ToArray();

            Console.WriteLine(possibilities.Length + " possibilities.");
            return possibilities[rnd.Next(possibilities.Length)];
        }
    }
}
