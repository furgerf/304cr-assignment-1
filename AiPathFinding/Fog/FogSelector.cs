using System;
using System.Linq;
using AiPathFinding.Model;

namespace AiPathFinding.Fog
{
    public static class FogSelector
    {
        #region Main Methods

        public static Node SelectFoggyNode(Node start, Node[] foggyNodes, FogMethod method, Func<Node, Node, int> getCostFromPlayerToNode)
        {
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
