using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AiPathFinding.Common;
using AiPathFinding.Fog;
using AiPathFinding.Model;
using AiPathFinding.Properties;
using AiPathFinding.View;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Abstract parent class for path finding algorithms.
    /// </summary>
    public abstract class AbstractPathFindAlgorithm
    {
        #region Fields

        public AlgorithmStep[] Steps { get { return _steps.ToArray(); } }

        /// <summary>
        /// List of steps that were computed during the path finding.
        /// </summary>
        private readonly List<AlgorithmStep> _steps = new List<AlgorithmStep>();

        /// <summary>
        /// Reference target the graph instance of the model.
        /// </summary>
        public static Graph Graph { get; set; }

        /// <summary>
        /// Name of the concrete algorithm.
        /// </summary>
        private PathFindName Name { get; set; }

        /// <summary>
        /// List of all foggy nodes that could be explored in the currently un-foggy area.
        /// </summary>
        protected List<Node> FoggyNodes = new List<Node>();

        /// <summary>
        /// List of all (previously) encountered foggy cells.
        /// </summary>
        private readonly List<Node> _allFoggyNodes = new List<Node>();

        protected readonly List<Node> UpdatedNodes = new List<Node>();

        protected int PartialCost { get; private set; }

        private Node _lastNode;

        protected int ExploredFoggyCells;

        protected int ExploredClearCells;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate algorithm with name.
        /// </summary>
        /// <param name="name">Name of the path finding algorithm</param>
        protected AbstractPathFindAlgorithm(PathFindName name)
        {
            Name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts target find a path.
        /// </summary>
        /// <param name="playerNode">Starting point of the search</param>
        /// <param name="targetNode">Goal of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        public void FindPath(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // <----------------- new segment starts here ----------------->
            // prepare data
            Console.WriteLine("\n* " + Name + " is attempting target find a path player " + playerNode + " target " + targetNode + ".");
            var watch = Stopwatch.StartNew();

            _lastNode = _lastNode ?? playerNode;

            PrepareData(playerNode, targetNode);

            Console.WriteLine("Preparation took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            // find path
            var pathFound = FindShortestPath(playerNode, targetNode);

            var mainSteps = _steps.Count;
            Console.WriteLine("Main calculation required " + _steps.Count + " steps and took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            if (pathFound)
            {
                // path was found!
                CreateStep(GetAlgorithmStep(playerNode, targetNode), "Path to target " + targetNode + " found!");

                // look for alternative, equal-cost paths
                var step = FindAlternativePaths(playerNode, targetNode);
                CreateStep(step, "Alternative paths to the target");
                Console.WriteLine("Looking for alternative paths required " + (_steps.Count - mainSteps) +
                                  " steps and took " +
                                  watch.ElapsedMilliseconds + "ms");
                watch.Reset();

                // <----------------- segment ends here ----------------->
                SegmentCompleted(step);
            }
            else
            {
                // no path was found. if fog is present, explore it
                Console.WriteLine("No path found, attempting target explore fog...");
                ExploreFog(playerNode, targetNode, fogMethod, fogExploreName);
            }
        }

        /// <summary>
        /// Explores fog.
        /// </summary>
        /// <param name="playerNode">Starting point of the search</param>
        /// <param name="targetNode">Goal of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        private void ExploreFog(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            Node clearNodeWhereFogWasLeft = null;
            while (true)
            {
                // find possible foggy nodes to investigate
                var foggyPossibilities = new List<Node>();
                foggyPossibilities.AddRange(AbstractFogExploreAlgorithm.RemoveKnownFoggyNodes(FoggyNodes));

                if (foggyPossibilities.Count == 0)
                    break;

                // find clear nodes adjacent to the foggy nodes (need those since they're part of the pathfinding)
                var clearPossibilities = new List<Node>();
                foreach (var n in foggyPossibilities)
                {
                    if (n.Edges.Count(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue) != 1)
                        throw new Exception("Dunno which clear neighbor to choose...");
                    clearPossibilities.Add(n.Edges.First(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue).GetOtherNode(n));
                }

                // pick "best" clear neighbor to foggy node node
                var clearFavorite = FogSelector.SelectFoggyNode(playerNode, targetNode, clearPossibilities.ToArray(), fogMethod,
                    CostFromNodeToNode);
                if (clearFavorite.Edges.Count(e => e != null && !e.GetOtherNode(clearFavorite).KnownToPlayer) != 1)
                    throw new Exception("Dunno which foggy neighbor to chose");

                // the foggy node to chose is the foggy neighbor to the clear favorite picked by the fogselector
                var foggyNode = clearFavorite.Edges.First(e => e != null && !e.GetOtherNode(clearFavorite).KnownToPlayer).GetOtherNode(clearFavorite);

                // maybe the foggy node has a cheaper clear neighbor to move to
                var minCost = int.MaxValue;
                foreach (var e in foggyNode.Edges)
                    if (e != null)
                    {
                        if (GetCostFromNode(playerNode) == int.MaxValue ||
                            GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                            continue;

                        var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                        if (c > 0 && c < minCost)
                            minCost = c;
                    }

                while (minCost == int.MaxValue)
                {
                    throw new Exception("is this really being executed?");
                    //foggyPossibilities.Remove(foggyNode);
                    //clearPossibilities.Clear();
                    //foreach (var n in foggyPossibilities)
                    //{
                    //    if (n.Edges.Count(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue) != 1)
                    //        throw new Exception("Dunno which neighbor to choose...");
                    //    clearPossibilities.Add(n.Edges.First(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue).GetOtherNode(n));
                    //}
                    //foggyNode = FogSelector.SelectFoggyNode(playerNode, targetNode, clearPossibilities.ToArray(), fogMethod,
                    //    CostFromNodeToNode);

                    //minCost = int.MaxValue;
                    //foreach (var e in foggyNode.Edges)
                    //    if (e != null)
                    //    {
                    //        if (GetCostFromNode(playerNode) == int.MaxValue || GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                    //            continue;

                    //        var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                    //        if (c > 0 && c < minCost)
                    //            minCost = c;
                    //    }
                }

                Console.WriteLine("Best node target investigate fog is " + foggyNode);

                // find path through clear map part to node
                var min = int.MaxValue;
                Node[] path = null;
                foreach (var e in foggyNode.Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(foggyNode)) < min && e.GetOtherNode(foggyNode).KnownToPlayer)
                    {
                        min = GetCostFromNode(e.GetOtherNode(foggyNode));
                            path = GetPath(playerNode, e.GetOtherNode(foggyNode));
                    }

                // modify path
                if (path != null)
                {
                    var pathList = path.ToList();

                    // remove starting node if it's the player node because that one has already been drawn
                    if (path[0] == playerNode)
                        pathList.RemoveAt(0);

                    // if we aren't looking at the first foggy node in this part, do some more modifications
                    if (clearNodeWhereFogWasLeft != null)
                    {
                        if (path.Contains(clearNodeWhereFogWasLeft))
                        {
                            // if the path from the player node to the next foggy node crosses the last clear node
                            // (which is where we currently are)
                            // then remove the first part of the path (playernode -> clearnode)
                            while (pathList[0] != clearNodeWhereFogWasLeft)
                                pathList.RemoveAt(0);
                        }
                        else
                        {
                            // otherwise, get the path from the playernode to the clear node,
                            // reverse that part (to get clearnode -> playernode) and insert it
                            var morePath = GetPath(playerNode, clearNodeWhereFogWasLeft);
                            pathList.InsertRange(0, morePath.Reverse());
                        }
                    }

                    path = pathList.ToArray();
                }

                // segment completed stuff
                var pathCostData = MovePath(path, "Moving towards fog at " + foggyNode);

                // add currently foggy nodes to all foggy nodes
                _allFoggyNodes.AddRange(FoggyNodes);

                // <----------------- segment ends here ----------------->
                SegmentCompleted(g => DrawPath(g, path, pathCostData));

                AddCostToNode(foggyNode, PartialCost + foggyNode.Cost);

                // <----------------- new segment starts here ----------------->
                // explore fog
                var result = AbstractFogExploreAlgorithm.ExploreFog(fogExploreName, foggyNode, Graph,
                    _allFoggyNodes.ToArray(), GetCostFromNode, AddCostToNode, MoveFog);

                //AddStep(result.Item2);
                SegmentCompleted(DrawFoggyPath(result.Item2, result.Item3));

                // <----------------- segment ends here ----------------->
                if (result.Item1 == null)
                {
                    // no other exit found, try other foggy node
                    Console.WriteLine("Node " + foggyNode + " didn't yield anything useful, trying next node...");

                    if (result.Item3.Last().Edges.Count(e => e != null && e.GetOtherNode(result.Item3.Last()).KnownToPlayer) != 1)
                        throw new Exception("We didn't return on a clear tile :/");
                    
                    // set the clear tile where we returned
                    clearNodeWhereFogWasLeft = result.Item3.Last().Edges.First(e => e != null && e.GetOtherNode(result.Item3.Last()).KnownToPlayer).GetOtherNode(result.Item3.Last());

                    continue;
                }

                // exit was found
                if (result.Item1.EntityOnNode == Entity.Target)
                {
                    // exit was the target, WE ARE DONE!
                    Console.WriteLine("Found path target target through fog!");
                    break;
                }
                
                // exit was another fog-less area, start pathfinding
                MoveNode(result.Item1);
                FoggyNodes.Clear();
                Console.WriteLine("Found another part of the known map, restarting pathfinding...");
                FindPath(result.Item1, targetNode, fogMethod, fogExploreName);

                return;
            }
        }

        protected void MoveNode(Node n)
        {
            if (_lastNode.Edges.Count(e => e != null && e.GetOtherNode(_lastNode) == n) != 1)
                throw new Exception("Teleporting is not allowed!");

            PartialCost += n.Cost;

            _lastNode = n;
        }

        private void MoveFog(Node n, Node[] path, Node[] backtrackedNodes, string comment, bool backtracking)
        {
            if (n.KnownToPlayer)
                throw new Exception();

            if (!backtracking)
                ExploredFoggyCells++;

            MoveNode(n);

            var foo = DrawFoggyPath(path, backtrackedNodes);

            CreateStep(foo, comment);
        }

        private int[] MovePath(Node[] nodes, string comment)
        {
            var cost = new int[nodes.Length];
            for (var i = 0; i < nodes.Length; i++)
            {
                MoveNode(nodes[i]);
                cost[i] = PartialCost;
                var i1 = i;
                CreateStep(g => DrawPath(g, nodes.SubArray(0, i1 + 1).ToArray(), cost.SubArray(0, i1 + 1)), comment);
            }

            return cost;
        }

        protected void CreateStep(Action<Graphics> drawStep, string comment)
        {
            var previousActions = new List<Action<Graphics>>();
            previousActions.AddRange(_segmentDrawActions);

            var newStep = new AlgorithmStep(g =>
            {
                // draw previous segments
                foreach (var a in previousActions)
                    a(g);

                // draw current (partial) segment
                drawStep(g);
            }, ExploredClearCells + ExploredFoggyCells, Graph.PassibleNodeCount, PartialCost, comment);

            _steps.Add(newStep);
        }

        private readonly List<Action<Graphics>> _segmentDrawActions = new List<Action<Graphics>>();

        private Action<Graphics> DrawFoggyPath(Node[] path, Node[] backtrackedNodes)
        {
            var pathCost = new int[path.Length];
            for (var i = 0; i < path.Length; i++)
                pathCost[i] = GetCostFromNode(path[i]);
            var backtrackedCost = new int[backtrackedNodes.Length];
            for (var i = 0; i < backtrackedNodes.Length; i++)
                backtrackedCost[i] = GetCostFromNode(backtrackedNodes[i]);

            return g =>
            {
                DrawPath(g, path, pathCost);
                DrawPath(g, backtrackedNodes, backtrackedCost, true);
            };
        }

        protected void DrawPath(Graphics g, Node[] path, int[] cost, bool backtrackingPath = false)
        {
            var foo = new Tuple<Node, bool>[path.Length];
            for (var i = 0; i < path.Length; i++)
                foo[i] = new Tuple<Node, bool>(path[i], backtrackingPath);

            DrawPath(g, foo, cost);
        }

        protected void DrawPath(Graphics g, Tuple<Node, bool>[] path, int[] cost)
        {
            for (var i = 0; i < path.Length; i++)
            {
                var r = MainForm.MapPointToCanvasRectangle(path[i].Item1.Location);
                var p = new Point(r.Location.X + r.Width / 2, r.Location.Y + r.Height / 2);
                Utils.DrawTransparentImage(g, Resources.runner.ToBitmap(), p,
                    0.3f + ((1 - (float)i / path.Length)) / 0.7f, path[i].Item2);

                g.DrawString(cost[i].ToString(CultureInfo.InvariantCulture),
                    new Font("Microsoft Sans Serif", 12, FontStyle.Bold), path[i].Item2 ? Brushes.Orange : Brushes.Red,
                    r.Location);
            }
        }

        private void SegmentCompleted(Action<Graphics> segmentDrawAction)
        {
            _segmentDrawActions.Insert(0, segmentDrawAction);

            //var oldAction = _partialDrawAction;
            //var newAction = new Action<Graphics>(g =>
            //{
            //    oldAction(g);
            //    segmentDrawAction(g);
            //});
            //_partialDrawAction = newAction;
        }

        /// <summary>
        /// Gets a step of the algorithm from the player to the target.
        /// </summary>
        /// <param name="player">Starting node</param>
        /// <param name="target">End node</param>
        /// <param name="withCost">True if the cost on the nodes should be displayed as well</param>
        /// <returns>AlgorithmStep that shows the step</returns>
        protected abstract Action<Graphics> GetAlgorithmStep(Node player, Node target, bool withCost = true);

        protected abstract Node[] GetPath(Node player, Node target);

        /// <summary>
        /// Gets all alternative paths to the target.
        /// </summary>
        /// <param name="player">Start of the paths</param>
        /// <param name="target">Taget of the paths</param>
        /// <returns>AlgorithmStep that shows all alternatives</returns>
        protected abstract Action<Graphics> GetAlternativesStep(Node player, Node target);

        /// <summary>
        /// Adds a cost to a node.
        /// </summary>
        /// <param name="node">Node to be updated</param>
        /// <param name="cost">New cost of the node</param>
        protected abstract void AddCostToNode(Node node, int cost);

        /// <summary>
        /// Retrieves the cost of the path to a node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Cost of the path to the node</returns>
        protected abstract int GetCostFromNode(Node node);

        /// <summary>
        /// Retrieves the cost of the path between nodes.
        /// </summary>
        /// <param name="start">One of the nodes</param>
        /// <param name="node">The other node</param>
        /// <returns>Cost between the nodes</returns>
        protected int CostFromNodeToNode(Node start, Node node)
        {
            return GetCostFromNode(node) - GetCostFromNode(start);
        }

        /// <summary>
        /// Actual algorithm implementation: Find alternative paths.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract Action<Graphics> FindAlternativePaths(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Find shortest path.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract bool FindShortestPath(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Prepare data for pathfinding.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract void PrepareData(Node playerNode, Node targetNode);

        /// <summary>
        /// Reset the algorithm data to run again.
        /// </summary>
        public virtual void ResetAlgorithm()
        {
            _steps.Clear();
            _lastNode = null;
            _segmentDrawActions.Clear();
            ExploredClearCells = 0;
            ExploredFoggyCells = 0;
            PartialCost = 0;
            FoggyNodes.Clear();
            _allFoggyNodes.Clear();
            AbstractFogExploreAlgorithm.Reset();
        }

        #endregion
    }
}
