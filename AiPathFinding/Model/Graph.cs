using System.Drawing;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Graph
    {
        #region Fields

        public Node[][] Nodes { get; set; }

        #endregion

        #region Constructor

        private Graph(Node[][] nodes)
        {
            Nodes = nodes;
        }

        #endregion

        #region Creation Methods
        public static Graph EmptyGraph(int width, int height)
        {
            // create array
            var nodes = new Node[width][];

            // fill nodes
            for (var i = 0; i < width; i++)
            {
                nodes[i] = new Node[height];
                for (var j = 0; j < height; j++)
                    nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);
            }

            // add edges
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                {
                    if (i < width - 1)
                        Edge.AddEdge(nodes[i][j], Direction.East, nodes[i + 1][j], Direction.West);
                    if (j < height - 1)
                        Edge.AddEdge(nodes[i][j], Direction.South, nodes[i][j + 1], Direction.North);
                }

            return new Graph(nodes);
        }

        #endregion
    }
}
