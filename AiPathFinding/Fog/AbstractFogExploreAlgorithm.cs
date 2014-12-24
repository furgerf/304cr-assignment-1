using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;
using AiPathFinding.Properties;
using AiPathFinding.View;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Abstract parent class for fog exploration algorithms.
    /// </summary>
    public abstract class AbstractFogExploreAlgorithm
    {
        #region Fields

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

        /// <summary>
        /// Explores the fog.
        /// </summary>
        /// <param name="name">Name of the algorithm to be used</param>
        /// <param name="position">Node where the exploration should start</param>
        /// <param name="graph">Graph of the map</param>
        /// <param name="ignoreNodes">All nodes that should be ignored (because they are adjacent to known non-foggy tiles)</param>
        /// <param name="addCostToNode">Action that updates a node's cost</param>
        /// <param name="getCostFromNode">Function that retrieves a node's cost</param>
        /// <param name="previousStep">Previous algorithm step which will be drawn as well</param>
        /// <param name="addStep">Action that adds an algorithm step</param>
        /// <returns>The node that is either the target or that is not foggy and the action that draws the path through the fog</returns>
        public static Tuple<Node, Action<Graphics>> ExploreFog(FogExploreName name, Node position, Graph graph, Node[] ignoreNodes, Action<Node, int> addCostToNode, Func<Node, int> getCostFromNode, AlgorithmStep previousStep, Action<AlgorithmStep> addStep)
        {
            // call instance method
            return Algorithms[name].ExploreFog(position, graph, ignoreNodes, addCostToNode, getCostFromNode, previousStep, addStep);
        }

        /// <summary>
        /// Explores the fog.
        /// </summary>
        /// <param name="position">Node where the exploration should start</param>
        /// <param name="graph">Graph of the map</param>
        /// <param name="ignoreNodes">All nodes that should be ignored (because they are adjacent to known non-foggy tiles)</param>
        /// <param name="addCostToNode">Action that updates a node's cost</param>
        /// <param name="getCostFromNode">Function that retrieves a node's cost</param>
        /// <param name="previousStep">Previous algorithm step which will be drawn as well</param>
        /// <param name="addStep">Action that adds an algorithm step</param>
        /// <returns>The node that is either the target or that is not foggy and the action that draws the path through the fog</returns>
        private Tuple<Node, Action<Graphics>> ExploreFog(Node position, Graph graph, Node[] ignoreNodes, Action<Node, int> addCostToNode, Func<Node, int> getCostFromNode, AlgorithmStep previousStep, Action<AlgorithmStep> addStep)
        {
            // add algorithm step for entering the fog
            addStep(new AlgorithmStep(g =>
            {
                DrawStep(g, new List<Node> { position }, new List<Node>(), getCostFromNode);
                previousStep.DrawStep(g);
            }, previousStep.Explored, previousStep.Explorable));

            // prepare data
            var visitedNodes = new List<Node>();
            var discardedNodes = new List<Node>();
            var currentNode = position;

            // loop until a way is found or we are stuck
            while (true)
            {
                // move
                var oldCost = getCostFromNode(currentNode);
                currentNode = ChooseNextNode(currentNode, graph, ignoreNodes, visitedNodes.Concat(discardedNodes).ToArray());

                while (currentNode == null)
                {
                    // stuck: have to backtrack

                    // can't backtrack, return without result
                    if (visitedNodes.Count == 0)
                        return null;

                    // backtrack
                    discardedNodes.Add(visitedNodes.Last());
                    visitedNodes.Remove(visitedNodes.Last());
                    oldCost = getCostFromNode(visitedNodes.Last());
                    currentNode = ChooseNextNode(visitedNodes.Last(), graph, ignoreNodes, visitedNodes.Concat(discardedNodes).ToArray());

                    // draw step
                    var allNodes = new List<Node> { position };
                    allNodes.AddRange(visitedNodes);
                    var allDiscNodes = new List<Node>();
                    allDiscNodes.AddRange(discardedNodes);
                    addStep(new AlgorithmStep(g =>
                    {
                        DrawStep(g, allNodes, allDiscNodes, getCostFromNode);
                        previousStep.DrawStep(g);
                    }, previousStep.Explored, previousStep.Explorable));
                }

                // update cost
                Console.WriteLine("Updating cost of " + currentNode + " to " + (oldCost + currentNode.Cost));
                addCostToNode(currentNode, oldCost + currentNode.Cost);

                // update path
                visitedNodes.Add(currentNode);

                // draw step
                var nodes = new List<Node> { position };
                nodes.AddRange(visitedNodes);
                var discNodes = new List<Node>();
                discNodes.AddRange(discardedNodes);
                addStep(new AlgorithmStep(g =>
                {
                    DrawStep(g, nodes, discNodes, getCostFromNode);
                    previousStep.DrawStep(g);
                }, previousStep.Explored, previousStep.Explorable));

                // are we done?
                if (currentNode.EntityOnNode == Entity.Target || currentNode.KnownToPlayer)
                    // return node because it's the target
                    return new Tuple<Node, Action<Graphics>>(currentNode, g =>
                    {
                        previousStep.DrawStep(g);
                        DrawStep(g, nodes, discNodes, getCostFromNode, false);
                    });
            }
        }

        /// <summary>
        /// Draws a step in the current stage of the exploration.
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="nodes">Nodes that form the current path</param>
        /// <param name="discardedNodes">All nodes that led to a dead end</param>
        /// <param name="getCost">Function that retrieves the cost of a node</param>
        /// <param name="withLabels">True if cost labels should be drawn</param>
        private static void DrawStep(Graphics g, IList<Node> nodes, IEnumerable<Node> discardedNodes, Func<Node, int> getCost,
            bool withLabels = true)
        {
            // draw discarded node labels
            if (withLabels)
                foreach (var n in discardedNodes)
                    g.DrawString(getCost(n).ToString(CultureInfo.InvariantCulture),
                        new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.DarkOrange,
                        MainForm.MapPointToCanvasRectangle(n.Location));

            // iterate over all active nodes
            for (var i = 0; i < nodes.Count; i++)
            {
                // draw icon
                var p1 = MainForm.MapPointToCanvasRectangle(nodes[i].Location);
                g.DrawIcon(Resources.runner, p1.X + p1.Width/2, p1.Y + p1.Width/2);

                // draw label
                if (withLabels)
                    g.DrawString(getCost(nodes[i]).ToString(CultureInfo.InvariantCulture),
                        new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.Red,
                        MainForm.MapPointToCanvasRectangle(nodes[i].Location));
            }
        }

        /// <summary>
        /// Implementation-dependent method that determines where to go in the fog.
        /// </summary>
        /// <param name="position">Current position</param>
        /// <param name="graph">Graph</param>
        /// <param name="ignoreNodes">Nodes that should be ignored</param>
        /// <param name="visitedNodes">Node that have already been visited</param>
        /// <returns>Node where the player should move to</returns>
        protected abstract Node ChooseNextNode(Node position, Graph graph, Node[] ignoreNodes, Node[] visitedNodes);

        #endregion
    }
}
