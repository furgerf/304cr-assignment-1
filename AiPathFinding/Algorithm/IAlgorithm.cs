using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    interface IAlgorithm
    {
        void FindPath(Node from, Node to);
    }
}
