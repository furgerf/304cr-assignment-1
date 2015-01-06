using System;
using System.Linq;
using AiPathFinding.Common;
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
        /// <param name="getCostToNode">Function that gets the cost from the player to a node</param>
        /// <returns>Best foggy candidate</returns>
        public static Node SelectFoggyNode(Node start, Node target, Node[] foggyNodes, FogMethod method, Func<Node, int> getCostToNode)
        {
            if (foggyNodes.Length == 0)
                throw new ArgumentException("cannot chose among zero candidates");

            var rnd = new Random();
            Node[] possibilities;

            switch (method)
            {
                case FogMethod.ClosestToPlayer:
                    // chose based on cost of the node
                    possibilities = foggyNodes.Where(n => getCostToNode(n) == foggyNodes.Min(getCostToNode)).ToArray();
                    return possibilities[rnd.Next(possibilities.Length)];
                case FogMethod.ClosestToTarget:
                    // chose based on manhatten distance to target
                    possibilities = foggyNodes.Where(n => Math.Abs(target.Location.X - n.Location.X) + Math.Abs(target.Location.Y - n.Location.Y) == foggyNodes.Min(nn => Math.Abs(target.Location.X - nn.Location.X) + Math.Abs(target.Location.Y - nn.Location.Y))).ToArray();
                    return possibilities[rnd.Next(possibilities.Length)];
                case FogMethod.MinClosestToPlayerPlusTarget:
                    // only consider nodes that are passible
                    // this has to be done here (and not when looking at closesttoplayer)
                    // because here the cost gets added to the distance which could lead to arithmetic overflow
                    var validNodes = foggyNodes.Where(n => getCostToNode(n) < int.MaxValue).ToArray();
                    if (validNodes.Length == 0)
                        throw new Exception("cannot chose among impassible candidates");

                    // chose based on the sum of the two above
                    possibilities = validNodes.Where(n => getCostToNode(n) + Math.Abs(target.Location.X - n.Location.X) + Math.Abs(target.Location.Y - n.Location.Y) == validNodes.Min(nn => getCostToNode(nn) + Math.Abs(target.Location.X - nn.Location.X) + Math.Abs(target.Location.Y - nn.Location.Y))).ToArray();
                    return possibilities[rnd.Next(possibilities.Length)];
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }

        #endregion
    }
}
