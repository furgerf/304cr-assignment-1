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
    /// Abstract parent class for data finding algorithms. The actual data finding happens here with the implementation details being deferred to the implementing classes.
    /// </summary>
    public abstract class AbstractPathFindAlgorithm
    {
        #region Fields

        /// <summary>
        /// Public immutable access to the algorithm steps.
        /// </summary>
        public AlgorithmStep[] Steps { get { return _steps.ToArray(); } }

        /// <summary>
        /// List of steps that were computed during the data finding. These are used to display the progress of the algorithm in detail.
        /// </summary>
        private readonly List<AlgorithmStep> _steps = new List<AlgorithmStep>();

        /// <summary>
        /// Reference to the graph instance of the model. Basis of the data used for data finding.
        /// </summary>
        public static Graph Graph { get; set; }

        /// <summary>
        /// Name of the algorithm implementation.
        /// </summary>
        private PathFindName Name { get; set; }

        /// <summary>
        /// List of all foggy nodes that could be explored in the currently un-foggy area.
        /// </summary>
        protected List<Node> FoggyNodes = new List<Node>();

        /// <summary>
        /// List of all (previously) encountered foggy cells. These are being kept so as not to exit fog to a clear area that has already been explored.
        /// </summary>
        private readonly List<Node> _allFoggyNodes = new List<Node>();

        /// <summary>
        /// List of all nodes that were updated during this run of the pathfinding. These are being kept in order to only draw algorithm cost of the current pathfinding section.
        /// </summary>
        protected readonly List<Node> UpdatedNodes = new List<Node>();

        /// <summary>
        /// Cost of the data that the player has currently travelled.
        /// </summary>
        protected int PartialCost { get; private set; }

        /// <summary>
        /// Last node the player has travelled to. This is merely there to prevent accidental teleportation and could be removed.
        /// </summary>
        private Node _lastNode;

        /// <summary>
        /// Number of foggy cells that have been explored.
        /// </summary>
        protected int ExploredFoggyCells;

        /// <summary>
        /// Number of clear cells that have been explored.
        /// </summary>
        protected int ExploredClearCells;

        /// <summary>
        /// Actions that draw the previously completed segments.
        /// </summary>
        private readonly List<Action<Graphics>> _segmentDrawActions = new List<Action<Graphics>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate algorithm with name.
        /// </summary>
        /// <param name="name">Name of the data finding algorithm</param>
        protected AbstractPathFindAlgorithm(PathFindName name)
        {
            Name = name;

            // TODO: move all algorithm instances to a dictionary here (like in the fogexplore stuff)
        }

        #endregion

        #region Implemented Methods

        /// <summary>
        /// Attempts target find a data.
        /// </summary>
        /// <param name="playerNode">Starting node of the search</param>
        /// <param name="targetNode">Goal node of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        public void FindPath(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // <----------------- new segment starts here ----------------->
            // prepare data
            Console.WriteLine("\node* " + Name + " is attempting target find a data player " + playerNode + " target " + targetNode + ".");
            var watch = Stopwatch.StartNew();

            // set last node to be the player node if it is null, eg. when this is the first instance of the data finding
            _lastNode = _lastNode ?? playerNode;

            // prepare implementation-dependent data
            PrepareData(playerNode, targetNode);

            Console.WriteLine("Preparation took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            // find data
            var pathFound = FindShortestPath(playerNode, targetNode);

            var mainSteps = _steps.Count;
            Console.WriteLine("Main calculation required " + _steps.Count + " steps and took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            if (pathFound)
            {
                // data was found!
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
                // no data was found. if fog is present, explore it
                Console.WriteLine("No data found, attempting target explore fog...");
                ExploreFog(playerNode, targetNode, fogMethod, fogExploreName);
            }
        }

        /// <summary>
        /// Explores fog.
        /// </summary>
        /// <param name="playerNode">Starting node of the search</param>
        /// <param name="targetNode">Goal node of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        private void ExploreFog(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // this is where fog was left after the last attempt to exit the current clear area
            Node clearNodeWhereFogWasLeft = null;

            // try different fog nodes until an exit is found or we run out of options
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

                if (minCost == int.MaxValue)
                    throw new Exception("The node has no clear neighbor. Shouldn't happen though...");

                Console.WriteLine("Best node target investigate fog is " + foggyNode);

                // find data through clear map part to node
                var min = int.MaxValue;
                Node[] path = null;
                foreach (var e in foggyNode.Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(foggyNode)) < min && e.GetOtherNode(foggyNode).KnownToPlayer)
                    {
                        min = GetCostFromNode(e.GetOtherNode(foggyNode));
                            path = GetPath(playerNode, e.GetOtherNode(foggyNode));
                    }

                // modify data
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
                            // if the data from the player node to the next foggy node crosses the last clear node
                            // (which is where we currently are)
                            // then remove the first part of the data (playernode -> clearnode)
                            while (pathList[0] != clearNodeWhereFogWasLeft)
                                pathList.RemoveAt(0);
                        }
                        else
                        {
                            // otherwise, get the data from the playernode to the clear node,
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

                // update cost of foggy node
                AddCostToNode(foggyNode, PartialCost + foggyNode.Cost);

                // <----------------- segment ends here ----------------->
                SegmentCompleted(g => DrawPath(g, path, pathCostData));

                // <----------------- new segment starts here ----------------->
                // explore fog
                var result = AbstractFogExploreAlgorithm.ExploreFog(fogExploreName, foggyNode, _allFoggyNodes.ToArray(), GetCostFromNode, AddCostToNode, MoveFog);
                
                // <----------------- segment ends here ----------------->
                SegmentCompleted(DrawFoggyPath(result.Item2, result.Item3));

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
                    Console.WriteLine("Found data target target through fog!");
                    break;
                }
                
                // exit was another fog-less area, start pathfinding
                MoveNode(result.Item1);
                FoggyNodes.Clear();
                Console.WriteLine("Found another part of the known map, restarting pathfinding...");
                FindPath(result.Item1, targetNode, fogMethod, fogExploreName);

                return;
            }

            // TODO draw algorithmstep or something that says that no data was found
        }

        /// <summary>
        /// Move player onto node.
        /// </summary>
        /// <param name="node">Node where the player should move to</param>
        protected void MoveNode(Node node)
        {
            if (_lastNode.Edges.Count(e => e != null && e.GetOtherNode(_lastNode) == node) != 1)
                throw new Exception("Teleporting is not allowed!");

            // update partial cost since player has travelled
            PartialCost += node.Cost;

            // update last node
            _lastNode = node;
        }

        /// <summary>
        /// Move through the fog to a new node and draw the complete foggy data.
        /// </summary>
        /// <param name="node">Node where the player moved to</param>
        /// <param name="path">Nodes that the player has already visited</param>
        /// <param name="backtrackedNodes">Nodes that the player has visited but had to backtrack from</param>
        /// <param name="comment">Comment of the algorithmstep that will be drawn</param>
        /// <param name="backtracking">True if moving to the new node is backtracking</param>
        private void MoveFog(Node node, Node[] path, Node[] backtrackedNodes, string comment, bool backtracking)
        {
            // we only move in the fog here
            if (node.KnownToPlayer)
                throw new Exception("The node is not foggy");

            // if we move to a new node, keep track of that
            if (!backtracking)
                ExploredFoggyCells++;

            // move onto the node
            MoveNode(node);

            // create new algorithm step
            CreateStep(DrawFoggyPath(path, backtrackedNodes), comment);
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

        /// <summary>
        /// Creates a new algorithmstep with the draw action and the comment for the new step.
        /// </summary>
        /// <param name="drawStep">The action that draws the new step</param>
        /// <param name="comment">The comment explaining what happened in the step</param>
        protected void CreateStep(Action<Graphics> drawStep, string comment)
        {
            // create local copy of currently completed segments
            var previousActions = new List<Action<Graphics>>();
            previousActions.AddRange(_segmentDrawActions);

            _steps.Add(new AlgorithmStep(g =>
            {
                // draw current (partial) segment
                drawStep(g);

                // draw previous segments
                foreach (var a in previousActions)
                    a(g);
            }, ExploredClearCells + ExploredFoggyCells, Graph.PassibleNodeCount, PartialCost, comment));
        }

        /// <summary>
        /// Draws a data through the fog and distinguishes between nodes of the current data and those that have been abandonned.
        /// </summary>
        /// <param name="path">Current data</param>
        /// <param name="backtrackedNodes">Nodes where the player had to backtrack</param>
        /// <returns></returns>
        private Action<Graphics> DrawFoggyPath(Node[] path, Node[] backtrackedNodes)
        {
            // calculate current data costs and store them locally
            var pathCost = new int[path.Length];
            for (var i = 0; i < path.Length; i++)
                pathCost[i] = GetCostFromNode(path[i]);
            var backtrackedCost = new int[backtrackedNodes.Length];
            for (var i = 0; i < backtrackedNodes.Length; i++)
                backtrackedCost[i] = GetCostFromNode(backtrackedNodes[i]);

            // draw both paths (though the backtracked nodes don't technically form a data...)
            return g =>
            {
                DrawPath(g, path, pathCost);
                DrawPath(g, backtrackedNodes, backtrackedCost, true);
            };
        }

        /// <summary>
        /// Draws a data.
        /// </summary>
        /// <param name="g">Reference to the graphics where the stuff should be drawn to</param>
        /// <param name="path">Nodes to be drawn</param>
        /// <param name="cost">Cost of the nodes to be drawn</param>
        /// <param name="backtrackingPath"></param>
        private static void DrawPath(Graphics g, IList<Node> path, int[] cost, bool backtrackingPath = false)
        {
            if (path.Count != cost.Length)
                throw new ArgumentException("Don't have the same number of nodes and node costs");

            // convert data into more useful form for drawing
            var data = new Tuple<Node, int, bool>[path.Count];
            for (var i = 0; i < path.Count; i++)
                data[i] = new Tuple<Node, int, bool>(path[i], cost[i], backtrackingPath);

            // call actual drawing method
            DrawPath(g, data);
        }

        /// <summary>
        /// Draws a data.
        /// </summary>
        /// <param name="g">Reference to the graphics where the stuff should be drawn to</param>
        /// <param name="data">Nodes to be drawn with a flag showing whether the node is backtracked or not and the cost of the node</param>
        private static void DrawPath(Graphics g, IList<Tuple<Node, int, bool>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var r = MainForm.MapPointToCanvasRectangle(data[i].Item1.Location);
                var p = new Point(r.Location.X + r.Width / 2, r.Location.Y + r.Height / 2);
                Utils.DrawTransparentImage(g, Resources.runner.ToBitmap(), p, Utils.GetPathOpacity(i, data.Count), data[i].Item3);

                g.DrawString(data[i].Item2.ToString(CultureInfo.InvariantCulture),
                    new Font("Microsoft Sans Serif", 12, FontStyle.Bold), data[i].Item3 ? Brushes.Orange : Brushes.Red,
                    r.Location);
            }
        }

        /// <summary>
        /// Called when a segment (clear/foggy path exploration) has been completed. Adds an action to draw the current segment to the list of previous segments.
        /// </summary>
        /// <param name="segmentDrawAction">Action that draws the segment</param>
        private void SegmentCompleted(Action<Graphics> segmentDrawAction)
        {
            _segmentDrawActions.Insert(0, segmentDrawAction);
        }

        /// <summary>
        /// Retrieves the cost of the data between nodes.
        /// </summary>
        /// <param name="start">One of the nodes</param>
        /// <param name="end">The other node</param>
        /// <returns>Cost between the nodes</returns>
        protected int CostFromNodeToNode(Node start, Node end)
        {
            return GetCostFromNode(end) - GetCostFromNode(start);
        }

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

        #region Abstract Methods

        /// <summary>
        /// Gets the action to draw a step of the algorithm from the player to the target.
        /// </summary>
        /// <param name="player">Starting node</param>
        /// <param name="target">End node</param>
        /// <param name="withCost">True if the cost on the nodes should be displayed as well</param>
        /// <returns>Action that draws the step</returns>
        protected abstract Action<Graphics> GetAlgorithmStep(Node player, Node target, bool withCost = true);

        /// <summary>
        /// Gets a path from the player to the target (in a known and explored section of the map).
        /// </summary>
        /// <param name="player">Starting node of the path</param>
        /// <param name="target">Target node of the path</param>
        /// <returns>Array of nodes from player to target</returns>
        protected abstract Node[] GetPath(Node player, Node target);

        /// <summary>
        /// Gets all alternative paths to the target.
        /// </summary>
        /// <param name="player">Start of the paths</param>
        /// <param name="target">Taget of the paths</param>
        /// <returns>AlgorithmStep that shows all alternatives</returns>
        protected abstract Action<Graphics> GetAlternativesStep(Node player, Node target);

        /// <summary>
        /// Adds a cost to a node. Depends on algorithm implementation.
        /// </summary>
        /// <param name="node">Node to be updated</param>
        /// <param name="cost">New cost of the node</param>
        protected abstract void AddCostToNode(Node node, int cost);

        /// <summary>
        /// Retrieves the cost of the data to a node. Depends on algorithm implementation.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Cost of the data to the node</returns>
        protected abstract int GetCostFromNode(Node node);

        /// <summary>
        /// Actual algorithm implementation: Find alternative paths.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract Action<Graphics> FindAlternativePaths(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Find shortest data.
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

        #endregion
    }
}
