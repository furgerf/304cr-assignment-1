using System;
using System.Collections.Generic;
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
        }

        private void ProcessNode(Node node)
        {
            foreach (var e in node.Edges)
            {
                var otherNode = e.GetOtherNode(node);
                
                //if (_nodeDataMap[otherNode].Item2 > _nodeDataMap[node].Item2 + otherNode.)
                //_nodeDataMap[otherNode] = new Tuple<bool, int>(false, );
            }

        }

        private void PrepareData(Node startNode)
        {
            // add all nodes to the unvisited-node-list and tag nodes as unvisited with max cost
            foreach (var nn in _graph.Nodes)
            {
                _unvisitedNodes.AddRange(nn);
                foreach (var n in nn)
                    _nodeDataMap.Add(n, new Tuple<bool, int>(false, n == startNode ? 0 : int.MaxValue));
            }
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
