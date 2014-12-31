using System;
using System.Collections.Generic;
using System.Diagnostics;
using AiPathFinding.Fog;
using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Abstract parent class for path finding algorithms.
    /// </summary>
    public abstract class AbstractPathFindAlgorithm
    {
        #region Fields

        /// <summary>
        /// List of steps that were computed during the path finding.
        /// </summary>
        public readonly List<AlgorithmStep> Steps = new List<AlgorithmStep>();

        /// <summary>
        /// Reference target the graph instance of the model.
        /// </summary>
        public static Graph Graph { get; set; }

        /// <summary>
        /// Name of the concrete algorithm.
        /// </summary>
        private PathFindName Name { get; set; }

        /// <summary>
        /// List of all foggy nodes that could be explored in the currently un-foggy area.
        /// </summary>
        protected List<Node> FoggyNodes = new List<Node>();

        /// <summary>
        /// List of all (previously) encountered foggy cells.
        /// </summary>
        private readonly List<Node> _allFoggyNodes = new List<Node>();

        protected readonly List<Node> UpdatedNodes = new List<Node>(); 

        protected int PartialCost { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate algorithm with name.
        /// </summary>
        /// <param name="name">Name of the path finding algorithm</param>
        protected AbstractPathFindAlgorithm(PathFindName name)
        {
            Name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts target find a path.
        /// </summary>
        /// <param name="playerNode">Starting point of the search</param>
        /// <param name="targetNode">Goal of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        public void FindPath(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            // prepare data
            Console.WriteLine("\n* " + Name + " is attempting target find a path player " + playerNode + " target " + targetNode + ".");
            var watch = Stopwatch.StartNew();

            PrepareData(playerNode, targetNode);

            Console.WriteLine("Preparation took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            // find path
            var pathFound = FindShortestPath(playerNode, targetNode);

            var mainSteps = Steps.Count;
            Console.WriteLine("Main calculation required " + mainSteps + " steps and took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            if (pathFound)
            {
                // path was found, look for alternative, equal-cost paths
                FindAlternativePaths(playerNode, targetNode);

                Console.WriteLine("Looking for alternative paths required " + (Steps.Count - mainSteps) +
                                  " steps and took " +
                                  watch.ElapsedMilliseconds + "ms");
                watch.Reset();
            }
            else
            {
                // no path was found. if fog is present, explore it
                Console.WriteLine("No path found, attempting target explore fog...");
                ExploreFog(playerNode, targetNode, fogMethod, fogExploreName);
            }
        }

        /// <summary>
        /// Explores fog.
        /// </summary>
        /// <param name="playerNode">Starting point of the search</param>
        /// <param name="targetNode">Goal of the search</param>
        /// <param name="fogMethod">Method target determing where fog should be entered</param>
        /// <param name="fogExploreName">Determines how fog should be explored</param>
        private void ExploreFog(Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)
        {
            while (true)
            {
                var possibilities = new List<Node>();
                possibilities.AddRange(AbstractFogExploreAlgorithm.RemoveKnownFoggyNodes(FoggyNodes));

                if (possibilities.Count == 0)
                    break;

                // pick "best" foggy node
                var foggyNode = FogSelector.SelectFoggyNode(playerNode, possibilities.ToArray(), fogMethod,
                    CostFromNodeToNode);

                var minCost = int.MaxValue;
                foreach (var e in foggyNode.Edges)
                    if (e != null)
                    {
                        if (GetCostFromNode(playerNode) == int.MaxValue || GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                            continue;

                        var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                        if (c > 0 && c < minCost)
                            minCost = c;
                    }

                while (minCost == int.MaxValue)
                {
                    possibilities.Remove(foggyNode);
                    foggyNode = FogSelector.SelectFoggyNode(playerNode, possibilities.ToArray(), fogMethod,
                        CostFromNodeToNode);

                    minCost = int.MaxValue;
                    foreach (var e in foggyNode.Edges)
                        if (e != null)
                        {
                            if (GetCostFromNode(playerNode) == int.MaxValue || GetCostFromNode(e.GetOtherNode(foggyNode)) == int.MaxValue)
                                continue;

                            var c = CostFromNodeToNode(playerNode, e.GetOtherNode(foggyNode));
                            if (c > 0 && c < minCost)
                                minCost = c;
                        }
                }

                //if (minCost <= PartialCost)
                //    throw new Exception("Cost to foggy tile shouldn't be lower than current cost");
                PartialCost = minCost;

                Console.WriteLine("Best node target investigate fog is " + foggyNode);

                // add step for graphics stuff
                var min = int.MaxValue;
                AlgorithmStep oldStep = null;
                Node clearNeighborToFoggyNode = null;
                foreach (var e in foggyNode.Edges)
                    if (e != null && GetCostFromNode(e.GetOtherNode(foggyNode)) < min)
                    {
                        min = GetCostFromNode(e.GetOtherNode(foggyNode));
                        PartialCost = min;
                        oldStep = GetAlgorithmStep(playerNode, e.GetOtherNode(foggyNode), true);
                        clearNeighborToFoggyNode = e.GetOtherNode(foggyNode);
                    }

                Console.WriteLine("Setting cost of " + foggyNode + " to " + (foggyNode.Cost + min));
                AddCostToNode(foggyNode, foggyNode.Cost + min);
                Steps.Add(oldStep);

                // add currently foggy nodes to all foggy nodes
                _allFoggyNodes.AddRange(FoggyNodes);
                // explore fog
                var result = AbstractFogExploreAlgorithm.ExploreFog(fogExploreName, foggyNode, Graph,
                    _allFoggyNodes.ToArray(), AddCostToNode, GetCostFromNode, oldStep, s => Steps.Add(s));

                Console.WriteLine("Cost of path upon return: " + result.Item3);
                if (result.Item1 == null)
                {
                    // no other exit found, try other foggy node
                    Console.WriteLine("Setting cost of " + clearNeighborToFoggyNode + " to " + (result.Item3 + clearNeighborToFoggyNode.Cost));
                    Console.WriteLine("Node " + foggyNode + " didn't yield anything useful, trying next node...");
                    //AddCostToNode(clearNeighborToFoggyNode, result.Item3 + clearNeighborToFoggyNode.Cost);
                    var fogCost = result.Item3 - GetCostFromNode(clearNeighborToFoggyNode) + clearNeighborToFoggyNode.Cost;
                    foreach (var n in UpdatedNodes)
                        AddCostToNode(n, GetCostFromNode(n) + fogCost);
                    FoggyNodes.Remove(foggyNode);
                    continue;
                }

                // exit was found
                if (result.Item1.EntityOnNode == Entity.Target)
                {
                    // exit was the target, WE ARE DONE!
                    Console.WriteLine("Found path target target through fog!");
                    break;
                }
                
                // exit was another fog-less area, start pathfinding

                if (result.Item3 <= PartialCost)
                    throw new Exception("Cost through fog shouldn't be lower than current cost");
                PartialCost = result.Item3;

                var stepCount = Steps.Count;
                // start pathfinding again
                FoggyNodes.Clear();
                Console.WriteLine("Found another part of the known map, restarting pathfinding...");
                FindPath(result.Item1, targetNode, fogMethod, fogExploreName);

                for (var i = stepCount; i < Steps.Count; i++)
                    Steps[i].AddDrawing(result.Item2);
                break;
            }
        }

        /// <summary>
        /// Gets a step of the algorithm from the player to the target.
        /// </summary>
        /// <param name="player">Starting node</param>
        /// <param name="target">End node</param>
        /// <param name="withPlayer">True if the path should be traced with the player icon rather than a line</param>
        /// <returns>AlgorithmStep that shows the step</returns>
        protected abstract AlgorithmStep GetAlgorithmStep(Node player, Node target, bool withPlayer = false);

        /// <summary>
        /// Gets all alternative paths to the target.
        /// </summary>
        /// <param name="player">Start of the paths</param>
        /// <param name="target">Taget of the paths</param>
        /// <returns>AlgorithmStep that shows all alternatives</returns>
        protected abstract AlgorithmStep GetAlternativesStep(Node player, Node target);

        /// <summary>
        /// Adds a cost to a node.
        /// </summary>
        /// <param name="node">Node to be updated</param>
        /// <param name="cost">New cost of the node</param>
        protected abstract void AddCostToNode(Node node, int cost);

        /// <summary>
        /// Retrieves the cost of the path to a node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Cost of the path to the node</returns>
        protected abstract int GetCostFromNode(Node node);

        /// <summary>
        /// Retrieves the cost of the path between nodes.
        /// </summary>
        /// <param name="start">One of the nodes</param>
        /// <param name="node">The other node</param>
        /// <returns>Cost between the nodes</returns>
        protected int CostFromNodeToNode(Node start, Node node)
        {
            return GetCostFromNode(node) - GetCostFromNode(start);
        }

        /// <summary>
        /// Actual algorithm implementation: Find alternative paths.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract void FindAlternativePaths(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Find shortest path.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract bool FindShortestPath(Node playerNode, Node targetNode);

        /// <summary>
        /// Actual algorithm implementation: Prepare data for pathfinding.
        /// </summary>
        /// <param name="playerNode">Starting point</param>
        /// <param name="targetNode">End point</param>
        protected abstract void PrepareData(Node playerNode, Node targetNode);

        /// <summary>
        /// Reset the algorithm data to run again.
        /// </summary>
        public virtual void ResetAlgorithm()
        {
            Steps.Clear();
            PartialCost = 0;
            FoggyNodes.Clear();
            _allFoggyNodes.Clear();
            AbstractFogExploreAlgorithm.Reset();
        }

        #endregion
    }
}
