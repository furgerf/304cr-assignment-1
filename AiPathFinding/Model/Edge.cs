using System;
using System.Collections.Generic;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Edge
    {
        #region Fields

        public Node Node1 { get; private set; }
        public Node Node2 { get; private set; }
        public int Cost { get; private set; }

        public Direction Node1Direction { get; private set; }

        public Direction Node2Direction { get; private set; }

        // don't really need that tbh...
        private static readonly List<Edge> _edges = new List<Edge>(); 

        #endregion

        #region Constructor

        private Edge(Node node1, Direction node1Direction, Node node2, Direction node2Direction, int cost)
        {
            // validate parameters
            if (((int)node1Direction + (int)node2Direction) % 2 != 0)
                throw new ArgumentException();

            // assign fields
            Node1 = node1;
            Node2 = node2;
            Node1Direction = node1Direction;
            Node2Direction = node2Direction;
            Cost = cost;

            // assign self to nodes
            Node1.Edges[(int) node1Direction] = this;
            Node2.Edges[(int) node2Direction] = this;
        }

        #endregion

        #region Methods

        public static void AddEdge(Node node1, Direction node1Direction, Node node2, Direction node2Direction)
        {
            AddEdge(node1, node1Direction, node2, node2Direction, 1);
        }

        public static void AddEdge(Node node1, Direction node1Direction, Node node2, Direction node2Direction, int cost)
        {
            _edges.Add(new Edge(node1, node1Direction, node2, node2Direction, cost));
        }

        #endregion
    }
}
