using System;
using System.Drawing;

namespace AiPathFinding.Model
{
    public class Node
    {
        #region Fields

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

        public Terrain Terrain { get; set; }

        public Point Location { get; private set; }

        public bool KnownToPlayer { get; set; }

        public readonly Edge[] Edges = new Edge[4];

        public Entity EntityOnNode { get; set; }

        #endregion

        #region Constructor

        public Node(Point location, bool knownToAi, Terrain terrain)
        {
            Location = location;
            KnownToPlayer = knownToAi;
            Terrain = terrain;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "(" + Location.X + "/" + Location.Y + ")";
        }

        #endregion
    }
}
