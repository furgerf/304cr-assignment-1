using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    public class Map
    {
        #region Fields

        private Graph Graph { get; set; }

        public int Width
        {
            get
            {
                var width = Graph.Nodes.Length;
                while (Graph.Nodes[width - 1] == null)
                    width--;
                return width;
            }
        }

        public int Height
        {
            get
            {
                var height = Graph.Nodes[0].Length;
                while (Graph.Nodes[0][height - 1] == null)
                    height--;
                return height;
            }
        }

        #endregion

        #region Events

        public delegate void OnCellTerrainChanged(Point location, Terrain oldTerrain, Terrain newTerrain);

        public event OnCellTerrainChanged CellTerrainChanged;

        public delegate void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight);

        public event OnMapSizeChanged MapSizeChanged;

        public delegate void OnEntityNodeChanged(Node oldNode, Node newNode, Entity entity);

        public event OnEntityNodeChanged EntityNodeChanged;

        public delegate void OnMapLoaded();

        public event OnMapLoaded MapLoaded;

        public delegate void OnFogChanged(Point location, bool hasFog);

        public event OnFogChanged FogChanged;

        #endregion

        #region Constructor

        public Map(Graph graph)
        {
            Graph = graph;

            Entity.NodeChanged += (o, n, e) => { if (EntityNodeChanged != null) EntityNodeChanged(o, n, e); };
        }

        public static Map FromMapFile(string path)
        {
            var map = new Map(null);

            map.LoadMap(path);

            return map;
        }

        #endregion

        #region Methods

        public Graph GetGraph()
        {
            return Graph;
        }

        public void SetEntityLocation(Entity player, Point location)
        {
            // set location and trigger event
            var oldNode = player.Node;
            player.Node = Graph.GetNode(location);
            if (EntityNodeChanged != null)
                EntityNodeChanged(oldNode, player.Node, player);
        }

        public Terrain GetTerrain(Point location)
        {
            return Graph.Nodes[location.X][location.Y].Terrain;
        }

        public void SetTerrain(Point location, Terrain terrain)
        {
            // set node type and trigger event
            var oldType = Graph.Nodes[location.X][location.Y].Terrain;
            Graph.Nodes[location.X][location.Y].Terrain = terrain;
            if (CellTerrainChanged != null)
                CellTerrainChanged(location, oldType, terrain);
        }

        public void SetFog(Point location, bool hasFog)
        {
            Graph.Nodes[location.X][location.Y].KnownToPlayer = !hasFog;
            if (FogChanged != null)
                FogChanged(location, Graph.Nodes[location.X][location.Y].KnownToPlayer);
        }

        public bool GetFog(Point location)
        {
            return !Graph.Nodes[location.X][location.Y].KnownToPlayer;
        }

        public bool HasFog(Point location)
        {
            return !Graph.Nodes[location.X][location.Y].KnownToPlayer;
        }

        public void SetMapSize(int width, int height)
        {
            // get current width/height
            var oldWidth = Width;
            var oldHeight = Height;

            if (width == oldWidth && height == oldHeight)
                return;

            if (width < oldWidth)
            {
                // if width decreased, set pointers null
                for (var i = width; i < oldWidth; i++)
                {
                    foreach (var n in Graph.Nodes[i].Where(n => n != null &&  n.EntityOnNode != null))
                        n.EntityOnNode.Node = null;

                    Graph.Nodes[i] = null;
                }

                // remove east-bound edges in left-most column
                for (var r = 0; r < height; r++)
                     Graph.Nodes[width - 1][r].Edges[(int) Direction.East] = null;
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
                        Graph.Nodes[i][j] = new Node(new Point(i, j), true, Terrain.Street);
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
                    {
                        if (Graph.Nodes[i][j].EntityOnNode != null)
                            Graph.Nodes[i][j].EntityOnNode.Node = null;

                        Graph.Nodes[i][j] = null;
                    }

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
                        Graph.Nodes[i][j] = new Node(new Point(i, j), true, Terrain.Street);

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

            if (MapSizeChanged != null)
                MapSizeChanged(oldWidth, oldHeight, width, height);
        }

        public void SaveMap(string path)
        {
            var data = "";
            for (var i = 0; i < (int) EntityType.Count; i++)
            {
                if (Entity.Entities[i].Node != null)
                    data += "\t" + (EntityType) i + ";" + Entity.Entities[i].Node.Location;
            }

            if (data != "")
                data = data.Substring(1);
            
            data += "\n" + Graph;

            File.WriteAllText(path, data);
        }

        public void LoadMap(string path)
        {
            var data = File.ReadAllLines(path);
            
            // create graph
            Graph = Graph.FromMap(data);

            // move entities
            foreach (var e in Entity.Entities)
                e.Node = null;
            var split = data[0].Split('\t');
            foreach (var s in split)
            {
                var e = s.Split(';');
                var g = Regex.Replace(e[1], @"[\{\}a-zA-Z=]", "").Split(',');
                var p = new Point(int.Parse(g[0]), int.Parse(g[1]));
                Entity.Entities[(int) Enum.Parse(typeof (EntityType), e[0])].Node = Graph.GetNode(p);
            }

            // trigger event
            if (MapLoaded != null)
                MapLoaded();
        }

        public void RegenerateMap(int width, int height, int street, int plains, int forest, int hill, int mountain, double fog)
        {
            // create graph
            double sum = street + plains + forest + hill + mountain;
            var weights = new double[] {street, plains, forest, hill, mountain};
            for (var i = 1; i < weights.Length; i++)
                weights[i] += weights[i - 1];
            for (var i = 0; i < weights.Length; i++)
                weights[i] /= sum;
            Graph = Graph.Random(width, height, weights, fog);

            // ignore entities

            // trigger event
            if (MapLoaded != null)
                MapLoaded();
        }

        #endregion
    }
}
