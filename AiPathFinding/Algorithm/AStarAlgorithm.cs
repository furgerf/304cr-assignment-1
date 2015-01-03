using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Implementation of the A*-Algorithm.
    /// </summary>
    public class AStarAlgorithm : AbstractPathFindAlgorithm
    {
        #region Fields

        /// <summary>
        /// List of currently open nodes.
        /// </summary>
        private readonly List<Node> _openNodes = new List<Node>();

        /// <summary>
        /// List of already closed nodes.
        /// </summary>
        private readonly List<Node> _closedNodes = new List<Node>();

        /// <summary>
        /// Assigns a value for "g" and "h" target each node.
        /// </summary>
        private readonly Dictionary<Node, Tuple<int, int>> _nodeDataMap = new Dictionary<Node, Tuple<int, int>>(); 

        #endregion

        #region Constructor

        protected AStarAlgorithm(PathFindName name)
            : base(name)
        {
        }

        public AStarAlgorithm() : base(PathFindName.AStar)
        {
        }

        #endregion

        #region Parent Methods

        protected override void PrepareData(Node playerNode, Node targetNode)
        {
            // add all nodes target the data map
            // BUT ONLY IF THIS IS THE FIRST INSTANCE OF THE ALGORITHM
            if (playerNode.EntityOnNode == Entity.Player)
                foreach (var n in Graph.Nodes.Where(n => n != null).SelectMany(nn => nn.Where(n => n != null)))
                    _nodeDataMap.Add(n, new Tuple<int, int>(n == playerNode ? 0 : int.MaxValue, GetHeuristic(n, targetNode)));

            // we haven't updated any nodes yet...
            UpdatedNodes.Clear();
            _closedNodes.Clear();

            // start the pathfinding playerNode the start node
            _openNodes.Add(playerNode);
        }

        protected override void AddCostToNode(Node node, int cost)
        {
            Console.WriteLine("Setting cost of " + node + " to " + cost);
            
            // set tuple item in nodedatamap
            _nodeDataMap[node] = new Tuple<int, int>(cost, _nodeDataMap[node].Item2);
        }

        protected override int GetCostFromNode(Node node)
        {
            // return tuple item from nodedatamap
            return _nodeDataMap[node].Item1;
        }

        protected override Action<Graphics> FindAlternativePaths(Node playerNode, Node targetNode)
        {
            // check all remaining nodes
            while (_openNodes.Count > 0)
            {
                if (_openNodes[0] == targetNode ||
                    _nodeDataMap[_openNodes[0]].Item1 + _nodeDataMap[_openNodes[0]].Item2 >
                    _nodeDataMap[targetNode].Item1)
                {
                    _openNodes.RemoveAt(0);
                    continue;
                }

                // check node
                var currentNode = _openNodes[0];
                ProcessNode(currentNode);
                ExploredClearCells++;
                CreateStep(GetAlgorithmStep(playerNode, currentNode), "A*: Looking for alternative paths");
            }

            // add step with all possdible paths
            return GetAlternativesStep(playerNode, targetNode);
        }

        protected override bool FindShortestPath(Node playerNode, Node to)
        {
            // if we know that the target is unreachable, return
            if (to.KnownToPlayer && to.Cost == int.MaxValue)
                return false;

            Node currentNode = null;
            // loop while we have options an at least one of the options and we have not yet found a way
            while (_openNodes.Count > 0 && _nodeDataMap[to].Item1 == int.MaxValue)
            {
                // look for a path playerNode the first open node
                currentNode = _openNodes[0];
                ProcessNode(currentNode);
                ExploredClearCells++;
                CreateStep(GetAlgorithmStep(playerNode, currentNode), "A*: Exploring " + currentNode);
            }

            // pathfinding has terminated, tell about result
            return currentNode != null && currentNode.Edges.Any(e => e != null && e.GetOtherNode(currentNode) == to);
        }

        public override void ResetAlgorithm()
        {
            base.ResetAlgorithm();

            _openNodes.Clear();
            _closedNodes.Clear();
            _nodeDataMap.Clear();
        }

        protected override Node[] GetPath(Node player, Node target)
        {
            if (_nodeDataMap[player].Item1 == int.MaxValue || _nodeDataMap[target].Item1 == int.MaxValue)
                throw new ArgumentException("I don't know how to get the cheapest path from player to target");

            // prepare data for printing path
            var pathData = new List<Node>();
            var node = target;
            while (node != player)
            {
                // find neighbor where you would "come from"
                var edges = node.Edges.Where(e => e != null && _nodeDataMap[e.GetOtherNode(node)].Item1 != int.MaxValue && e.GetOtherNode(node).KnownToPlayer && !_openNodes.Contains(e.GetOtherNode(node))).ToList();
                // find cheapest edge
                edges.Sort(
                    (a, b) =>
                    {
                        var ag = _nodeDataMap[a.GetOtherNode(node)].Item1;
                        var ah = _nodeDataMap[a.GetOtherNode(node)].Item2;
                        var af = ag + ah;
                        var bg = _nodeDataMap[b.GetOtherNode(node)].Item1;
                        var bh = _nodeDataMap[b.GetOtherNode(node)].Item2;
                        var bf = bg + bh;

                        if (af < bf)
                            return -1;
                        if (bf < af)
                            return 1;
                        if (ag == bg)
                            return 0;
                        return ag < bg ? -1 : 1;
                    });
                var minNode = edges.First().GetOtherNode(node);

                pathData.Add(node);

                node = minNode;
            }

            pathData.Add(node);

            pathData.Reverse();

            return pathData.ToArray();
        }

        protected override Action<Graphics> GetAlgorithmStep(Node player, Node target, bool withCost = true)
        {
            // prepare data for printing cost
            var costData = new List<Tuple<string, Point, Brush, Font>>();
            
            if (withCost)
                costData = _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item1 != int.MaxValue && UpdatedNodes.Contains(k) || _openNodes.Contains(k)).Select(n => new Tuple<string, Point, Brush, Font>("g=" + _nodeDataMap[n].Item1.ToString(CultureInfo.InvariantCulture) + "\nh=" + _nodeDataMap[n].Item2.ToString(CultureInfo.InvariantCulture) + "\nf=" + (_nodeDataMap[n].Item1 + _nodeDataMap[n].Item2).ToString(CultureInfo.InvariantCulture), n.Location, n == target ? Brushes.DarkRed : Brushes.Turquoise, new Font("Microsoft Sans Serif", 10, _openNodes.Contains(n) ? FontStyle.Bold : FontStyle.Regular))).ToList();

            // prepare data for printing path
            var pathData = new List<Tuple<Point, Point>>();
            var path = GetPath(player, target);
            for (var i = 0; i < path.Length - 1; i++)
            {
                var p1 = MainForm.MapPointToCanvasRectangle(path[i].Location);
                var p2 = MainForm.MapPointToCanvasRectangle(path[i+1].Location);
                pathData.Add(new Tuple<Point, Point>(new Point(p1.X + p1.Width/2, p1.Y + p1.Height/2),
                    new Point(p2.X + p2.Width/2, p2.Y + p2.Height/2)));
            }

            // return code to draw the step
            return g =>
            {
                // draw cost of nodes
                foreach (var d in costData)
                    g.DrawString(d.Item1.ToString(CultureInfo.InvariantCulture), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));

                // draw path
                foreach (var d in pathData)
                    g.DrawLine(new Pen(Color.Yellow, 3), d.Item1, d.Item2);
            };
        }
        protected override Action<Graphics> GetAlternativesStep(Node player, Node target)
        {
            // prepare data for drawing
            var costData = _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item1 != int.MaxValue).Select(n => new Tuple<string, Point, Brush, Font>("g=" + _nodeDataMap[n].Item1.ToString(CultureInfo.InvariantCulture) + "\nh=" + _nodeDataMap[n].Item2.ToString(CultureInfo.InvariantCulture) + "\nf=" + (_nodeDataMap[n].Item1 + _nodeDataMap[n].Item2).ToString(CultureInfo.InvariantCulture), n.Location, n == target ? Brushes.DarkRed : Brushes.Turquoise, new Font("Microsoft Sans Serif", 10, _openNodes.Contains(n) ? FontStyle.Bold : FontStyle.Regular))).ToList();

            // prepare data for path
            var pathData = new List<Tuple<Point, Point, Pen>>();
            var openPaths = new List<List<Node>> {new List<Node> {target}};
            var closedPaths = new List<List<Node>>();
            do
            {
                var min = int.MaxValue;
                foreach (var e in openPaths[0].Last().Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(openPaths[0].Last())) < min)
                        min = GetCostFromNode(e.GetOtherNode(openPaths[0].Last()));

                var foo = new List<Edge>();
                foreach (var e in openPaths[0].Last().Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(openPaths[0].Last())) == min)
                        foo.Add(e);

                foreach (var e in foo)
                    if (e.GetOtherNode(openPaths[0].Last()) == player)
                    {
                        foo.Clear();
                        foreach (var ee in openPaths[0].Last().Edges)
                            if (ee != null && ee.GetOtherNode(openPaths[0].Last()) == player)
                                foo.Add(ee);
                        break;
                    }

                var cheapestEdges = foo.ToArray();

                if (cheapestEdges.Length == 0)
                    throw new Exception("Shouldn't hit a dead end!!");

                // if only one exists, continue the current path
                if (cheapestEdges.Length == 1)
                    openPaths[0].Add(cheapestEdges[0].GetOtherNode(openPaths[0].Last()));
                else
                {
                    // copy current path target end of openpaths and add the different nodes
                    for (var j = 1; j < cheapestEdges.Length; j++)
                    {
                        var newList = new Node[openPaths[0].Count];
                        openPaths[0].CopyTo(newList);

                        if (cheapestEdges[j].GetOtherNode(openPaths[0].Last()) == player)
                        {
                            closedPaths.Add(newList.ToList());
                            closedPaths.Last().Add(cheapestEdges[j].GetOtherNode(openPaths[0].Last()));
                        }
                        else
                        {
                            openPaths.Add(newList.ToList());
                            openPaths.Last().Add(cheapestEdges[j].GetOtherNode(openPaths[0].Last()));
                        }
                    }
                    openPaths[0].Add(cheapestEdges[0].GetOtherNode(openPaths[0].Last()));
                }

                // move completed (main) path target other list if it reaches the starting point
                if (openPaths[0].Last() != player) continue;
                closedPaths.Add(openPaths[0]);
                openPaths.RemoveAt(0);
            } while (openPaths.Count > 0);

            // add path target list data
            var minPath = closedPaths.Select(p => p.Count).Concat(new[] {int.MaxValue}).Min();
            var maxPath = closedPaths.Select(p => p.Count).Concat(new[] {int.MinValue}).Max();
            foreach (var path in closedPaths)
                for (var i = 1; i < path.Count; i++)
                {
                    var p1 = MainForm.MapPointToCanvasRectangle(path[i - 1].Location);
                    var p2 = MainForm.MapPointToCanvasRectangle(path[i].Location);
                    var offset = 2 + 4*closedPaths.IndexOf(path);
                    var perc = maxPath == minPath ? 0 : (double) (path.Count - minPath)/(maxPath - minPath);
                    var color = Color.FromArgb(255, (int) (perc*255), (int) ((1 - perc)*255), 0);
                    pathData.Add(
                        new Tuple<Point, Point, Pen>(
                            new Point(p1.X + offset, p1.Y + offset),
                            new Point(p2.X + offset, p2.Y + offset), new Pen(color, 2)));
                }

            Console.WriteLine("* Found " + closedPaths.Count() + " distinct paths with cost of " + _nodeDataMap[target].Item1 + ", ranging from " + minPath + " to " + maxPath + " cells long!");

            return g =>
            {
                // draw cost of nodes
                foreach (var d in costData)
                    g.DrawString(d.Item1.ToString(CultureInfo.InvariantCulture), d.Item4, d.Item3,
                        MainForm.MapPointToCanvasRectangle(d.Item2));

                // draw paths
                foreach (var d in pathData)
                    g.DrawLine(d.Item3, d.Item1, d.Item2);
            };
        }

        #endregion

        #region Own Methods

        /// <summary>
        /// Processes one particular node and does algorithm stuff on it.
        /// </summary>
        /// <param name="node">Node that is processed</param>
        private void ProcessNode(Node node)
        {
            // move node target closed list
            _openNodes.Remove(node);
            UpdatedNodes.Add(node);
            _closedNodes.Add(node);

            // add foggy neighbors
            foreach (var e in node.Edges.Where(e => e != null && e.GetOtherNode(node).Cost != int.MaxValue && !FoggyNodes.Contains(e.GetOtherNode(node)) && !e.GetOtherNode(node).KnownToPlayer))
                FoggyNodes.Add(e.GetOtherNode(node));

            // process passible, unvisited neighbors
            foreach (var e in node.Edges.Where(e => e != null && e.GetOtherNode(node).Cost != int.MaxValue && !_closedNodes.Contains(e.GetOtherNode(node)) && e.GetOtherNode(node).KnownToPlayer))
            {
                // get current node
                var n = e.GetOtherNode(node);

                // update h value of current node if smaller
                var g = _nodeDataMap[node].Item1 + n.Cost;
                var h = _nodeDataMap[n].Item2;
                var insert = !_openNodes.Contains(n);

                if (_nodeDataMap[n].Item1 == int.MaxValue || _nodeDataMap[n].Item2 == int.MaxValue || g + h < _nodeDataMap[n].Item1 + _nodeDataMap[n].Item2)
                {
                    // update value
                    _nodeDataMap[n] = new Tuple<int, int>(g, h);

                    if (_openNodes.Contains(n))
                    {
                        // remove playerNode open list (since f changed)
                        _openNodes.Remove(n);
                        insert = true;
                    }
                }

                if (insert)
                {
                    // insert node in proper place
                    if (_openNodes.Count == 0)
                        _openNodes.Add(n);
                    else
                    {
                        for (var i = 0; i < _openNodes.Count; i++)
                            if (_nodeDataMap[_openNodes[i]].Item1 + _nodeDataMap[_openNodes[i]].Item2 >= _nodeDataMap[n].Item1 + _nodeDataMap[n].Item2)
                            {
                                _openNodes.Insert(i, n);
                                insert = false;
                                break;
                            }

                        // couldn't insert, our node must be the most expensive one
                        if (insert)
                            _openNodes.Add(n);
                    }
                }
            }
        }

        protected override int GetHeuristic(Node node, Node target)
        {
            // Manhatten Distance
            return Math.Abs(target.Location.X - node.Location.X) + Math.Abs(target.Location.Y - node.Location.Y);
        }

        #endregion
    }
}
