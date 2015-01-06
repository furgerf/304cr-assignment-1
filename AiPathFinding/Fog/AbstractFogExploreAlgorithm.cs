using System;
using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Common;
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

        /// <summary>
        /// Explores fog using the specified method and starting on the specified node.
        /// </summary>
        /// <param name="name">Name of the method how to chose adjacent unknown foggy node</param>
        /// <param name="position">Node where the exploration starts</param>
        /// <param name="ignoreNodes">Nodes to ignore, eg. from previous fog explorations or those that are "known" foggy nodes from pathfinding</param>
        /// <param name="getCostFromNode">Function that returns the cost of a node</param>
        /// <param name="addCostToNode">Action that updates the cost of a node</param>
        /// <param name="moveInFog">Action that moves onto another node in the fog</param>
        /// <param name="getDistanceToTarget">Function that returns the distance to the target</param>
        /// <returns>Tuple containing the node where the fog has been exited, an array of the path that has been taken and an array of nodes where there has been backtracking</returns>
        public static Tuple<Node, Node[], Node[]> ExploreFog(FogExploreName name, Node position, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog, Func<Node, int> getDistanceToTarget)
        {
            // call instance method
            return Utils.FogExploreAlgorithms[name].ExploreFog(position, ignoreNodes, getCostFromNode, addCostToNode, moveInFog, getDistanceToTarget);
        }

        /// <summary>
        /// Explores fog using the specified method and starting on the specified node.
        /// </summary>
        /// <param name="position">Node where the exploration starts</param>
        /// <param name="ignoreNodes">Nodes to ignore, eg. from previous fog explorations or those that are "known" foggy nodes from pathfinding</param>
        /// <param name="getCostFromNode">Function that returns the cost of a node</param>
        /// <param name="addCostToNode">Action that updates the cost of a node</param>
        /// <param name="moveInFog">Action that moves onto another node in the fog</param>
        /// <param name="getDistanceToTarget">Function that returns the distance to the target</param>
        /// <returns>Tuple containing the node where the fog has been exited, an array of the path that has been taken and an array of nodes where there has been backtracking</returns>
        private Tuple<Node, Node[], Node[]> ExploreFog(Node position, Node[] ignoreNodes, Func<Node, int> getCostFromNode, Action<Node, int> addCostToNode, Action<Node, Node[], Node[], string, bool> moveInFog, Func<Node, int> getDistanceToTarget)
        {
            // prepare data
            var currentNode = position;
            DiscardedNodes.AddRange(VisitedNodes);
            VisitedNodes.Clear();
            VisitedNodes.Add(position);
            _backtrackedNodes.Clear();

            // move on to first foggy tile
            moveInFog(position, new[] { position }, _backtrackedNodes.ToArray(), "Moving onto first foggy node " + position, false);

            currentNode = ChooseNextNode(currentNode, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray(), getDistanceToTarget, getCostFromNode);

            if (currentNode == null)
            {
                Console.WriteLine("Started exploring fog on a node that has no neighbor that can be explored, aborting...");
                return new Tuple<Node, Node[], Node[]>(null, new Node[0], new Node[0]);
            }
            currentNode = position;
            
            // loop until a way is found or we are stuck
            while (true)
            {
                // move
                var oldCost = getCostFromNode(currentNode);
                currentNode = ChooseNextNode(currentNode, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray(), getDistanceToTarget, getCostFromNode);

                while (currentNode == null)
                {
                    // stuck: have to backtrack

                    // if we can't backtrack, return without result
                    if (VisitedNodes.Count == 1)
                    {
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
                    currentNode = ChooseNextNode(VisitedNodes.Last(), ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray(), getDistanceToTarget, getCostFromNode);

                    // update cost
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
        /// <param name="getDistanceToTarget">Function that returns the heuristic distance to the target</param>
        /// <param name="getCostOfNode">Function that returns the cost of the node</param>
        /// <returns>Best node to move to</returns>
        protected Node ChooseNextNode(Node position, Node[] ignoreNodes, Node[] visitedNodes,
            Func<Node, int> getDistanceToTarget, Func<Node, int> getCostOfNode)
        {
            // prepare data
            var clearNeighborsCost = new List<Node>();
            var foggyNeighborsCost = new List<Node>();
            var lastResortNeighborsCost = new List<Node>();

            // iterate over all edges and add them to proper lists
            foreach (var e in position.Edges)
            {
                if (e == null)
                    continue;

                var neighbor = e.GetOtherNode(position);

                // if the neighbor is the target, move there
                if (neighbor.EntityOnNode == Entity.Target)
                    return neighbor;

                // if the node should be ignored but we have no way to go, we could use them...
                if (ignoreNodes.Contains(neighbor) && !visitedNodes.Contains(neighbor))
                    lastResortNeighborsCost.Add(neighbor);

                // ignore some neighbors...
                if ((neighbor.Cost == int.MaxValue || ignoreNodes.Contains(neighbor) || getCostOfNode(neighbor) != int.MaxValue))
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
            if (clearNeighborsCost.Count > 0)
            {
                possibilities =
                    clearNeighborsCost.Where(n => GetMetric(n, getDistanceToTarget) == clearNeighborsCost.Min(nn => GetMetric(nn, getDistanceToTarget))).ToArray();
                return possibilities[rnd.Next(possibilities.Length)];
            }

            // if there are not even foggy nodes, we are fcked
            if (foggyNeighborsCost.Count == 0)
            {
                if (lastResortNeighborsCost.Count > 0)
                {
                    // well, maybe there is a naughty way after all...
                    possibilities = lastResortNeighborsCost.Where(n => GetMetric(n, getDistanceToTarget) == lastResortNeighborsCost.Min(nn => GetMetric(nn, getDistanceToTarget))).ToArray();
                    return possibilities[rnd.Next(possibilities.Length)];
                }

                return null;
            }
                
            // chose among foggy nodes
            possibilities = foggyNeighborsCost.Where(n => GetMetric(n, getDistanceToTarget) == foggyNeighborsCost.Min(nn => GetMetric(nn, getDistanceToTarget))).ToArray();
            return possibilities[rnd.Next(possibilities.Length)];
        }

        /// <summary>
        /// Gets the metric of a candidate which is used to pick the best candidate.
        /// </summary>
        /// <param name="candidate">Node that is a candidate</param>
        /// <param name="getDistanceToTarget">Function that returns a heuristic distance of a node to the target</param>
        /// <returns>Metric</returns>
        protected abstract int GetMetric(Node candidate, Func<Node, int> getDistanceToTarget);

        #endregion
    }
}
