using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    /// <summary>
    /// Class that implements the graph of the map model.
    /// </summary>
    public sealed class Graph
    {
        #region Fields

        /// <summary>
        /// Nodes of the graph. Arranged in a matrix since the graph forms a matrix...
        /// </summary>
        public Node[][] Nodes { get; set; }

        /// <summary>
        /// Counts all nodes that can be visited by the player.
        /// </summary>
        public int PassibleNodeCount
        {
            get { return Nodes.Sum(n => n.Count(nn => nn.Cost != int.MaxValue)); }
        }

        /// <summary>
        /// Maps the string (character) representation of terrain to the enum type.
        /// </summary>
        private static readonly Dictionary<Terrain, char> TerrainToChar = new Dictionary<Terrain, char> { { Terrain.Street, 'S' }, { Terrain.Plains, 'P' }, { Terrain.Forest, 'F' }, { Terrain.Hill, 'H' }, { Terrain.Mountain, 'M' } }; 

        /// <summary>
        /// Reverses the above mapping.
        /// </summary>
        private static readonly Dictionary<char, Terrain> CharToTerrain = new Dictionary<char, Terrain>(); 

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Graph()
        {
            // creates the reverse mapping
            // this ensures that both dictionaries complement eachother properly
            CharToTerrain = TerrainToChar.GroupBy(p => p.Value).ToDictionary(g => g.Key, g => g.Select(pp => pp.Key).ToList()[0]);
        }

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="nodes">Creates a new graph with given nodes</param>
        private Graph(Node[][] nodes)
        {
            Nodes = nodes;

            Console.WriteLine("New graph instantiated");
        }

        #endregion

        #region Static Graph Creation Methods

        /// <summary>
        /// Creates an empty graph.
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <returns>New map</returns>
        public static Graph EmptyGraph(int width, int height)
        {
            Console.WriteLine("Creating new empty graph");

            // create array
            var nodes = new Node[width][];

            // fill nodes
            for (var i = 0; i < width; i++)
            {
                nodes[i] = new Node[height];
                for (var j = 0; j < height; j++)
                    nodes[i][j] = new Node(new Point(i, j), true, Terrain.Street);
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

        /// <summary>
        /// Creates a map from data that has been stored in a text file.
        /// </summary>
        /// <param name="data">String representation of the data, lines of the array representing lines in the file</param>
        /// <returns>New map</returns>
        public static Graph FromData(string[] data)
        {
            Console.WriteLine("Creating new graph from data");

            // create array
            var nodes = new Node[data[1].Count(x => x == ';') + 1][];

            // fill nodes
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new Node[data.Length - 1];
                for (var j = 0; j < nodes[i].Length; j++)
                {
                    var cellData = data[j + 1].Split(';')[i];
                    nodes[i][j] = new Node(new Point(i, j), !cellData.Contains('*'), CharToTerrain[cellData[0]]);
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

        /// <summary>
        /// Creates new random map.
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="weights">Weights for the different terrains, summed up</param>
        /// <param name="fog">Percentage of fog</param>
        /// <returns>New map</returns>
        public static Graph Random(int width, int height, double[] weights, double fog)
        {
            Console.WriteLine("Creating new random graph");

            var rnd = new Random();

            // create array
            var nodes = new Node[width][];

            // fill nodes
            for (var i = 0; i < width; i++)
            {
                nodes[i] = new Node[height];
                for (var j = 0; j < height; j++)
                {
                    // for some reason, it looks like the terrain rnd needs to be stored...
                    var r = rnd.NextDouble();
                    var t = (Terrain) 5 - weights.Count(w => r < w);

                    // ensure that entities don't get stuck on mountains
                    foreach (var e in Entity.Entities.Where(e => e.Node != null))
                        while (t == Terrain.Mountain && e.Node.Location == new Point(i, j))
                        {
                            r = rnd.NextDouble();
                            t = (Terrain)5 - weights.Count(w => r < w);
                        }

                    nodes[i][j] = new Node(new Point(i, j), Entity.Player.Node != null && Entity.Player.Node.Location == new Point(i, j) || rnd.NextDouble() > fog, t);
                }
            }

            // update entity locations
            foreach (var e in Entity.Entities.Where(e => e.Node != null))
                e.Node = nodes[e.Node.Location.X][e.Node.Location.Y];

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

        #region Methods

        /// <summary>
        /// Simplified access to a specific node with the node's location point.
        /// </summary>
        /// <param name="p">Location of the node</param>
        /// <returns>Node</returns>
        public Node GetNode(Point p)
        {
            return Nodes[p.X][p.Y];
        }

        //public Graph DeepCopy()
        //{
        //    return FromData(ToString().Split('\n'));
        //}

        /// <summary>
        /// Text representation of a graph; Encodes terrain according to static map; fog as *; cells separated by semicolon (CSV).
        /// </summary>
        /// <returns>Map-string</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Nodes[0].Length && Nodes[0][i] != null; i++)
            {
                for (var j = 0; j < Nodes.Length && Nodes[j] != null; j++)
                {
                    // can store any information about node here
                    // but first char MUST be the terrain type
                    sb.Append(TerrainToChar[Nodes[j][i].Terrain]);
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
