using System;
using System.Drawing;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Map
    {
        #region Fields

        private Graph Graph { get; set; }

        #endregion

        #region Events

        public delegate void OnCellTypeChanged(Point locatoin, NodeType oldType, NodeType newType);

        public event OnCellTypeChanged CellTypeChanged;

        public delegate void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight);

        public event OnMapSizeChanged MapSizeChanged;

        #endregion

        #region Constructor

        public Map(Graph graph)
        {
            Graph = graph;
        }

        #endregion

        #region Methods

        public NodeType GetNodeType(Point location)
        {
            return Graph.Nodes[location.X][location.Y].Type;
        }

        public void SetNodeType(Point location, NodeType type)
        {
            // set node type and trigger event
            var oldType = Graph.Nodes[location.X][location.Y].Type;
            Graph.Nodes[location.X][location.Y].Type = type;
            CellTypeChanged(location, oldType, type);
        }

        public void SetMapSize(int width, int height)
        {
            // get current width/height
            var oldWidth = Graph.Nodes.Length;
            while (Graph.Nodes[oldWidth - 1] == null)
                oldWidth--;
            var oldHeight = Graph.Nodes[0].Length;
            while (Graph.Nodes[0][oldHeight - 1] == null)
                oldHeight--;

            if (width == oldWidth && height == oldHeight)
                return;

            if (width < oldWidth)
            {
                // if width decreased, set pointers null
                for (var i = width; i < oldWidth; i++)
                    Graph.Nodes[i] = null;

                // remove east-bound edges in left-most column
                foreach (var n in Graph.Nodes[width - 1])
                    n.Edges[(int) Direction.East] = null;
            }
            
            if (width > oldWidth)
            {
                // if width increased, copy to new array if necessary
                if (width > Graph.Nodes.Length)
                {
                    var newNodes = new Node[width][];
                    Array.Copy(Graph.Nodes, newNodes, Graph.Nodes.Length);
                    for (var i = Graph.Nodes.Length; i < width; i++)
                        newNodes[i] = new Node[Graph.Nodes[0].Length];
                    Graph.Nodes = newNodes;
                }

                // initialize new nodes
                for (var i = oldWidth; i < width; i++)
                {
                    Graph.Nodes[i] = new Node[Graph.Nodes[0].Length];
                    for (var j = 0; j < height; j++)
                        Graph.Nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);
                }

                // add new edges
                for (var j = 0; j < height; j++)
                {
                    Edge.AddEdge(Graph.Nodes[width - 2][j], Direction.East, Graph.Nodes[width - 1][j], Direction.West);
                    
                    if (j < height - 1)
                        Edge.AddEdge(Graph.Nodes[width - 1][j], Direction.South, Graph.Nodes[width - 1][j + 1], Direction.North);
                }
            }

            // if height decreased, set pointers null
            if (oldHeight > height)
            {
                for (var i = 0; i < width; i++)
                    for (var j = height; j < oldHeight; j++)
                        Graph.Nodes[i][j] = null;

                // remove south-bound edges in bottom row
                for (var i = 0; i < width; i++)
                    Graph.Nodes[i][height - 1].Edges[(int) Direction.South] = null;
            }

            // if height increased, copy to new array if necessary
            if (oldHeight < height)
            {
                if (height > Graph.Nodes[0].Length)
                    for (var i = 0; i < Graph.Nodes.Length; i++)
                    {
                        if (Graph.Nodes[i] == null)
                            continue;

                        var newArray = new Node[height];
                        Array.Copy(Graph.Nodes[i], newArray, Graph.Nodes[i].Length);
                        Graph.Nodes[i] = newArray;
                    }

                // add nodes
                for (var i = 0; i < width; i++)
                    for (var j = oldHeight; j < height; j++)
                        Graph.Nodes[i][j] = new Node(new Point(i, j), true, NodeType.Street);

                // add new edges
                for (var i = 0; i < width; i++)
                {
                    Edge.AddEdge(Graph.Nodes[i][height - 2], Direction.South, Graph.Nodes[i][height - 1], Direction.North);

                    if (i < width - 1)
                        Edge.AddEdge(Graph.Nodes[i][height - 1], Direction.East, Graph.Nodes[i + 1][height - 1], Direction.West);
                }
            }

            // ensure all arrays are "similar"
            foreach (var t in Graph.Nodes)
                if (t != null && t.Length != Graph.Nodes[0].Length)
                    throw new Exception("Array size doesnt match!");
                else if (t != null && height > Graph.Nodes[0].Length && t[height] == null)
                    throw new Exception("should be null!");

            MapSizeChanged(oldWidth, oldHeight, width, height);
        }

        #endregion
    }
}
