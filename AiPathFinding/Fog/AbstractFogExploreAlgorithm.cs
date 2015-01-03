using System;
using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Abstract parent class for fog exploration algorithms.
    /// </summary>
    public abstract class AbstractFogExploreAlgorithm
    {
        #region Fields

        private static readonly List<Node> VisitedNodes = new List<Node>();
        private static readonly List<Node> DiscardedNodes = new List<Node>();
        private readonly List<Node> _backtrackedNodes = new List<Node>();

        /// <summary>
        /// Contains all the instances of the fog explore algorithms.
        /// </summary>
        private static readonly Dictionary<FogExploreName, AbstractFogExploreAlgorithm> Algorithms =
            new Dictionary<FogExploreName, AbstractFogExploreAlgorithm>
            {
                {
                    FogExploreName.DepthFirst,
                    new DepthFirstAlgorithm()
                }
            };

        #endregion

        #region Methods

        public static void Reset()
        {
            VisitedNodes.Clear();
            DiscardedNodes.Clear();
        }

        public static List<Node> RemoveKnownFoggyNodes(List<Node> foggyNodes)
        {
            var valid = new List<Node>();
            var invalid = VisitedNodes.Concat(DiscardedNodes).Where(i => !i.KnownToPlayer && i.Edges.Count(e => e != null && e.GetOtherNode(i).KnownToPlayer) == 0).ToArray();

            if (invalid.Length == 0)
                return foggyNodes;

            foreach (var f in foggyNodes)
            {
                var ok = true;

                foreach (var i in invalid)
                    foreach (var e in i.Edges)
                        if (e != null && e.GetOtherNode(i) == f)
                            if (!i.KnownToPlayer && !e.GetOtherNode(i).KnownToPlayer)
                                ok = false;

                if (ok)
                    valid.Add(f);
            }

            return valid;
        }

        public static Tuple<Node, Node[], Node[]> ExploreFog(FogExploreName name, Node position, Graph graph, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog)
        {
            // call instance method
            return Algorithms[name].ExploreFog(position, graph, ignoreNodes, getCostFromNode, addCostToNode, moveInFog);
        }

        private Tuple<Node, Node[], Node[]> ExploreFog(Node position, Graph graph, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog)
        {
            // prepare data
            var currentNode = position;
            DiscardedNodes.AddRange(VisitedNodes);
            VisitedNodes.Clear();
            _backtrackedNodes.Clear();

            // move on to first foggy tile
            moveInFog(position, new[] { position }, _backtrackedNodes.ToArray(), "Moving onto first foggy node " + position, false);
            
            // loop until a way is found or we are stuck
            while (true)
            {
                // move
                var oldCost = getCostFromNode(currentNode);
                currentNode = ChooseNextNode(currentNode, graph, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray());

                while (currentNode == null)
                {
                    // stuck: have to backtrack

                    // if we can't backtrack, return without result
                    if (VisitedNodes.Count == 1)
                    {
                        // update cost
                        addCostToNode(position, oldCost + position.Cost);

                        // add backtracking algorithm step
                        _backtrackedNodes.Add(VisitedNodes.Last());
                        moveInFog(position, new[] { position }.Concat(VisitedNodes).ToArray(),
                            _backtrackedNodes.ToArray(), "Backtracking to node " + position, true);
                        _backtrackedNodes.Add(position);

                        var bar = new List<Node>();
                        bar.AddRange(VisitedNodes);
                        bar.AddRange(_backtrackedNodes);
                        return new Tuple<Node, Node[], Node[]>(null, new Node[0], bar.ToArray());
                    }

                    // backtrack
                    DiscardedNodes.Add(VisitedNodes.Last());
                    _backtrackedNodes.Add(VisitedNodes.Last());
                    VisitedNodes.Remove(VisitedNodes.Last());
                    oldCost += VisitedNodes.Last().Cost;
                    currentNode = ChooseNextNode(VisitedNodes.Last(), graph, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray());

                    // update cost
                    Console.WriteLine("Backtracking to node " + VisitedNodes[VisitedNodes.Count - 1] + ".");
                    addCostToNode(VisitedNodes[VisitedNodes.Count - 1], oldCost);

                    // draw step
                    moveInFog(VisitedNodes.Last(), new[] {position}.Concat(VisitedNodes).ToArray(),
                        _backtrackedNodes.ToArray(), "Backtracking to node " + VisitedNodes.Last(), true);
                }

                // update cost
                addCostToNode(currentNode, oldCost + currentNode.Cost);

                // update path
                VisitedNodes.Add(currentNode);

                // if the tile is foggy, move to it
                if (!currentNode.KnownToPlayer)
                    moveInFog(currentNode, new[] { position }.Concat(VisitedNodes).ToArray(), _backtrackedNodes.ToArray(), "Moving in fog to cell " + currentNode, false);

                // if we found an exit or the target, return
                if (currentNode.EntityOnNode == Entity.Target || currentNode.KnownToPlayer)
                    // return node because it's the target
                    return new Tuple<Node, Node[], Node[]>(currentNode, new[] {position}.Concat(VisitedNodes).ToArray(),
                        _backtrackedNodes.ToArray());
            }
        }

        protected abstract Node ChooseNextNode(Node position, Graph graph, Node[] ignoreNodes, Node[] visitedNodes);

        #endregion
    }
}
