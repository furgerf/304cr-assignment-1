using System.Drawing;

namespace AiPathFinding
{
    public class Graph
    {
        public Node[][] Nodes { get; set; }

        public static Graph EmptyGraph(int width, int height)
        {
            var nodes = new Node[width][];

            for (var i = 0; i < width; i++)
            {
                nodes[i] = new Node[height];
                for (var j = 0; j < height; j++)
                    nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);
            }

            for (var i = 0; i < width - 1; i++)
                for (var j = 0; j < height - 1; j++)
                {
                    Edge.AddEdge(nodes[i][j], Direction.East, nodes[i + 1][j], Direction.West);
                    Edge.AddEdge(nodes[i][j], Direction.South, nodes[i][j + 1], Direction.North);
                }

            return new Graph(nodes);
        }

        private Graph(Node[][] nodes)
        {
            Nodes = nodes;
        }
    }
}
