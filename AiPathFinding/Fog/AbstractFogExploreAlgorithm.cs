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

        /// <summary>
        /// Contains all nodes that have (ever) been visited.
        /// </summary>
        private static readonly List<Node> VisitedNodes = new List<Node>();

        /// <summary>
        /// Contains all nodes that have (ever) been discarded.
        /// </summary>
        private static readonly List<Node> DiscardedNodes = new List<Node>();

        /// <summary>
        /// Contains all nodes where the algorithm, in the current iteration, has had to backtrack.
        /// </summary>
        private readonly List<Node> _backtrackedNodes = new List<Node>();

        /// <summary>
        /// Contains all the instances of the fog explore algorithms.
        /// </summary>
        private static readonly Dictionary<FogExploreName, AbstractFogExploreAlgorithm> Algorithms =
            new Dictionary<FogExploreName, AbstractFogExploreAlgorithm>
            {
                {
                    FogExploreName.MinCost,
                    new MinCostAlgorithm()
                }
            };

        #endregion

        #region Implemented Methods

        /// <summary>
        /// Resets all static data.
        /// </summary>
        public static void Reset()
        {
            VisitedNodes.Clear();
            DiscardedNodes.Clear();
        }

        /// <summary>
        /// Expects an array of foggy nodes and filters out all nodes where it would not make sense to start looking for a path.
        /// </summary>
        /// <param name="foggyNodes">Foggy nodes</param>
        /// <returns>Nodes where it's reasonable to explore</returns>
        public static List<Node> RemoveKnownFoggyNodes(List<Node> foggyNodes)
        {
            // invalid nodes are those that have been visited and that border on a clear node
            var invalid = VisitedNodes.Concat(DiscardedNodes).Where(i => !i.KnownToPlayer && i.Edges.Count(e => e != null && e.GetOtherNode(i).KnownToPlayer) == 0).ToArray();

            // if no nodes are invalid there is nothing to filter out
            if (invalid.Length == 0)
                return foggyNodes;

            // add all nodes to valid list that don't border on a invalid node
            var valid = new List<Node>();
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

            // return valid nodes
            return valid;
        }

        //TODO: migrate ignored nodes (previously known foggy nodes) from pathfinding to here...
        /// <summary>
        /// Explores fog.
        /// </summary>
        /// <param name="name">Name of the method how to chose new, unknown foggy node</param>
        /// <param name="position">Node where the exploration starts</param>
        /// <param name="ignoreNodes">Nodes to ignore, eg from prevous fog explorations</param>
        /// <param name="getCostFromNode">Function that returns the cost of a node</param>
        /// <param name="addCostToNode">Action that updates the cost of a node</param>
        /// <param name="moveInFog">Action that moves onto another node in the fog</param>
        /// <returns>Tuple containing the node where the fog has been exited, an array of the path that has been taken and an array of nodes where there has been backtracking</returns>
        public static Tuple<Node, Node[], Node[]> ExploreFog(FogExploreName name, Node position, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog)
        {
            // call instance method
            return Algorithms[name].ExploreFog(position, ignoreNodes, getCostFromNode, addCostToNode, moveInFog);
        }

        /// <summary>
        /// Explores fog.
        /// </summary>
        /// <param name="name">Name of the method how to chose new, unknown foggy node</param>
        /// <param name="position">Node where the exploration starts</param>
        /// <param name="ignoreNodes">Nodes to ignore, eg from prevous fog explorations</param>
        /// <param name="getCostFromNode">Function that returns the cost of a node</param>
        /// <param name="addCostToNode">Action that updates the cost of a node</param>
        /// <param name="moveInFog">Action that moves onto another node in the fog</param>
        /// <returns>Tuple containing the node where the fog has been exited, an array of the path that has been taken and an array of nodes where there has been backtracking</returns>
        private Tuple<Node, Node[], Node[]> ExploreFog(Node position, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog)
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
                currentNode = ChooseNextNode(currentNode, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray());

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
                    currentNode = ChooseNextNode(VisitedNodes.Last(), ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray());

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

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Determines to which node to move to.
        /// </summary>
        /// <param name="position">Current position</param>
        /// <param name="ignoreNodes">All nodes to ignore</param>
        /// <param name="visitedNodes">All nodes that have previously been visited</param>
        /// <returns></returns>
        protected abstract Node ChooseNextNode(Node position, Node[] ignoreNodes, Node[] visitedNodes);

        #endregion
    }
}
