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
            PrepareData(from, to);// TODO: find out why algorithm doesnt stop in time :/

            // loop while we have options an at least one of the options and we have not yet found a way
            while (_openNodes.Count > 0 && _nodeDataMap[to].Item1 == int.MaxValue)
            {
                // apply the algorithm to do the actual pathfinding
                var currentNode = _openNodes[0];
                //var f = _nodeDataMap[currentNode].Item1 + _nodeDataMap[currentNode].Item2;
                ProcessNode(currentNode);

                Steps.Add(GetAlgorithmStep(from, currentNode));
            }

            // add step for when the path was found
            Steps.Add(GetAlgorithmStep(from, to));

            // add step with all possdible paths
            //Steps.Add(GetAlternativesStep(from, to));
        }

        protected override void ResetAlgorithm()
        {
            throw new NotImplementedException();
        }

        private AlgorithmStep GetAlgorithmStep(Node from, Node currentNode)
        {
            // prepare data for printing cost
            var costData = _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item1 != int.MaxValue).Select(n => new Tuple<string, Point, Brush, Font>("g=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : _nodeDataMap[n].Item1.ToString()) + "\nh=" + _nodeDataMap[n].Item2.ToString() + "\nf=" + (_nodeDataMap[n].Item1 == int.MaxValue ? "\u8734" : (_nodeDataMap[n].Item1 + _nodeDataMap[n].Item2).ToString()), n.Location, n == currentNode ? Brushes.DarkRed : Brushes.Turquoise, new Font("Microsoft Sans Serif", 12, FontStyle.Regular))).ToList();

            //// prepare data for printing path
            //var pathData = new List<Tuple<Point, Point>>();
            //var node = currentNode;
            //while (node != from)
            //{
            //    // find neighbor where you would "come from"
            //    var minNode = node.Edges.First(n => n != null).GetOtherNode(node);
            //    foreach (
            //        var e in
            //            node.Edges.Where(n => n != null)
            //                .Where(e => _nodeDataMap[e.GetOtherNode(node)].Item2 < _nodeDataMap[minNode].Item2))
            //        minNode = e.GetOtherNode(node);

            //    // add to list
            //    var p1 = MainForm.MapPointToCanvasRectangle(node.Location);
            //    var p2 = MainForm.MapPointToCanvasRectangle(minNode.Location);
            //    pathData.Add(new Tuple<Point, Point>(new Point(p1.X + p1.Width / 2, p1.Y + p1.Height / 2),
            //        new Point(p2.X + p2.Width / 2, p2.Y + p2.Height / 2)));

            //    node = minNode;
            //}

            // create new step
            var newStep = new AlgorithmStep(g =>
            {
                // draw cost of nodes
                foreach (var d in costData)
                    g.DrawString(d.Item1.ToString(), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));

                //// draw path
                //foreach (var d in pathData)
                //    g.DrawLine(new Pen(Color.Yellow, 3), d.Item1, d.Item2);
            });

            return newStep;
        }

        private void ProcessNode(Node node)
        {
            // move node to closed list
            _openNodes.Remove(node);
            _closedNodes.Add(node);

            foreach (var e in node.Edges.Where(e => e != null && e.GetOtherNode(node).Cost != int.MaxValue && !_closedNodes.Contains(e.GetOtherNode(node))))
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
