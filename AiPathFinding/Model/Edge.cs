using System;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    /// <summary>
    /// Represents the connection between two nodes.
    /// </summary>
    public sealed class Edge
    {
        #region Fields

        /// <summary>
        /// One of the connected nodes.
        /// </summary>
        public Node Node1 { get; private set; }

        /// <summary>
        /// The other connected node.
        /// </summary>
        public Node Node2 { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Only way to instantiate edges. Ensures that all references are set properly between edge and nodes.
        /// </summary>
        /// <param name="node1">First node</param>
        /// <param name="node1Direction">Direction of the edge for the first node</param>
        /// <param name="node2">Second node</param>
        /// <param name="node2Direction">Direction of the edge for the second node</param>
        private Edge(Node node1, Direction node1Direction, Node node2, Direction node2Direction)
        {
            // validate parameters: Direction must be "complementary"
            if (((int)node1Direction + (int)node2Direction) % 2 != 0)
                throw new ArgumentException();

            // assign fields
            Node1 = node1;
            Node2 = node2;

            // assign self to nodes
            // this is what keeps edges from being GarbageCollected
            Node1.Edges[(int) node1Direction] = this;
            Node2.Edges[(int) node2Direction] = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Given one node, it retrieves the other node connected to the edge.
        /// </summary>
        /// <param name="node">One of the connected nodes</param>
        /// <returns>The other connected node</returns>
        public Node GetOtherNode(Node node)
        {
            if (node == Node1)
                return Node2;
            if (node == Node2)
                return Node1;

            throw new Exception("node isnt part of edge");
        }

        /// <summary>
        /// Adds a new edge between two nodes
        /// </summary>
        /// <param name="node1">First node</param>
        /// <param name="node1Direction">Direction of the edge for the first node</param>
        /// <param name="node2">Second node</param>
        /// <param name="node2Direction">Direction of the edge for the second node</param>
        public static void AddEdge(Node node1, Direction node1Direction, Node node2, Direction node2Direction)
        {
            new Edge(node1, node1Direction, node2, node2Direction);
        }

        #endregion
    }
}
