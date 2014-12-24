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
    public abstract class AbstractFogExploreAlgorithm
    {
        private static readonly Dictionary<FogExploreName, AbstractFogExploreAlgorithm> Algorithms = new Dictionary<FogExploreName, AbstractFogExploreAlgorithm>();

        static AbstractFogExploreAlgorithm()
        {
            Algorithms.Add(FogExploreName.DepthFirst, new DepthFirstAlgorithm());
        }

        public static Tuple<Node, Action<Graphics>> ExploreFog(FogExploreName name, Node position, Graph graph, Node[] ignoreNodes, Action<Node, int> addCostToNode, Func<Node, int> getCostFromNode, AlgorithmStep previousStep, Action<AlgorithmStep> addStep)
        {
            return Algorithms[name].ExploreFog(position, graph, ignoreNodes, addCostToNode, getCostFromNode, previousStep, addStep);
        }

        private Tuple<Node, Action<Graphics>> ExploreFog(Node position, Graph graph, Node[] ignoreNodes, Action<Node, int> addCostToNode, Func<Node, int> getCostFromNode, AlgorithmStep previousStep, Action<AlgorithmStep> addStep)
        {
            addStep(new AlgorithmStep(g =>
            {
                DrawStep(g, new List<Node> { position }, new List<Node>(), getCostFromNode);
                previousStep.DrawStep(g);
            }, previousStep.Explored, previousStep.Explorable));


            var visitedNodes = new List<Node>();
            var discardedNodes = new List<Node>();
            var currentNode = position;
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

                var nodes = new List<Node> { position };
                nodes.AddRange(visitedNodes);
                var discNodes = new List<Node>();
                discNodes.AddRange(discardedNodes);

                addStep(new AlgorithmStep(g =>
                {
                    DrawStep(g, nodes, discNodes, getCostFromNode);
                    previousStep.DrawStep(g);
                }, previousStep.Explored, previousStep.Explorable));

                if (currentNode.EntityOnNode == Entity.Target || currentNode.KnownToPlayer)
                    // return node because it's the target
                    return new Tuple<Node, Action<Graphics>>(currentNode, g =>
                    {
                        previousStep.DrawStep(g);
                        DrawStep(g, nodes, discNodes, getCostFromNode, false);
                    });
            }
        }

        private void DrawStep(Graphics g, IList<Node> nodes, IEnumerable<Node> discardedNodes, Func<Node, int> getCost,
            bool withLabels = true)
        {
            if (withLabels)
                foreach (var n in discardedNodes)
                    g.DrawString(getCost(n).ToString(CultureInfo.InvariantCulture),
                        new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.DarkOrange,
                        MainForm.MapPointToCanvasRectangle(n.Location));

            for (var i = 0; i < nodes.Count; i++)
            {
                var p1 = MainForm.MapPointToCanvasRectangle(nodes[i].Location);
                g.DrawIcon(Resources.runner, p1.X + p1.Width/2, p1.Y + p1.Width/2);

                if (withLabels)
                    g.DrawString(getCost(nodes[i]).ToString(CultureInfo.InvariantCulture),
                        new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.Red,
                        MainForm.MapPointToCanvasRectangle(nodes[i].Location));
            }
        }

        protected abstract Node ChooseNextNode(Node position, Graph graph, Node[] ignoreNodes, Node[] visitedNodes);
    }
}
