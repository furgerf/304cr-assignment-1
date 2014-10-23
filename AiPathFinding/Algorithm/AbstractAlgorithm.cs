using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    public abstract class AbstractAlgorithm
    {
        
        #region Events

        #endregion

        public AlgorithmStep FirstStep { get; set; }

        public AlgorithmStep CurrentStep { get; set; }

        public AlgorithmStep LastStep { get; set; }

        protected readonly Graph Graph;

        protected AbstractAlgorithm(Graph graph)
        {
            Graph = graph;
        }

        public abstract void FindPath(Node from, Node to);

    }
}
