using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Implementation of the Dijkstra-Algorithm. Since it's basically a slightly simpler A*-Algorithm, override it.
    /// </summary>
    public sealed class DijkstraAlgorithm : AStarAlgorithm
    {
        #region Constructor

        public DijkstraAlgorithm()
            : base(PathFindName.Dijkstra)
        {
        }

        #endregion

        #region Main Methods

        protected override int GetHeuristic(Node node, Node target)
        {
            // zero
            return 0;
        }

        #endregion
    }
}
