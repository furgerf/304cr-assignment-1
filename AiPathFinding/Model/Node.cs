using System;
using System.Drawing;

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
        public int Cost
        {
            get
            {
                switch (Terrain)
                {
                    case Terrain.Street:
                        return 1;
                    case Terrain.Plains:
                        return 2;
                    case Terrain.Forest:
                        return 3;
                    case Terrain.Hill:
                        return 5;
                    case Terrain.Mountain:
                        return int.MaxValue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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
