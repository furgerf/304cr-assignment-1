using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Algorithm
{
    public class DijkstraAbstractAlgorithm : AbstractAlgorithm
    {
        #region Fields

        private readonly List<Node> _visitedNodes = new List<Node>();
        
        private readonly List<Node> _unvisitedNodes = new List<Node>(); 

        private readonly Dictionary<Node, Tuple<bool, int>> _nodeDataMap = new Dictionary<Node, Tuple<bool, int>>();

        #endregion

        #region Constructor

        public DijkstraAbstractAlgorithm(Graph graph)
            : base(graph)
        {
        }

        #endregion

        #region Main Methods

        override public void FindPath(Node from, Node to)
        {
            PrepareData(from);

            // loop while we have options an at least one of the options and we have not yet found a way
            while (_unvisitedNodes.Count > 0 && _nodeDataMap[to].Item2 == int.MaxValue)
            {
                // apply the algorithm to do the actual pathfinding
                var currentNode = _unvisitedNodes[0];
                ProcessNode(currentNode);

                // prepare data for drawing
                var data = new List<Tuple<string, Point, Brush, Font>>();
                foreach (var n in _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item2 != int.MaxValue))
                    data.Add(new Tuple<string, Point, Brush, Font>(_nodeDataMap[n].Item2.ToString(), n.Location,
                        n == currentNode ? Brushes.DarkRed : Brushes.Turquoise,
                        new Font("Microsoft Sans Serif", _nodeDataMap[n].Item1 ? 10 : 14,
                            _nodeDataMap[n].Item1 ? FontStyle.Regular : FontStyle.Bold)));

                // create new step
                var newStep = new AlgorithmStep(g =>
                {
                    // draw current path
                    var node = currentNode;
                    while (node != from)
                    {
                        var minNode = node.Edges.First(n => n != null).GetOtherNode(node);
                        foreach (var e in node.Edges.Where(n => n != null).Where(e => _nodeDataMap[e.GetOtherNode(node)].Item2 < _nodeDataMap[minNode].Item2))
                            minNode = e.GetOtherNode(node);

                        var p1 = MainForm.MapPointToCanvasRectangle(node.Location);
                        var p2 = MainForm.MapPointToCanvasRectangle(minNode.Location);
                        g.DrawLine(new Pen(Color.Yellow, 3), new Point(p1.X + p1.Width/2, p1.Y + p1.Height/2), new Point(p2.X + p2.Width/2, p2.Y + p2.Height/2));

                        node = minNode;
                    }

                    // draw cost of nodes
                    foreach (var d in data)
                        g.DrawString(d.Item1.ToString(), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));
                });
                Steps.Add(newStep);
            }

            // TODO: Add step for when the algorithm is complete
        }

        private void ProcessNode(Node node)
        {
            // update distance to all neighbors (if shorter)
            foreach (var e in node.Edges.Where(ee => ee != null && ee.GetOtherNode(node).Cost != int.MaxValue))
            {
                // get other node
                var otherNode = e.GetOtherNode(node);

                // insert node if it's not yet inserted (this way is cheaper to test that)
                var insert = _nodeDataMap[otherNode].Item2 == int.MaxValue;

                // if the current way is cheaper, update data
                if (_nodeDataMap[otherNode].Item2 > _nodeDataMap[node].Item2 + otherNode.Cost)
                    _nodeDataMap[otherNode] = new Tuple<bool, int>(false, _nodeDataMap[node].Item2 + otherNode.Cost);

                if (!insert) continue;

                // insert node in proper place
                if (_unvisitedNodes.Count == 0)
                    _unvisitedNodes.Add(otherNode);
                for (var i = 0; i < _unvisitedNodes.Count; i++)
                    if (_nodeDataMap[_unvisitedNodes[i]].Item2 >= _nodeDataMap[otherNode].Item2)
                    {
                        _unvisitedNodes.Insert(i, otherNode);
                        insert = false;
                        break;
                    }

                // couldn't insert, our node must be the most expensive one
                if (insert)
                    _unvisitedNodes.Add(otherNode);
            }

            // mark current node as visited and move it to the other list
            _nodeDataMap[node] = new Tuple<bool, int>(true, _nodeDataMap[node].Item2);
            _visitedNodes.Add(node);
            _unvisitedNodes.Remove(node);
        }

        private void PrepareData(Node startNode)
        {
            // add all nodes to the unvisited-node-list and tag nodes as unvisited with max cost
            foreach (var nn in Graph.Nodes.Where(n => n != null))
            {
                //_unvisitedNodes.AddRange(nn);
                foreach (var n in nn.Where(n => n != null))
                    _nodeDataMap.Add(n, new Tuple<bool, int>(false, n == startNode ? 0 : int.MaxValue));
            }

            _unvisitedNodes.Add(startNode);
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
