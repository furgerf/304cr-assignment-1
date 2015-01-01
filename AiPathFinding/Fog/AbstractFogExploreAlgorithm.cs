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
            // add algorithm step for entering the fog
            //var cost = getCostFromNode(position);
            //moveInFog(g => DrawStep(g, new List<Node> { position }, new List<Node>(),
            //        new Dictionary<Node, int> { { position, cost } }), "foo");//, -1, -2, getCostFromNode(position), null));
            // prepare data
            var currentNode = position;
            DiscardedNodes.AddRange(VisitedNodes);
            VisitedNodes.Clear();
            _backtrackedNodes.Clear();
            //var cost = new Dictionary<Node, int>();
            //DiscardedNodes.Clear();

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
                        Console.WriteLine("Updating cost of " + position + " to " + (oldCost + position.Cost));
                        addCostToNode(position, oldCost + position.Cost);

                        // add backtracking algorithm step
                        //var foobar = foo.Concat(bar).ToDictionary(n => n, getCostFromNode);
                        //moveInFog(g => DrawStep(g, foo, bar, foobar), -1, -2, "bar");//, -1, -2, oldCost + position.Cost, null));

                        _backtrackedNodes.Add(VisitedNodes.Last());
                        moveInFog(VisitedNodes.Last(), new[] { position }.Concat(VisitedNodes).ToArray(),
                            _backtrackedNodes.ToArray(), "Backtracking to node " + position, true);
                        _backtrackedNodes.Add(position);

                        //_backtrackedNodes.Add(position);
                        //moveInFog(VisitedNodes.Last(), new[] { position }.Concat(VisitedNodes).ToArray(),
                        //    _backtrackedNodes.ToArray(), "Backtracking to node " + position, true);

                        //addStep(new AlgorithmStep(g =>
                        //{
                        //    DrawStep(g, new List<Node>(), bar.Concat(new List<Node>{position}), foobar);
                        //    previousStep.DrawStep(g);
                        //}, previousStep.Explored, previousStep.Explorable, oldCost + position.Cost));

                        var foo = new List<Node> { position };
                        var bar = new List<Node>();
                        bar.AddRange(VisitedNodes);
                        bar.AddRange(_backtrackedNodes);
                        //return new Tuple<Node, AlgorithmStep>(null, new AlgorithmStep(g => DrawStep(g, foo, bar, foobar), -1, -2, oldCost + position.Cost, null));
                        return new Tuple<Node, Node[], Node[]>(null, foo.ToArray(), bar.ToArray());
                    }

                    // backtrack
                    //Console.WriteLine("Backtracking to node " + VisitedNodes[VisitedNodes.Count - 2]);
                    DiscardedNodes.Add(VisitedNodes.Last());
                    _backtrackedNodes.Add(VisitedNodes.Last());
                    VisitedNodes.Remove(VisitedNodes.Last());
                    oldCost += VisitedNodes.Last().Cost;
                    currentNode = ChooseNextNode(VisitedNodes.Last(), graph, ignoreNodes, VisitedNodes.Concat(DiscardedNodes).ToArray());

                    // update cost
                    Console.WriteLine("Backtracking to node " + VisitedNodes[VisitedNodes.Count - 1] + " and updating cost to " + oldCost);
                    addCostToNode(VisitedNodes[VisitedNodes.Count - 1], oldCost);

                    // draw step
                    //var allNodes = new List<Node> { position };
                    //allNodes.AddRange(VisitedNodes);
                    //var allDiscNodes = new List<Node>();
                    //allDiscNodes.AddRange(_backtrackedNodes);
                    //var costMap = allNodes.Concat(allDiscNodes).ToDictionary(n => n, getCostFromNode);
                    //!moveInFog(g => DrawStep(g, allNodes, allDiscNodes, costMap), -1, -2, "foobar");//, -1, -2, oldCost, null));
                    moveInFog(VisitedNodes.Last(), new[] {position}.Concat(VisitedNodes).ToArray(),
                        _backtrackedNodes.ToArray(), "Backtracking to node " + VisitedNodes.Last(), true);
                }

                // update cost
                Console.WriteLine("Updating cost of " + currentNode + " to " + (oldCost + currentNode.Cost));
                addCostToNode(currentNode, oldCost + currentNode.Cost);

                // update path
                VisitedNodes.Add(currentNode);

                // draw step
                //var nodes = new List<Node> { position };
                //nodes.AddRange(VisitedNodes);
                //var discNodes = new List<Node>();
                //discNodes.AddRange(_backtrackedNodes);
                //var costs = nodes.Concat(discNodes).ToDictionary(n => n, getCostFromNode);
                //!moveInFog(g => DrawStep(g, nodes, discNodes, costs), -1, -2, "fsdfsdf");//, -1, -2, getCostFromNode(currentNode), null));
                //var asdf = new int[VisitedNodes.Count];
                
                if (!currentNode.KnownToPlayer)
                    moveInFog(currentNode, new[] { position }.Concat(VisitedNodes).ToArray(), _backtrackedNodes.ToArray(), "Moving in fog to cell " + currentNode, false);

                // are we done?
                if (currentNode.EntityOnNode == Entity.Target || currentNode.KnownToPlayer)
                    // current node is not part of the foggy path, remove it
                    //VisitedNodes.Remove(currentNode);

                    // return node because it's the target
                    return new Tuple<Node, Node[], Node[]>(currentNode, new[] {position}.Concat(VisitedNodes).ToArray(),
                        _backtrackedNodes.ToArray());
            }
        }

        /// <summary>
        /// Draws a step in the current stage of the exploration.
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="nodes">Nodes that form the current path</param>
        /// <param name="discardedNodes">All nodes that led to a dead end</param>
        /// <param name="nodeCosts">Dictionary that maps a cost to each node</param>
        /// <param name="withLabels">True if cost labels should be drawn</param>
        //private static void DrawStep(Graphics g, IList<Node> nodes, IList<Node> discardedNodes, Dictionary<Node, int> nodeCosts,
        //    bool withLabels = true)
        //{
        //    // draw discarded node labels
        //    if (withLabels)
        //        foreach (var n in discardedNodes)
        //            g.DrawString(nodeCosts[n].ToString(CultureInfo.InvariantCulture),
        //                new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.DarkOrange,
        //                MainForm.MapPointToCanvasRectangle(n.Location));

        //    // iterate over all active nodes
        //    for (var i = 0; i < nodes.Count; i++)
        //    {
        //        // draw icon
        //        var p1 = MainForm.MapPointToCanvasRectangle(nodes[i].Location);
        //        //g.DrawIcon(Resources.runner, p1.X + p1.Width/2, p1.Y + p1.Width/2);
        //        Utils.DrawTransparentImage(g, Resources.runner.ToBitmap(), p1.X + p1.Width / 2, p1.Y + p1.Height / 2, 0.3f + (float) i / nodes.Count / 0.7f);

        //        // draw label
        //        if (withLabels)
        //            g.DrawString(nodeCosts[nodes[i]].ToString(CultureInfo.InvariantCulture),
        //                new Font("Microsoft Sans Serif", 12, FontStyle.Bold), Brushes.Red,
        //                MainForm.MapPointToCanvasRectangle(nodes[i].Location));
        //    }

        //    foreach (var n in discardedNodes)
        //    {
        //        // draw icon
        //        var p1 = MainForm.MapPointToCanvasRectangle(n.Location);
        //        //g.DrawIcon(Resources.runner, p1.X + p1.Width/2, p1.Y + p1.Width/2);
        //        Utils.DrawTransparentImage(g, Resources.runner.ToBitmap(), p1.X + p1.Width / 2, p1.Y + p1.Height / 2, 0.5f, true);
        //    }
        //}

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
