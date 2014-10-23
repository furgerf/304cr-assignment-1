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

            while (_nodeDataMap[to].Item2 == int.MaxValue && _unvisitedNodes.Count > 0)
            {
                if (_nodeDataMap[to].Item2 != int.MaxValue &&
                    _unvisitedNodes.Any(n => _nodeDataMap[n].Item2 > _nodeDataMap[to].Item2))
                    break;

                var currentNode = _unvisitedNodes[0];
                ProcessNode(currentNode);

                var data = new List<Tuple<string, Point, Brush, Font>>();

                foreach (var n in _nodeDataMap.Keys.Where(k => _nodeDataMap[k].Item2 != int.MaxValue))
                    data.Add(new Tuple<string, Point, Brush, Font>(_nodeDataMap[n].Item2.ToString(), n.Location, n == currentNode ? Brushes.Orange : Brushes.Turquoise, new Font("Microsoft Sans Serif", _nodeDataMap[n].Item1 ? 10 : 14, _nodeDataMap[n].Item1 ? FontStyle.Regular : FontStyle.Bold)));

                var newStep = new AlgorithmStep(g =>
                {
                    foreach (var d in data)
                        g.DrawString(d.Item1.ToString(), d.Item4, d.Item3, MainForm.MapPointToCanvasRectangle(d.Item2));
                });

                Steps.Add(newStep);
            }
        }

        private void ProcessNode(Node node)
        {
            // update distance to all neighbors (if shorter)
            foreach (var e in node.Edges.Where(ee => ee != null && ee.GetOtherNode(node).Cost != int.MaxValue))
            {
                // get other node
                var otherNode = e.GetOtherNode(node);

                var insert = _nodeDataMap[otherNode].Item2 == int.MaxValue;

                if (_nodeDataMap[otherNode].Item2 > _nodeDataMap[node].Item2 + otherNode.Cost)
                    _nodeDataMap[otherNode] = new Tuple<bool, int>(false, _nodeDataMap[node].Item2 + otherNode.Cost);

                if (insert)
                {
                    if (_unvisitedNodes.Count == 0)
                        _unvisitedNodes.Add(otherNode);
                    for (var i = 0; i < _unvisitedNodes.Count; i++)
                        if (_nodeDataMap[_unvisitedNodes[i]].Item2 >= _nodeDataMap[otherNode].Item2)
                        {
                            _unvisitedNodes.Insert(i, otherNode);
                            insert = false;
                            break;
                        }

                    if (insert)
                        _unvisitedNodes.Add(otherNode);
                }
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
