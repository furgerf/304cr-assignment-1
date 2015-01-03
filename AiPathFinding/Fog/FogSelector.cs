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
        /// <param name="target">Where the target is</param>
        /// <param name="foggyNodes">Foggy candidates</param>
        /// <param name="method">Name of the method to chose foggy node</param>
        /// <param name="getCostFromPlayerToNode">Function that gets the cost from the player to a node</param>
        /// <returns>Best foggy candidate</returns>
        public static Node SelectFoggyNode(Node start, Node target, Node[] foggyNodes, FogMethod method, Func<Node, Node, int> getCostFromPlayerToNode)
        {
            if (foggyNodes.Length == 0)
                throw new ArgumentException("cannot chose among zero candidates");

            switch (method)
            {
                case FogMethod.ClosestToPlayer:
                    return foggyNodes.First(n => getCostFromPlayerToNode(start, n) == 
                        foggyNodes.Min(nn => getCostFromPlayerToNode(start, nn)));
                case FogMethod.ClosestToTarget:
                    return foggyNodes.First(n => Math.Abs(target.Location.X - n.Location.X) + Math.Abs(target.Location.Y - n.Location.Y) == 
                        foggyNodes.Min(nn => Math.Abs(target.Location.X - nn.Location.X) + Math.Abs(target.Location.Y - nn.Location.Y)));
                case FogMethod.MinClosestToPlayerPlusTarget:
                    var validNodes = foggyNodes.Where(n => getCostFromPlayerToNode(start, n) < int.MaxValue).ToArray();
                    return validNodes.First(n => getCostFromPlayerToNode(start, n) + Math.Abs(target.Location.X - n.Location.X) + Math.Abs(target.Location.Y - n.Location.Y) ==
                        validNodes.Min(nn => getCostFromPlayerToNode(start, nn) + Math.Abs(target.Location.X - nn.Location.X) + Math.Abs(target.Location.Y - nn.Location.Y)));
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }

        #endregion
    }
}
