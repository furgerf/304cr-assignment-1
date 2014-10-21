using System;
using System.Drawing;

namespace AiPathFinding
{
    public class Model
    {

        public delegate void OnCellTypeChanged(Point locatoin, NodeType oldType, NodeType newType);

        public event OnCellTypeChanged CellTypeChanged;

        public delegate void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight);

        public event OnMapSizeChanged MapSizeChanged;

        public Model(Graph graph)
        {
            Graph = graph;
        }

        public NodeType GetNodeType(Point location)
        {
            return Graph.Nodes[location.X][location.Y].Type;
        }

        public void SetNodeType(Point location, NodeType type)
        {
            var oldType = Graph.Nodes[location.X][location.Y].Type;
            Graph.Nodes[location.X][location.Y].Type = type;
            CellTypeChanged(location, oldType, type);
        }

        public void SetMapSize(int width, int height)
        {
            var oldWidth = Graph.Nodes.Length;
            var oldHeight = Graph.Nodes[0].Length;

            if (width == oldWidth && height == oldHeight)
                return;
            
            if (width != oldWidth)
            {
                // if width decreased, set pointers null
                for (var i = width; i < oldWidth; i++)
                    Graph.Nodes[i] = null;

                // if width increased, copy to new array if necessary
                if (oldWidth < width)
                {
                    if (width > Graph.Nodes.Length)
                    {
                        var newNodes = new Node[width][];
                        Array.Copy(Graph.Nodes, newNodes, Graph.Nodes.Length);
                        for (var i = Graph.Nodes.Length; i < width; i++)
                            newNodes[i] = new Node[Graph.Nodes[0].Length];
                        Graph.Nodes = newNodes;
                    }

                    // initialize null-pointers with new arrays and nodes
                    for (var i = oldWidth; i < width; i++)
                    {
                        Graph.Nodes[i] = new Node[height];
                        for (var j = 0; j < height; j++)
                            Graph.Nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);
                    }
                }
            }
            else if (height != oldHeight)
            {
                // if height decreased, set pointers null
                if (oldHeight > height)
                    for (var i = 0; i < width; i++)
                        for (var j = height; j < oldHeight; j++)
                            Graph.Nodes[i][j] = null;

                // if height increased, copy to new array if necessary
                if (oldHeight < height)
                {
                    if (height > Graph.Nodes[0].Length)
                        for (var i = 0; i < Graph.Nodes.Length; i++)
                        {
                            var newArray = new Node[height];
                            Array.Copy(Graph.Nodes[i], newArray, Graph.Nodes[i].Length);
                            Graph.Nodes[i] = newArray;
                        }

                    // add nodes
                    for (var i = 0; i < width; i++)
                        for (var j = oldHeight; j < height; j++)
                            Graph.Nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);
                }
            }

            // ensure all arrays are "similar"
            foreach (var t in Graph.Nodes)
                if (t != null && t.Length != Graph.Nodes[0].Length)
                    throw new Exception("Array size doesnt match!");
                else if (t != null && t.Length != height && t[height] == null)
                    throw new Exception("should be null!");

            MapSizeChanged(oldWidth, oldHeight, width, height);
        }

        private Graph Graph { get; set; }
    }
}
