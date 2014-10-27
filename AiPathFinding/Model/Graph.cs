using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Graph
    {
        #region Fields

        public Node[][] Nodes { get; set; }

        private static readonly Dictionary<NodeType, char> NodeTypeToChar = new Dictionary<NodeType, char> { { NodeType.Street, 'S' }, { NodeType.Plains, 'P' }, { NodeType.Forest, 'F' }, { NodeType.Hill, 'H' }, { NodeType.Mountain, 'M' } }; 

        private static readonly Dictionary<char, NodeType> CharToNodeType = new Dictionary<char, NodeType>(); 

        #endregion

        #region Constructor

        static Graph()
        {
            CharToNodeType = NodeTypeToChar.GroupBy(p => p.Value).ToDictionary(g => g.Key, g => g.Select(pp => pp.Key).ToList()[0]);
        }

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

        public static Graph FromMap(string[] data)
        {
            // create array
            var nodes = new Node[data[1].Count(x => x == ';') + 1][];


            // fill nodes
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new Node[data.Length - 1];
                for (var j = 0; j < nodes[i].Length; j++)
                {
                    var cellData = data[j + 1].Split(';')[i];
                    nodes[i][j] = new Node(new Point(i, j), !cellData.Contains('*'), CharToNodeType[cellData[0]]);
                }
            }

            // add edges
            for (var i = 0; i < nodes.Length; i++)
                for (var j = 0; j < nodes[i].Length; j++)
                {
                    if (i < nodes.Length - 1)
                        Edge.AddEdge(nodes[i][j], Direction.East, nodes[i + 1][j], Direction.West);
                    if (j < nodes[i].Length - 1)
                        Edge.AddEdge(nodes[i][j], Direction.South, nodes[i][j + 1], Direction.North);
                }

            return new Graph(nodes);
        }

        #endregion

        #region Methods

        public Node GetNode(Point p)
        {
            return Nodes[p.X][p.Y];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Nodes[0].Length && Nodes[0][i] != null; i++)
            {
                for (var j = 0; j < Nodes.Length && Nodes[j] != null; j++)
                {
                    // can store any information about node here
                    // but first char MUST be the terrain type
                    sb.Append(NodeTypeToChar[Nodes[j][i].Type]);
                    if (!Nodes[j][i].KnownToPlayer)
                        sb.Append('*');

                    if (j < Nodes.Length - 1 && Nodes[j + 1] != null)
                        sb.Append(';');
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }

        #endregion
    }
}
