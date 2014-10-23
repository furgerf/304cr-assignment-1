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
                switch (Type)
                {
                    case NodeType.Street:
                        return 1;
                    case NodeType.Plains:
                        return 2;
                    case NodeType.Forest:
                        return 3;
                    case NodeType.Hill:
                        return 5;
                    case NodeType.Mountain:
                        return int.MaxValue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public NodeType Type { get; set; }

        public Point Location { get; private set; }

        public bool KnownToPlayer { get; set; }

        public readonly Edge[] Edges = new Edge[4];

        public Entity EntityOnNode { get; set; }

        #endregion

        #region Constructor

        public Node(Point location, bool knownToAi, NodeType type)
        {
            Location = location;
            KnownToPlayer = knownToAi;
            Type = type;
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
