using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    public abstract class AbstractAlgorithm
    {

        public AlgorithmStep FirstStep { get; set; }

        protected readonly Graph _graph;

        protected AbstractAlgorithm(Graph graph)
        {
            _graph = graph;
        }

        public abstract void FindPath(Node from, Node to);
    }
}
