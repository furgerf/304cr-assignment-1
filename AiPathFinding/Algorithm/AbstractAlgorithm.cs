using System;
using System.Collections.Generic;
using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Abstract parent class for all concrete algorithms.
    /// </summary>
    public abstract class AbstractAlgorithm
    {
        #region Fields

        /// <summary>
        /// Constant array of all algorithms that do not work in fog.
        /// </summary>
        public static readonly AlgorithmNames[] AlgorithmsRequiringVisibility = {AlgorithmNames.AStar, AlgorithmNames.BestFirstSearch};

        private static Graph _graph;

        /// <summary>
        /// List of all steps that the current algorithm calculated.
        /// </summary>
        public readonly List<AlgorithmStep> Steps = new List<AlgorithmStep>();

        /// <summary>
        /// Graph instance for all algorithms
        /// </summary>
        public static Graph Graph
        {
            get { return _graph; }
            set { _graph = value; Console.WriteLine("New graph set for algorithms."); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the algorithm to find the path on the graph.
        /// </summary>
        /// <param name="from">Node to start from</param>
        /// <param name="to">Node which is attempted to be reached</param>
        public abstract void FindPath(Node from, Node to);

        /// <summary>
        /// Resets the algorithm so it can be re-run. Implementation depends on the particular algorithm.
        /// </summary>
        public abstract void ResetAlgorithm();

        #endregion
    }
}
