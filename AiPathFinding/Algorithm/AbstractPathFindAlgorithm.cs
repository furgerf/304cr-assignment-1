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
    /// Abstract parent class for path finding algorithms. Contains logic that alternates between pathfinding and fog exploration (if necessary). The actual path finding is being deferred to the implementing classes.
    /// </summary>
    public abstract class AbstractPathFindAlgorithm
    {
        #region Public Fields

        /// <summary>
        /// Public immutable access to the algorithm steps.
        /// </summary>
        public AlgorithmStep[] Steps { get { return _steps.ToArray(); } }

        #endregion

        #region Protected Fields

        /// <summary>
        /// Reference to the graph instance of the model. Basis of the data used for path finding. This instance is common among all path find algorithms and should never be modified.
        /// </summary>
        protected static Graph Graph { get; set; }

        /// <summary>
        /// Number of clear cells that have been explored. This field is updated by the implementing algorithms since that is the only section where clear cells are being discovered.
        /// </summary>
        protected int ExploredClearCells;

        /// <summary>
        /// List of all foggy nodes that could be explored in the currently un-foggy area. Nodes will be added here during path finding by the implementing algorithms.
        /// </summary>
        protected readonly List<Node> FoggyNodes = new List<Node>();

        /// <summary>
        /// List of all nodes that were updated during this run of the pathfinding. This list is being maintained by the implementing algorithms. These are being kept in order to only draw algorithm cost of the current pathfinding section.
        /// </summary>
        protected readonly List<Node> UpdatedNodes = new List<Node>();

        #endregion

        #region Private Fields

        /// <summary>
        /// Cost of the path that the player has currently travelled. This field gets updated in two cases: The player is moving through a clear section to a foggy tile or the player is exploring a foggy section. Access only via the Move*-methods.
        /// </summary>
        private int PartialCost { get; set; }

        /// <summary>
        /// Number of foggy cells that have been explored. This field is updated by the MoveFog-method.
        /// </summary>
        private int _exploredFoggyCells;

        /// <summary>
        /// Name of the algorithm implementation. Passed via the constructor.
        /// </summary>
        private readonly PathFindName _name;

        /// <summary>
        /// Last node the player has travelled to. This is merely there to prevent accidental teleportation and could be removed for optimization.
        /// </summary>
        private Node _lastNode;

        /// <summary>
        /// List of steps that were computed during the path finding. These are used to display the progress of the algorithm in detail.
        /// </summary>
        private readonly List<AlgorithmStep> _steps = new List<AlgorithmStep>();

        /// <summary>
        /// List of all (previously) encountered foggy cells. These are being kept so as not to exit fog to a clear area that has already been explored.
        /// </summary>
        private readonly List<Node> _allFoggyNodes = new List<Node>();

        /// <summary>
        /// Actions that draw the previously completed segments.
        /// </summary>
        private readonly List<Action<Graphics>> _segmentDrawActions = new List<Action<Graphics>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate algorithm with name.
        /// </summary>
        /// <param name="name">Name of the path finding algorithm</param>
        protected AbstractPathFindAlgorithm(PathFindName name)
        {
            _name = name;
        }

        #endregion

        #region Implemented Methods

        /// <summary>
        /// Attempts to find a path from playerNode to targetNode, using fogMethod and fogExploreName to determine behavior in case no clear path is discovered.
        /// </summary>
        /// <param name="name">Name of the algorithm to use</param>
        /// <param name="playerNode">Starting node of the search</param>
        /// <param name="targetNode">Goal node of the search</param>
        /// <param name="fogMethod">Method to determine where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        public static void FindPath(PathFindName name, Node playerNode, Node targetNode, FogMethod fogMethod,
            FogExploreName fogExploreName)
        {
            var watch = Stopwatch.StartNew();

            // call instance method
            Utils.PathFindAlgorithms[name].FindPath(playerNode, targetNode, fogMethod, fogExploreName);
            
            Console.WriteLine("The complete calculation took " + watch.ElapsedMilliseconds + "ms and required " + Utils.PathFindAlgorithms[name]._steps.Count + " steps.");
        }

        /// <summary>
        /// Attempts to find a path from playerNode to targetNode, using fogMethod and fogExploreName to determine behavior in case no clear path is discovered.
        /// </summary>
        /// <param name="playerNode">Starting node of the search</param>
        /// <param name="targetNode">Goal node of the search</param>
        /// <param name="fogMethod">Method to determine where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        private void FindPath(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // <----------------- new segment starts here ----------------->
            // prepare data
            Console.WriteLine("\nAlgorithm " + _name + " is attempting to find a path from " + playerNode + " to " + targetNode + ".");
            var watch = Stopwatch.StartNew();

            // set last node to be the player node if it is null, eg. when this is the first instance of the path finding
            _lastNode = _lastNode ?? playerNode;

            // prepare implementation-dependent data
            PrepareData(playerNode, targetNode);

            Console.WriteLine("Preparation for path finding took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            // find path through clear section
            var mainSteps = _steps.Count;
            var pathFound = FindShortestPath(playerNode, targetNode);

            Console.WriteLine("Main calculation required " + (_steps.Count - mainSteps) + " steps and took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();
            mainSteps = _steps.Count;

            if (pathFound)
            {
                // path was found!
                CreateStep(GetAlgorithmStep(playerNode, targetNode), "Path to target " + targetNode + " found!");

                // look for alternative, equal-cost paths
                var step = FindAlternativePaths(playerNode, targetNode);
                PartialCost = GetCostFromNode(targetNode);
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
                Console.WriteLine("No path found, attempting target explore fog.");
                ExploreFog(playerNode, targetNode, fogMethod, fogExploreName);
            }
        }

        /// <summary>
        /// Explores fog with the player standing on clear node playerNode looking to reach targetNode. fogMethod determines selection of the foggy node to explore and fogExploreName determines how to chose nodes in the fog.
        /// </summary>
        /// <param name="playerNode">Starting node of the search</param>
        /// <param name="targetNode">Goal node of the search</param>
        /// <param name="fogMethod">Method to determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        private void ExploreFog(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // this is where fog was left after the last attempt to exit the current clear area
            Node clearNodeWhereFogWasLeft = null;

            var watch = Stopwatch.StartNew();
            Console.WriteLine("\nStarting fog exploration...");

            // try different foggy nodes until an exit is found or we run out of options
            while (true)
            {
                // add currently foggy nodes to all foggy nodes
                foreach (var n in FoggyNodes.Where(n => !_allFoggyNodes.Contains(n)))
                    _allFoggyNodes.Add(n);

                // find possible foggy nodes to investigate (filter out "useless" nodes)
                var foggyPossibilities = new List<Node>();
                foggyPossibilities.AddRange(AbstractFogExploreAlgorithm.RemoveKnownFoggyNodes(FoggyNodes));
                var removableNodes = RemoveLonelyFoggyNodes(foggyPossibilities);
                foggyPossibilities = foggyPossibilities.Except(removableNodes).Where(n => GetCostFromNode(n) == int.MaxValue).ToList();

                FoggyNodes.Clear();
                FoggyNodes.AddRange(foggyPossibilities);

                if (foggyPossibilities.Count == 0)
                    break;

                // find clear nodes adjacent to the foggy nodes (need those nodes since they're part of the pathfinding)
                var clearPossibilities = new List<Node>();
                foreach (var n in foggyPossibilities)
                {
                    var cheapestClearPossibility = n.Edges.Where(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) < int.MaxValue).Min(e => GetCostFromNode(e.GetOtherNode(n)));

                    clearPossibilities.Add(n.Edges.First(e => e != null && e.GetOtherNode(n).KnownToPlayer && GetCostFromNode(e.GetOtherNode(n)) == cheapestClearPossibility).GetOtherNode(n));
                }

                // pick "best" clear neighbor to foggy node node
                // this is the only place where fogMethod is being used
                var clearFavorite = FogSelector.SelectFoggyNode(playerNode, targetNode, clearPossibilities.ToArray(), fogMethod, GetCostFromNode);

                // the foggy node to chose is the foggy neighbor to the clear favorite picked by the fogselector
                var foggyNode = clearFavorite.Edges.First(e => e != null && !e.GetOtherNode(clearFavorite).KnownToPlayer && e.GetOtherNode(clearFavorite).Cost != int.MaxValue && FoggyNodes.Contains(e.GetOtherNode(clearFavorite))).GetOtherNode(clearFavorite);

                Console.WriteLine("Best node to investigate fog is " + foggyNode);

                // create algorithmstep showing possible foggy nodes
                CreateStep(g => DrawFoggyAlternatives(g, foggyNode, foggyPossibilities.ToArray()), "Possible foggy nodes to investigate.");

                Node[] path;

                if (clearNodeWhereFogWasLeft == null)
                {
                    // move from playernode to clearfavorite
                    path = GetPath(playerNode, clearFavorite);
                    if (path.Length > 0)
                        path = path.SubArray(1);
                }
                else
                {
                    // move from clearnodewherefogwasleft to playernode and from playernode to clearfavorite
                    if (clearNodeWhereFogWasLeft != clearFavorite)
                    {
                        var part1 = GetPath(playerNode, clearNodeWhereFogWasLeft).Reverse().ToArray();
                        if (part1.Length > 0)
                            part1 = part1.SubArray(0, part1.Length - 1);
                        var part2 = GetPath(playerNode, clearFavorite);

                        // if the two paths share common parts, there is no need to visit the playernode
                        for (var i = 0; i < part1.Length; i++)
                            if (part2.Contains(part1[i]))
                            {
                                part2 = part2.SubArray(Array.IndexOf(part2, part1[i]));
                                part1 = part1.SubArray(0, i);
                            }

                        path = part1.Concat(part2).ToArray();
                    }
                    else
                        path = new[] {clearFavorite};
                }
                
                // segment completed stuff
                int[] pathCostData = null;
                if (path.Length > 0)
                    pathCostData = MovePath(path, "Moving towards fog at " + foggyNode);

                // update cost of foggy node
                AddCostToNode(foggyNode, PartialCost + foggyNode.Cost);

                if (pathCostData != null)
                    // <----------------- segment ends here ----------------->
                    SegmentCompleted(g => DrawPath(g, path, pathCostData));

                Console.WriteLine("Preparation for fog exploration took " + watch.ElapsedMilliseconds + "ms");
                watch.Restart();

                // <----------------- new segment starts here ----------------->
                // explore fog
                var mainSteps = _steps.Count;
                var result = AbstractFogExploreAlgorithm.ExploreFog(fogExploreName, foggyNode, _allFoggyNodes.ToArray(), GetCostFromNode, AddCostToNode, MoveFog, n => GetHeuristic(n, targetNode));

                Console.WriteLine("Fog exploration required " + (_steps.Count - mainSteps) + " steps and took " + watch.ElapsedMilliseconds + "ms");
                watch.Restart();

                // <----------------- segment ends here ----------------->
                SegmentCompleted(DrawFoggyPath(result.Item2, result.Item3));

                // check whether a new exit was discovered
                if (result.Item1 == null)
                {
                    // no other exit found, try other foggy node
                    Console.WriteLine("Node " + foggyNode + " didn't yield anything useful, trying next node...");

                    //// item2 contains the path that was taken, the last node should be the clear node where the fog was left
                    //if (result.Item2.Length > 0)
                    //{
                    //    if (result.Item2.Last().Edges.Count(e => e != null && e.GetOtherNode(result.Item2.Last()).KnownToPlayer) != 1)
                    //        throw new Exception("We didn't return on a clear tile :/");

                    //    // set the clear node where we returned
                    //    clearNodeWhereFogWasLeft =
                    //        result.Item2.Last()
                    //            .Edges.First(e => e != null && e.GetOtherNode(result.Item2.Last()).KnownToPlayer)
                    //            .GetOtherNode(result.Item2.Last());
                    //}
                    //else
                    //{
                        
                    // we returned on the same node where we entered the fog (because no way out was found)
                    clearNodeWhereFogWasLeft = clearFavorite;
                    FoggyNodes.Remove(foggyNode);
                    
                    //}

                    if (!clearNodeWhereFogWasLeft.KnownToPlayer)
                        throw new ArgumentException("Must be a clear node!");

                    continue;
                }

                // exit was found!
                if (result.Item1.EntityOnNode == Entity.Target)
                {
                    // exit was the target, WE ARE DONE!
                    Console.WriteLine("Found path to target through fog!");

                    return;
                }
                
                // exit was another fog-less area, start pathfinding

                // move to the clear tile
                MoveNode(result.Item1);

                // new pathfinding has not yet discovered foggy nodes
                FoggyNodes.Clear();

                Console.WriteLine("Found another part of the known map, restarting pathfinding.");
                FindPath(result.Item1, targetNode, fogMethod, fogExploreName);

                return;
            }

            // we get here if there is no foggy node we could explore anymore, we got stuck
            CreateStep(g => g.DrawString("No path found! :(", new Font("Microsoft Sans Serif", 64, FontStyle.Bold), Brushes.Lime, 0, 0), "No path found! :(");
        }

        /// <summary>
        /// Move player onto a node.
        /// </summary>
        /// <param name="node">Node where the player should move to</param>
        private void MoveNode(Node node)
        {
            if (_lastNode.Edges.Count(e => e != null && e.GetOtherNode(_lastNode) == node) != 1)
                throw new Exception("Teleporting is not allowed!");

            //Console.WriteLine("Moving to node " + node);

            // update partial cost since player has travelled
            PartialCost += node.Cost;

            // update last node
            _lastNode = node;
        }

        /// <summary>
        /// Move through the fog to one new node and draw the complete foggy path.
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
                _exploredFoggyCells++;

            // move onto the node
            MoveNode(node);

            // create new algorithm step
            CreateStep(DrawFoggyPath(path, backtrackedNodes), comment);
        }

        /// <summary>
        /// Moves the player along a clear path.
        /// </summary>
        /// <param name="nodes">Nodes that make up the path</param>
        /// <param name="comment">Comment for the steps</param>
        /// <returns>Array containing the costs for the different steps of the path</returns>
        private int[] MovePath(Node[] nodes, string comment)
        {
            if (nodes.Any(n => !n.KnownToPlayer))
                throw new Exception("Path must consist of clear tiles!");

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
        /// Removes all foggy nodes where all neighbors are known or otherwise contained in the foggy node list.
        /// </summary>
        /// <param name="nodes">Collection of REMOVABLE nodes</param>
        /// <returns>Nodes without the ones that don't need to be investigated</returns>
        private IEnumerable<Node> RemoveLonelyFoggyNodes(IList<Node> nodes)
        {
            // prepare data
            bool loop;
            var removedString = "";
            var removedNodes = new List<Node>();
            var oldFoggyNodes = _allFoggyNodes.Except(FoggyNodes);

            // loop as long as at least one foggy node has been removed
            do
            {
                loop = false;

                for (var i = nodes.Count - 1; i >= 0; i--)
                {
                    if (!nodes[i].Edges.All(
                        e =>
                            e == null || nodes.Contains(e.GetOtherNode(nodes[i])) ||
                            removedNodes.Contains(e.GetOtherNode(nodes[i])) ||
                            oldFoggyNodes.Contains(e.GetOtherNode(nodes[i])) ||
                            (e.GetOtherNode(nodes[i]).Cost == int.MaxValue &&
                             e.GetOtherNode(nodes[i])
                                 .Edges.Any(
                                     ee =>
                                         ee != null &&
                                         GetCostFromNode(ee.GetOtherNode(e.GetOtherNode(nodes[i]))) != int.MaxValue)) ||
                            (e.GetOtherNode(nodes[i]).KnownToPlayer &&
                             GetCostFromNode(e.GetOtherNode(nodes[i])) != int.MaxValue))) continue;

                    // remove a node
                    loop = true;
                    removedString += ", " + nodes[i];
                    removedNodes.Add(nodes[i]);
                    nodes.RemoveAt(i);
                }
            } while (loop);

            if (removedString != "")
                Console.WriteLine("Removed nodes " + removedString.Substring(2) + " from foggy node list.");

            return removedNodes;
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

            // add new step
            _steps.Add(new AlgorithmStep(g =>
            {
                // draw current (partial) segment
                drawStep(g);

                // draw previous segments
                foreach (var a in previousActions)
                    a(g);
            }, ExploredClearCells + _exploredFoggyCells, Graph.PassibleNodeCount, PartialCost, comment));
        }

        /// <summary>
        /// Draws all possible nodes that could be explored in the fog and highlight the node that has been chosen.
        /// </summary>
        /// <param name="g">Graphics reference</param>
        /// <param name="foggyNode">Node that will be explored</param>
        /// <param name="candidates">Other nodes that could have been chosen.</param>
        private static void DrawFoggyAlternatives(Graphics g, Node foggyNode, IEnumerable<Node> candidates)
        {
            var candidatePen = new Pen(Brushes.Teal, 2);
            var foggyNodePen = new Pen(Brushes.Salmon, 3);

            // draw candidates (circle)
            foreach (var r in candidates.Select(c => MainForm.MapPointToCanvasRectangle(c.Location)))
                g.DrawEllipse(candidatePen, r.Left, r.Top, r.Width - 1, r.Height - 1);

            // draw foggy node (X)
            var rr = MainForm.MapPointToCanvasRectangle(foggyNode.Location);
            g.DrawLine(foggyNodePen, rr.Left, rr.Top, rr.Right - 1, rr.Bottom - 1);
            g.DrawLine(foggyNodePen, rr.Left, rr.Bottom - 1, rr.Right - 1, rr.Top);
        }

        /// <summary>
        /// Draws a path through the fog and distinguishes between nodes of the current path and those that have been abandonned.
        /// </summary>
        /// <param name="path">Current path</param>
        /// <param name="backtrackedNodes">Nodes where the player had to backtrack</param>
        /// <returns>Action that draws the path</returns>
        private Action<Graphics> DrawFoggyPath(IList<Node> path, IList<Node> backtrackedNodes)
        {
            // calculate current path costs and store them locally
            var pathCost = new int[path.Count];
            for (var i = 0; i < path.Count; i++)
                pathCost[i] = GetCostFromNode(path[i]);
            var backtrackedCost = new int[backtrackedNodes.Count];
            for (var i = 0; i < backtrackedNodes.Count; i++)
                backtrackedCost[i] = GetCostFromNode(backtrackedNodes[i]);

            // draw both paths (though the backtracked nodes don't technically form a path... shhhh!)
            return g =>
            {
                DrawPath(g, path, pathCost);
                DrawPath(g, backtrackedNodes, backtrackedCost, true);
            };
        }

        /// <summary>
        /// Draws a path (collection of nodes) with associated costs for the different nodes.
        /// </summary>
        /// <param name="g">Reference to the graphics where the stuff should be drawn to</param>
        /// <param name="path">Nodes to be drawn</param>
        /// <param name="cost">Cost of the nodes to be drawn</param>
        /// <param name="backtrackingPath">True if the path is backtracking</param>
        private static void DrawPath(Graphics g, IList<Node> path, IList<int> cost, bool backtrackingPath = false)
        {
            if (path.Count != cost.Count)
                throw new ArgumentException("Don't have the same number of nodes and node costs");

            // convert data into more useful form for drawing
            var data = new Tuple<Node, int, bool>[path.Count];
            for (var i = 0; i < path.Count; i++)
                data[i] = new Tuple<Node, int, bool>(path[i], cost[i], backtrackingPath);

            // call actual drawing method
            DrawNodes(g, data);
        }

        /// <summary>
        /// Draws a collection of nodes where three pieces of information are required: the node, the cost of the node and whether the node is backtracking.
        /// </summary>
        /// <param name="g">Reference to the graphics where the stuff should be drawn to</param>
        /// <param name="data">Nodes to be drawn with the cost of the node and a flag showing whether the node is backtracked or not</param>
        private static void DrawNodes(Graphics g, IList<Tuple<Node, int, bool>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                // get rectangles
                var nodeRect = MainForm.MapPointToCanvasRectangle(data[i].Item1.Location);
                var iconRect = new Rectangle(nodeRect.Location.X + nodeRect.Width / 2, nodeRect.Location.Y + nodeRect.Height / 2, nodeRect.Width / 2, nodeRect.Height / 2);
                
                // draw icon
                Utils.DrawTransparentImage(g, Resources.runner.ToBitmap(), iconRect, Utils.GetPathOpacity(i, data.Count), data[i].Item3);

                // draw cost
                g.DrawString(data[i].Item2.ToString(CultureInfo.InvariantCulture),
                    new Font("Microsoft Sans Serif", 10, data[i].Item3 ? FontStyle.Regular : FontStyle.Bold), data[i].Item3 ? Brushes.Orange : Brushes.Red,
                    nodeRect.Location);
            }
        }

        /// <summary>
        /// Called when a segment (clear/foggy path exploration) has been completed. Adds an action to draw the current segment to the list of previous segments. This makes sure that the "result" of a segment is always drawn even if we're working on the next segment.
        /// </summary>
        /// <param name="segmentDrawAction">Action that draws the segment</param>
        private void SegmentCompleted(Action<Graphics> segmentDrawAction)
        {
            _segmentDrawActions.Insert(0, segmentDrawAction);
        }

        /// <summary>
        /// Reset the algorithm data completely to do a fresh run.
        /// </summary>
        public virtual void ResetAlgorithm()
        {
            // reset own data
            _steps.Clear();
            _lastNode = null;
            _segmentDrawActions.Clear();
            ExploredClearCells = 0;
            _exploredFoggyCells = 0;
            PartialCost = 0;
            FoggyNodes.Clear();
            _allFoggyNodes.Clear();

            // reset fog data
            AbstractFogExploreAlgorithm.Reset();
        }

        /// <summary>
        /// Registers a new graph.
        /// </summary>
        /// <param name="graph">New graph to use</param>
        public static void RegisterGraph(Graph graph)
        {
            Graph = graph;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets the action to draw a step of the algorithm from the player to the (current) target.
        /// </summary>
        /// <param name="start">Starting node</param>
        /// <param name="end">End node</param>
        /// <param name="withCost">True if the cost on the nodes should be displayed as well</param>
        /// <returns>Action that draws the step</returns>
        protected abstract Action<Graphics> GetAlgorithmStep(Node start, Node end, bool withCost = true);

        /// <summary>
        /// Gets all alternative paths to the target.
        /// </summary>
        /// <param name="player">Starting node</param>
        /// <param name="target">Taget node</param>
        /// <returns>Action that draws all alternatives</returns>
        protected abstract Action<Graphics> GetAlternativesStep(Node player, Node target);

        /// <summary>
        /// Actual algorithm implementation: Find alternative paths.
        /// </summary>
        /// <param name="playerNode">Starting node</param>
        /// <param name="targetNode">End node</param>
        protected abstract Action<Graphics> FindAlternativePaths(Node playerNode, Node targetNode);

        /// <summary>
        /// Adds a cost to a node. Depends on algorithm implementation.
        /// </summary>
        /// <param name="node">Node to be updated</param>
        /// <param name="cost">New cost of the node</param>
        protected abstract void AddCostToNode(Node node, int cost);

        /// <summary>
        /// Retrieves the cost of the path to a node. Depends on algorithm implementation.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Cost of the path to the node</returns>
        protected abstract int GetCostFromNode(Node node);

        /// <summary>
        /// Gets a path from the player to the target (in a known and explored section of the map).
        /// </summary>
        /// <param name="start">Starting node of the path</param>
        /// <param name="end">End node of the path</param>
        /// <returns>Array of nodes from start to end</returns>
        protected abstract Node[] GetPath(Node start, Node end);

        /// <summary>
        /// Actual algorithm implementation: Find shortest path.
        /// </summary>
        /// <param name="playerNode">Starting node</param>
        /// <param name="targetNode">End node</param>
        protected abstract bool FindShortestPath(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Prepare data for pathfinding.
        /// </summary>
        /// <param name="playerNode">Starting node</param>
        /// <param name="targetNode">End node</param>
        protected abstract void PrepareData(Node playerNode, Node targetNode);

        /// <summary>
        /// Returns heuristic distance between nodes.
        /// </summary>
        /// <param name="node1">One node</param>
        /// <param name="node2">Other node</param>
        /// <returns>Distance</returns>
        protected abstract int GetHeuristic(Node node1, Node node2);

        #endregion
    }
}
