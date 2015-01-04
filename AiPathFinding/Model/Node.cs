using System.Collections.Generic;
using System.Drawing;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    /// <summary>
    /// Represents a node in a graph.
    /// </summary>
    public sealed class Node
    {
        #region Fields

        /// <summary>
        /// Cost which is required to travel onto that node.
        /// </summary>
        public int Cost { get { return TerrainCostMap[Terrain]; } }

        /// <summary>
        /// Contains the mapping of terrain to the cost.
        /// </summary>
        public static readonly Dictionary<Terrain, int> TerrainCostMap = new Dictionary<Terrain, int>
        {
            {Terrain.Street, 1},
            {Terrain.Plains, 2},
            {Terrain.Forest, 3},
            {Terrain.Hill, 5},
            {Terrain.Mountain, int.MaxValue}
        };

        /// <summary>
        /// Terrain of the node.
        /// </summary>
        public Terrain Terrain { get; set; }

        /// <summary>
        /// Location (on the graph) of the node.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// True if it is not a foggy node.
        /// </summary>
        public bool KnownToPlayer { get; set; }

        /// <summary>
        /// Edges, one for each possible direction.
        /// </summary>
        public readonly Edge[] Edges = new Edge[4];

        /// <summary>
        /// Entity that is located on the node.
        /// </summary>
        public Entity EntityOnNode { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new node with basic node data.
        /// </summary>
        /// <param name="location">Location in the graph</param>
        /// <param name="knownToAi">True if the node is not foggy</param>
        /// <param name="terrain">Terrain of the node</param>
        public Node(Point location, bool knownToAi, Terrain terrain)
        {
            Location = location;
            KnownToPlayer = knownToAi;
            Terrain = terrain;
        }

        #endregion

        #region Methods

        /// <summary>
        /// String representation (like a point, more or less).
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "(" + Location.X + "/" + Location.Y + ")";
        }

        #endregion
    }
}
