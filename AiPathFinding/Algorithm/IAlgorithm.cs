using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    public abstract class AbstractAlgorithm
    {

        public AlgorithmStep FirstStep { get; set; }

        public abstract void FindPath(Node from, Node to);
    }
}
