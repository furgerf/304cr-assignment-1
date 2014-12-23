using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AiPathFinding.Common;

namespace AiPathFinding.Model
{
    /// <summary>
    /// Map of the model.
    /// </summary>
    public sealed class Map
    {
        #region Fields

        /// <summary>
        /// Graph representation of the map which is used by the algorithms.
        /// </summary>
        public Graph Graph { get; private set; }

        /// <summary>
        /// Width of the map.
        /// </summary>
        public int Width
        {
            get
            {
                // get nodes length and remove empty columns
                var width = Graph.Nodes.Length;
                while (Graph.Nodes[width - 1] == null)
                    width--;
                return width;
            }
        }

        /// <summary>
        /// Height of the map.
        /// </summary>
        public int Height
        {
            get
            {
                // get nodes length and remove empty rows
                var height = Graph.Nodes[0].Length;
                while (Graph.Nodes[0][height - 1] == null)
                    height--;
                return height;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Triggered when a cell's terrain changes.
        /// </summary>
        /// <param name="location">Cell location</param>
        /// <param name="oldTerrain">Old terrain</param>
        /// <param name="newTerrain">New terrain</param>
        public delegate void OnCellTerrainChanged(Point location, Terrain oldTerrain, Terrain newTerrain);

        public event OnCellTerrainChanged CellTerrainChanged;

        /// <summary>
        /// Triggered when the map's size changes.
        /// </summary>
        /// <param name="oldWidth">Old width</param>
        /// <param name="oldHeight">Old height</param>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        public delegate void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight);

        public event OnMapSizeChanged MapSizeChanged;

        /// <summary>
        /// Triggered when the map is loaded.
        /// </summary>
        public delegate void OnMapLoaded();

        public event OnMapLoaded MapLoaded;

        /// <summary>
        /// Triggered when a cell's fog changes.
        /// </summary>
        /// <param name="location">Location of the cell</param>
        /// <param name="hasFog">True if the cell now is foggy</param>
        public delegate void OnFogChanged(Point location, bool hasFog);

        public event OnFogChanged FogChanged;

        #endregion

        #region Constructor and Static Creation

        /// <summary>
        /// Creates a new map with a given graph.
        /// </summary>
        /// <param name="graph">Graph of the model</param>
        public Map(Graph graph)
        {
            Graph = graph;

            Console.WriteLine("New map instantiated");
        }

        /// <summary>
        /// Creates a new map from a text file.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>New map</returns>
        public static Map FromMapFile(string path)
        {
            Console.WriteLine("Loading map from file");

            var map = new Map(null);

            map.LoadMap(path);

            return map;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the location of an entity.
        /// </summary>
        /// <param name="player">Entity to move</param>
        /// <param name="location">New location of the entity</param>
        public void SetEntityLocation(Entity player, Point location)
        {
            player.Node = Graph.GetNode(location);
        }

        /// <summary>
        /// Gets the terrain of a specific cell.
        /// </summary>
        /// <param name="location">Location of the cell</param>
        /// <returns>Terrain of the cell</returns>
        public Terrain GetTerrain(Point location)
        {
            return Graph.GetNode(location).Terrain;
        }

        /// <summary>
        /// Sets the terrain of a specific cell.
        /// </summary>
        /// <param name="location">Location of the cell</param>
        /// <param name="terrain">New terrain of the cell</param>
        public void SetTerrain(Point location, Terrain terrain)
        {
            // set node type and trigger event
            var oldType = Graph.Nodes[location.X][location.Y].Terrain;
            Graph.Nodes[location.X][location.Y].Terrain = terrain;
            if (CellTerrainChanged != null)
                CellTerrainChanged(location, oldType, terrain);
        }

        /// <summary>
        /// Changes the fog on a specific cell.
        /// </summary>
        /// <param name="location">Node where the fog should be changed</param>
        /// <param name="hasFog">True if fog should be set, false if it should be cleared</param>
        public void SetFog(Point location, bool hasFog)
        {
            // set fog and trigger event
            Graph.Nodes[location.X][location.Y].KnownToPlayer = !hasFog;
            if (FogChanged != null)
                FogChanged(location, Graph.Nodes[location.X][location.Y].KnownToPlayer);
        }

        /// <summary>
        /// Gets the fog on a specific cell.
        /// </summary>
        /// <param name="location">Location of the cell</param>
        /// <returns>True if the cell is foggy</returns>
        public bool HasFog(Point location)
        {
            return !Graph.Nodes[location.X][location.Y].KnownToPlayer;
        }

        /// <summary>
        /// Changes the size of the map. If width/height of the map is greater than before, the map is filled with empty cells (streets).
        /// </summary>
        /// <param name="width">New width of the map</param>
        /// <param name="height">New height of the map</param>
        public void SetMapSize(int width, int height)
        {
            Console.WriteLine("Changing map size");

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

        /// <summary>
        /// Saves the map to a file.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void SaveMap(string path)
        {
            Console.WriteLine("Saving current map to " + path);

            // gather data: entities
            var data = "";
            for (var i = 0; i < (int) EntityType.Count; i++)
            {
                if (Entity.Entities[i].Node != null)
                    data += "\t" + (EntityType) i + ";" + Entity.Entities[i].Node.Location;
            }

            if (data != "")
                data = data.Substring(1);
            
            // add graph to data
            data += "\n" + Graph;

            // write data
            File.WriteAllText(path, data);
        }

        /// <summary>
        /// Loads a map from a text file.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void LoadMap(string path)
        {
            Console.WriteLine("Loading map from " + path);

            var data = File.ReadAllLines(path);
            
            // create graph
            Graph = Graph.FromData(data);

            // move entities
            foreach (var e in Entity.Entities)
                e.Node = null;
            if (data[0] != "")
            {
                var split = data[0].Split('\t');
                foreach (var s in split)
                {
                    var e = s.Split(';');
                    var g = Regex.Replace(e[1], @"[\{\}a-zA-Z=]", "").Split(',');
                    var p = new Point(int.Parse(g[0]), int.Parse(g[1]));
                    Entity.Entities[(int)Enum.Parse(typeof(EntityType), e[0])].Node = Graph.GetNode(p);
                }
            }

            // trigger event
            if (MapLoaded != null)
                MapLoaded();
        }

        /// <summary>
        /// Creates a new random graph.
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="street">Weight of street cells</param>
        /// <param name="plains">Weight of plains cells</param>
        /// <param name="forest">Weight of forest cells</param>
        /// <param name="hill">Weight of hill cells</param>
        /// <param name="mountain">Weight of mountain cells</param>
        /// <param name="fog">Percentage of foggy cells</param>
        public void RegenerateMap(int width, int height, int street, int plains, int forest, int hill, int mountain, double fog)
        {
            Console.WriteLine("Regenerating map");

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
