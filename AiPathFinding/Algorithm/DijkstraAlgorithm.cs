using System;
using System.Collections.Generic;
using System.Linq;
using AiPathFinding.Model;

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

            ProcessNode(from);

            while (_nodeDataMap[to].Item2 == int.MaxValue && _unvisitedNodes.Count > 0)
                ProcessNode(_unvisitedNodes[0]);
        }

        private void ProcessNode(Node node)
        {
            // update distance to all neighbors (if shorter)
            foreach (var e in node.Edges.Where(ee => ee != null && ee.GetOtherNode(node).Cost != int.MaxValue))
            {
                // get other node
                var otherNode = e.GetOtherNode(node);

                if (_nodeDataMap[otherNode].Item2 == int.MaxValue)
                    _unvisitedNodes.Add(otherNode);

                if (_nodeDataMap[otherNode].Item2 > _nodeDataMap[node].Item2 + otherNode.Cost)
                    _nodeDataMap[otherNode] = new Tuple<bool, int>(false, _nodeDataMap[node].Item2 + otherNode.Cost);
            }

            // mark current node as visited and move it to the other list
            _nodeDataMap[node] = new Tuple<bool, int>(true, _nodeDataMap[node].Item2);
            _visitedNodes.Add(node);
            _unvisitedNodes.Remove(node);
        }

        private void PrepareData(Node startNode)
        {
            // add all nodes to the unvisited-node-list and tag nodes as unvisited with max cost
            foreach (var nn in _graph.Nodes.Where(n => n != null))
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
