using System;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Edge
    {
        public static void AddEdge(Node node1, Direction node1Direction, Node node2, Direction node2Direction, int cost)
        {
            new Edge(node1, node1Direction, node2, node2Direction, cost);
        }

        public static void AddEdge(Node node1, Direction node1Direction, Node node2, Direction node2Direction)
        {
            new Edge(node1, node1Direction, node2, node2Direction, 1);
        }

        private Edge(Node node1, Direction node1Direction, Node node2, Direction node2Direction, int cost)
        {
            Node1 = node1;
            Node2 = node2;
            Cost = cost;

            if (((int)node1Direction + (int)node2Direction) % 2 != 0)
                throw new ArgumentException();
            Node1.Edges[(int)node1Direction] = this;
            Node2.Edges[(int)node2Direction] = this;
        }

        public Node Node1 { get; private set; }
        public Node Node2 { get; private set; }
        public int Cost { get; private set; }
    }
}
