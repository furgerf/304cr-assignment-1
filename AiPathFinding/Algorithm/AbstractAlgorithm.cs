using System.Collections.Generic;
using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    public abstract class AbstractAlgorithm
    {
        
        #region Events

        #endregion

        public readonly List<AlgorithmStep> Steps = new List<AlgorithmStep>();

        protected readonly Graph Graph;

        protected AbstractAlgorithm(Graph graph)
        {
            Graph = graph;
        }

        public abstract void FindPath(Node from, Node to);

    }
}
