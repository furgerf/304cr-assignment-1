using AiPathFinding.Common;
using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Implementation of the Dijkstra-Algorithm. Since it's basically a slightly simpler A*-Algorithm, override it.
    /// </summary>
    public sealed class DijkstraAlgorithm : AStarAlgorithm
    {
        #region Constructor

        /// <summary>
        /// Creates new instance of the Dijkstra algorithm.
        /// </summary>
        public DijkstraAlgorithm()
            : base(PathFindName.Dijkstra)
        {
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Returns heuristic distance between nodes. Uses zero as the heuristic value.
        /// </summary>
        /// <param name="node1">One node</param>
        /// <param name="node2">Other node</param>
        /// <returns>Distance</returns>
        protected override int GetHeuristic(Node node1, Node node2)
        {
            // zero
            return 0;
        }

        #endregion
    }
}
