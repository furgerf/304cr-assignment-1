using System;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    /// <summary>
    /// Selects foggy node.
    /// </summary>
    public static class FogSelector
    {
        #region Main Methods

        /// <summary>
        /// Selects a foggy node among candidates.
        /// </summary>
        /// <param name="start">Where the pathfinding started</param>
        /// <param name="foggyNodes">Foggy candidates</param>
        /// <param name="method">Name of the method to chose foggy node</param>
        /// <param name="getCostFromPlayerToNode">Function that gets the cost from the player to a node</param>
        /// <returns>Best foggy candidate</returns>
        public static Node SelectFoggyNode(Node start, Node[] foggyNodes, FogMethod method, Func<Node, Node, int> getCostFromPlayerToNode)
        {
            if (foggyNodes.Length == 0)
                throw new ArgumentException("cannot chose among zero candidates");

            switch (method)
            {
                case FogMethod.MinCost:
                    return foggyNodes.First(n => getCostFromPlayerToNode(start, n) == foggyNodes.Min(nn => getCostFromPlayerToNode(start, nn)));
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }

        #endregion
    }
}
