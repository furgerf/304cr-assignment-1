using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Algorithm
{
    public class AStarAbstractAlgorithm : AbstractAlgorithm
    {
        #region Fields

        private readonly List<Node> _openNodes = new List<Node>();

        private readonly List<Node> _closedNodes = new List<Node>();

        private readonly List<Tuple<Node, int>> _foggyNodes = new List<Tuple<Node, int>>(); 

        private readonly Dictionary<Node, Tuple<int, int>> _nodeDataMap = new Dictionary<Node, Tuple<int, int>>(); 

        #endregion

        #region Constructor

        public AStarAbstractAlgorithm(Graph graph)
            : base(graph)
        {
        }

        #endregion

        #region Main Methods

        override public void FindPath(Node from, Node to)
        {
            PrepareData(from, to);

            Node currentNode = null;

            // loop while we have options an at least one of the options and we have not yet found a way
            while (_openNodes.Count > 0 && _nodeDataMap[to].Item1 == int.MaxValue)
            {
                // apply the algorithm to do the actual pathfinding
                currentNode = _openNodes[0];
                //var f = _nodeDataMap[currentNode].Item1 + _nodeDataMap[currentNode].Item2;
                ProcessNode(currentNode);

                Steps.Add(GetAlgorithmStep(from, currentNode));
            }

            if (currentNode != null && currentNode.Edges.Any(e => e.GetOtherNode(currentNode) == to))
            {
                // path found! don't bother checking fog

                // check for other paths with the same cost
                while (_openNodes.Count > 0)
                {
                    // remove all costlier nodes from the list
                    var stillOpen =
                        _openNodes.Where(n => n != to && _nodeDataMap[n].Item1 + _nodeDataMap[n].Item2 == _nodeDataMap[to].Item1).ToArray();
                    _openNodes.Clear();
                    _openNodes.AddRange(stillOpen);

                    // check all remaining nodes
                    for (var i = 0; i < _openNodes.Count; i++)
                    {
                        if (_openNodes[i] == to)
                            // no need to check target node
                            _openNodes.RemoveAt(i);
                        else if (_nodeDataMap[_openNodes[i]].Item1 + _nodeDataMap[_openNodes[i]].Item2 == _nodeDataMap[to].Item1)
                        {
                            // check node
                            currentNode = _openNodes[i];

                            ProcessNode(currentNode);

                            Steps.Add(GetAlgorithmStep(from, currentNode));
                        }
                    }
                }

                // add step with all possdible paths
                Steps.Add(GetAlternativesStep(from, to));
            }
            else
            {
                // no path found. check fog
                // find foggy tile that is closest to target
                _foggyNodes.Sort((a, b) =>
                {
                    var aCost = a.Item2 + _nodeDataMap[a.Item1].Item2;
                    var bCost = b.Item2 + _nodeDataMap[b.Item1].Item2;
                    if (aCost == bCost) return 0;
                    return aCost < bCost ? -1 : 1;
                });
            }
        }

        protected override void ResetAlgorithm()
        {
            _openNodes.Clear();
            _closedNodes.Clear();
            _nodeDataMap.Clear();
            _foggyNodes.Clear();
        }

        private AlgorithmStep GetAlgorithmStep(Node from, Node currentNode)
        {
            // prepare data for printing cost
            var costData = _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item1 != int.MaxValue).Select(n => new Tuple<string, Point, Brush, Font>("g=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : _nodeDataMap[n].Item1.ToString()) + "\nh=" + _nodeDataMap[n].Item2.ToString() + "\nf=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : (_nodeDataMap[n].Item1 + _nodeDataMap[n].Item2).ToString()), n.Location, n == currentNode ? Brushes.DarkRed : Brushes.Turquoise, new Font("Microsoft Sans Serif", 12, _openNodes.Contains(n) ? FontStyle.Bold : FontStyle.Regular))).ToList();

            // prepare data for printing path
            var pathData = new List<Tuple<Point, Point>>();
            var node = currentNode;
            while (node != from)
            {
                // find neighbor where you would "come from"
                var edges = node.Edges.Where(e => e != null && _nodeDataMap[e.GetOtherNode(node)].Item1 != int.MaxValue && !_openNodes.Contains(e.GetOtherNode(node))).ToList();
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
                // add to list
                var p1 = MainForm.MapPointToCanvasRectangle(node.Location);
                var p2 = MainForm.MapPointToCanvasRectangle(minNode.Location);
                pathData.Add(new Tuple<Point, Point>(new Point(p1.X + p1.Width/2, p1.Y + p1.Height/2),
                    new Point(p2.X + p2.Width/2, p2.Y + p2.Height/2)));

                node = minNode;
            }

            // create new step
            var newStep = new AlgorithmStep(g =>
            {
                // draw cost of nodes
                foreach (var d in costData)
                    g.DrawString(d.Item1.ToString(), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));

                // draw path
                foreach (var d in pathData)
                    g.DrawLine(new Pen(Color.Yellow, 3), d.Item1, d.Item2);
            }, _closedNodes.Count, Graph.PassibleNodeCount);

            return newStep;
        }

        private AlgorithmStep GetAlternativesStep(Node from, Node to)
        {
            // prepare data for drawing
            var costData = _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item1 != int.MaxValue).Select(n => new Tuple<string, Point, Brush, Font>("g=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : _nodeDataMap[n].Item1.ToString()) + "\nh=" + _nodeDataMap[n].Item2.ToString() + "\nf=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : (_nodeDataMap[n].Item1 + _nodeDataMap[n].Item2).ToString()), n.Location, n == to ? Brushes.DarkRed : Brushes.Turquoise, new Font("Microsoft Sans Serif", 12, _openNodes.Contains(n) ? FontStyle.Bold : FontStyle.Regular))).ToList();

            // prepare data for path
            var pathData = new List<Tuple<Point, Point, Pen>>();
            var openPaths = new List<List<Node>> {new List<Node> {to}};
            var closedPaths = new List<List<Node>>();
            do
            {
                for (var i = 0; i < openPaths.Count; i++)
                {
                    var min = (from e in openPaths[i].Last().Edges where e != null select e.GetOtherNode(openPaths[i].Last()) into otherNode where _nodeDataMap[otherNode].Item1 != int.MaxValue select _nodeDataMap[otherNode].Item1).Concat(new[] {int.MaxValue}).Min();

                    // find all neighbor (edges) with min cost
                    var cheapestEdges =
                        openPaths[i].Last()
                            .Edges.Where(
                                e =>
                                    e != null && _nodeDataMap[e.GetOtherNode(openPaths[i].Last())].Item1 != int.MaxValue &&
                                    _nodeDataMap[e.GetOtherNode(openPaths[i].Last())].Item1 == min)
                            .ToArray();

                    if (cheapestEdges.Length == 0)
                        throw new Exception("Shouldn't hit a dead end!!");

                    // if only one exists, continue the current path
                    if (cheapestEdges.Length == 1)
                        openPaths[i].Add(cheapestEdges[0].GetOtherNode(openPaths[i].Last()));
                    else
                    {
                        // copy current path to end of openpaths and add the different nodes
                        for (var j = 1; j < cheapestEdges.Length; j++)
                        {
                            var newList = new Node[openPaths[i].Count];
                            openPaths[i].CopyTo(newList);

                            if (cheapestEdges[j].GetOtherNode(openPaths[i].Last()) == from)
                            {
                                closedPaths.Add(newList.ToList());
                                closedPaths.Last().Add(cheapestEdges[j].GetOtherNode(openPaths[i].Last()));
                            }
                            else
                            {
                                openPaths.Add(newList.ToList());
                                openPaths.Last().Add(cheapestEdges[j].GetOtherNode(openPaths[i].Last()));
                            }
                        }
                        openPaths[i].Add(cheapestEdges[0].GetOtherNode(openPaths[i].Last()));
                    }

                    // move completed (main) path to other list if it reaches the starting point
                    if (openPaths[i].Last() != from) continue;
                    closedPaths.Add(openPaths[i]);
                    openPaths.RemoveAt(i);
                    i--;
                }
            } while (openPaths.Count > 0);

            // add path to list data
            var minPath = closedPaths.Select(p => p.Count).Concat(new[] {int.MaxValue}).Min();
            var maxPath = closedPaths.Select(p => p.Count).Concat(new[] {int.MinValue}).Max();
            foreach (var path in closedPaths)
                for (var i = 1; i < path.Count; i++)
                {
                    var p1 = MainForm.MapPointToCanvasRectangle(path[i - 1].Location);
                    var p2 = MainForm.MapPointToCanvasRectangle(path[i].Location);
                    var offset = 2 + 4*closedPaths.IndexOf(path);
                    var perc = maxPath == minPath ? 1 : (double) (path.Count - minPath)/(maxPath - minPath);
                    var color = Color.FromArgb(255, (int) (perc*255), (int) ((1 - perc)*255), 0);
                    pathData.Add(
                        new Tuple<Point, Point, Pen>(
                            new Point(p1.X + offset, p1.Y + offset),
                            new Point(p2.X + offset, p2.Y + offset), new Pen(color, 2)));
                }

            // create new step
            var newStep = new AlgorithmStep(g =>
            {
                // draw cost of nodes
                foreach (var d in costData)
                    g.DrawString(d.Item1.ToString(), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));

                // draw paths
                foreach (var d in pathData)
                    g.DrawLine(d.Item3, d.Item1, d.Item2);
            }, _closedNodes.Count, Graph.PassibleNodeCount);

            return newStep;
        }

        private void ProcessNode(Node node)
        {
            // move node to closed list
            _openNodes.Remove(node);
            _closedNodes.Add(node);

            // add foggy neighbors
            foreach (var e in node.Edges.Where(e => e != null && e.GetOtherNode(node).Cost != int.MaxValue && !_foggyNodes.Contains(new Tuple<Node, int>(e.GetOtherNode(node), _nodeDataMap[node].Item1)) && !e.GetOtherNode(node).KnownToPlayer))
                _foggyNodes.Add(new Tuple<Node, int>(e.GetOtherNode(node), _nodeDataMap[node].Item1));

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
                        // remove from open list (since f changed)
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

        private void PrepareData(Node startNode, Node targetNode)
        {
            // add all nodes to the unvisited-node-list and tag nodes as unvisited with max cost
            foreach (var nn in Graph.Nodes.Where(n => n != null))
            {
                foreach (var n in nn.Where(n => n != null))
                    _nodeDataMap.Add(n, new Tuple<int, int>(n == startNode ? 0 : int.MaxValue, GetHeuristic(n, targetNode)));
            }

            _openNodes.Add(startNode);
        }

        private int GetHeuristic(Node node, Node target)
        {
            // Manhatten Distance
            return Math.Abs(target.Location.X - node.Location.X) + Math.Abs(target.Location.Y - node.Location.Y);
        }

        #endregion
    }
}
