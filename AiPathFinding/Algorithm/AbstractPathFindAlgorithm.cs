﻿using System;
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
                // path was found, look for alternative, equal-cost paths
                var step = FindAlternativePaths(playerNode, targetNode);
                //AddStep(step);
                Console.WriteLine("Looking for alternative paths required " + (_steps.Count - mainSteps) +
                                  " steps and took " +
                                  watch.ElapsedMilliseconds + "ms");
                watch.Reset();

                // <----------------- segment ends here ----------------->
                SegmentCompleted(step.DrawStep);
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
            while (true)
            {
                var foggyPossibilities = new List<Node>();
                foggyPossibilities.AddRange(AbstractFogExploreAlgorithm.RemoveKnownFoggyNodes(FoggyNodes));

                if (foggyPossibilities.Count == 0)
                    break;

                var clearPossibilities = new List<Node>();
                foreach (var n in foggyPossibilities)
                {
                    if (n.Edges.Count(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue) != 1)
                        throw new Exception("Dunno which clear neighbor to choose...");
                    clearPossibilities.Add(n.Edges.First(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue).GetOtherNode(n));
                }

                // pick "best" foggy node
                var clearFavorite = FogSelector.SelectFoggyNode(playerNode, targetNode, clearPossibilities.ToArray(), fogMethod,
                    CostFromNodeToNode);
                if (clearFavorite.Edges.Count(e => e != null && !e.GetOtherNode(clearFavorite).KnownToPlayer) != 1)
                    throw new Exception("Dunno which foggy neighbor to chose");
                var foggyNode = clearFavorite.Edges.First(e => e != null && !e.GetOtherNode(clearFavorite).KnownToPlayer).GetOtherNode(clearFavorite);

                var minCost = int.MaxValue;
                foreach (var e in foggyNode.Edges)
                    if (e != null)
                    {
                        if (GetCostFromNode(playerNode) == int.MaxValue || GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                            continue;

                        var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                        if (c > 0 && c < minCost)
                            minCost = c;
                    }

                while (minCost == int.MaxValue)
                {
                    foggyPossibilities.Remove(foggyNode);
                    clearPossibilities.Clear();
                    foreach (var n in foggyPossibilities)
                    {
                        if (n.Edges.Count(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue) != 1)
                            throw new Exception("Dunno which neighbor to choose...");
                        clearPossibilities.Add(n.Edges.First(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue).GetOtherNode(n));
                    }
                    foggyNode = FogSelector.SelectFoggyNode(playerNode, targetNode, clearPossibilities.ToArray(), fogMethod,
                        CostFromNodeToNode);

                    minCost = int.MaxValue;
                    foreach (var e in foggyNode.Edges)
                        if (e != null)
                        {
                            if (GetCostFromNode(playerNode) == int.MaxValue || GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                                continue;

                            var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                            if (c > 0 && c < minCost)
                                minCost = c;
                        }
                }

                Console.WriteLine("Best node target investigate fog is " + foggyNode);

                // add step for graphics stuff
                var min = int.MaxValue;
                Node[] path = null;
                //Node clearNeighborToFoggyNode = null;
                //var partialCostBackup = PartialCost;
                foreach (var e in foggyNode.Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(foggyNode)) < min)
                    {
                        min = GetCostFromNode(e.GetOtherNode(foggyNode));
                        //PartialCost = partialCostBackup + min;
                        path = GetPath(playerNode, e.GetOtherNode(foggyNode));
                        //clearNeighborToFoggyNode = e.GetOtherNode(foggyNode);
                    }

                if (path != null)
                {
                    var pathList = path.ToList();
                    //pathList.Add(foggyNode);

                    if (path[0] == playerNode)
                        pathList.RemoveAt(0);
                    
                    path = pathList.ToArray();
                }

                // segment completed stuff
                var pathCostData = MovePath(path, "Moving towards fog at " + foggyNode);

                // add currently foggy nodes to all foggy nodes
                _allFoggyNodes.AddRange(FoggyNodes);

                // <----------------- segment ends here ----------------->
                //MoveFog(foggyNode, "Moving onto fog at " + foggyNode);

                SegmentCompleted(g => DrawPath(g, path, pathCostData));

                Console.WriteLine("Setting cost of " + foggyNode + " to " + (PartialCost + foggyNode.Cost));
                AddCostToNode(foggyNode, PartialCost + foggyNode.Cost);


                // <----------------- new segment starts here ----------------->
                // explore fog
                var result = AbstractFogExploreAlgorithm.ExploreFog(fogExploreName, foggyNode, Graph,
                    _allFoggyNodes.ToArray(), GetCostFromNode, AddCostToNode, MoveFog);

                //AddStep(result.Item2);
                SegmentCompleted(DrawFoggyPath(result.Item2, result.Item3));

                // <----------------- segment ends here ----------------->
                //Console.WriteLine("Cost of path upon return: " + result.Item2.CurrentCost);
                if (result.Item1 == null)
                {
                    // no other exit found, try other foggy node
                    //Console.WriteLine("Setting cost of " + clearNeighborToFoggyNode + " to " + (result.Item3 + clearNeighborToFoggyNode.Cost));
                    Console.WriteLine("Node " + foggyNode + " didn't yield anything useful, trying next node...");
                    //foreach (var n in UpdatedNodes)
                    //    AddCostToNode(n, int.MaxValue);
                    //AddCostToNode(clearNeighborToFoggyNode, PartialCost);
                    //FoggyNodes.Clear();
                    //FindPath(clearNeighborToFoggyNode, targetNode, fogMethod, fogExploreName);

                    //AddCostToNode(clearNeighborToFoggyNode, result.Item3 + clearNeighborToFoggyNode.Cost);
                    //PartialCost += result.Item3 - GetCostFromNode(clearNeighborToFoggyNode) + clearNeighborToFoggyNode.Cost;
                    //foreach (var n in UpdatedNodes)
                    //    AddCostToNode(n, GetCostFromNode(n) + fogCost);
                    //FoggyNodes.Remove(foggyNode);
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

                //if (result.Item3 <= PartialCost)
                //    throw new Exception("Cost through fog shouldn't be lower than current cost");
                //PartialCost += result.Item3;

                //var stepCount = _steps.Count;
                // start pathfinding again
                FoggyNodes.Clear();
                Console.WriteLine("Found another part of the known map, restarting pathfinding...");
                FindPath(result.Item1, targetNode, fogMethod, fogExploreName);

                //for (var i = stepCount; i < _steps.Count; i++)
                //    _steps[i].AddDrawing(result.Item2);
                return;
            }
        }

        protected void MoveNode(Node n)
        {
            PartialCost += n.Cost;
        }

        protected int ExploredFoggyCells;
        protected int ExploredClearCells;

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
        /// <param name="withPlayer">True if the path should be traced with the player icon rather than a line</param>
        /// <returns>AlgorithmStep that shows the step</returns>
        protected abstract Action<Graphics> GetAlgorithmStep(Node player, Node target, bool withCost = true);

        protected abstract Node[] GetPath(Node player, Node target);

        /// <summary>
        /// Gets all alternative paths to the target.
        /// </summary>
        /// <param name="player">Start of the paths</param>
        /// <param name="target">Taget of the paths</param>
        /// <returns>AlgorithmStep that shows all alternatives</returns>
        protected abstract AlgorithmStep GetAlternativesStep(Node player, Node target);

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
        protected abstract AlgorithmStep FindAlternativePaths(Node playerNode, Node targetNode);

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
            //AdditionalCost = 0;
            //_partialDrawAction = g => { };
            _segmentDrawActions.Clear();
            PartialCost = 0;
            FoggyNodes.Clear();
            _allFoggyNodes.Clear();
            AbstractFogExploreAlgorithm.Reset();
        }

        #endregion
    }
}
