using System.Drawing;

namespace AiPathFinding.Model
{
    public class Node
    {
        public Node(Point location, bool knownToAi, NodeType type)
        {
            Location = location;
            KnownToAi = knownToAi;
            Type = type;
        }

        public NodeType Type { get; set; }

        public Point Location { get; private set; }

        public bool KnownToAi { get; set; }

        public readonly Edge[] Edges = new Edge[4];

        public override string ToString()
        {
            return "(" + Location.X + "/" + Location.Y + ")";
        }
    }
}
