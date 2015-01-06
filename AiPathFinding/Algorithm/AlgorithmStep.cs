using System;
using System.Drawing;

namespace AiPathFinding.Algorithm
{
    /// <summary>
    /// Contains information about a step of an algorithm. This information includes the action to draw the step, data on how many nodes have been explored and how much the path costs up to now.
    /// </summary>
    public sealed class AlgorithmStep
    {
        #region Fields

        /// <summary>
        /// Action that draws the step using the Graphics provided as parameter.
        /// </summary>
        public readonly Action<Graphics> DrawStep;

        /// <summary>
        /// Number of cells that have been explored.
        /// </summary>
        public readonly int Explored;

        /// <summary>
        /// Number of cells that can be explored (all cells except the impassible ones).
        /// </summary>
        public readonly int Explorable;

        /// <summary>
        /// Returns the percentage of explored cells.
        /// </summary>
        public double ExplorationPercentage { get { return Explorable == 0 ? 0 : (double)Explored/Explorable; } }

        /// <summary>
        /// Contains the cost of the currently travelled path.
        /// </summary>
        public readonly int CurrentCost;

        /// <summary>
        /// Comment about what happens in the step.
        /// </summary>
        public readonly string Comment;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that creates a new step.
        /// </summary>
        /// <param name="drawStep">Action that draws the step</param>
        /// <param name="explored">Number of explored cells</param>
        /// <param name="explorable">Number of explorable cells</param>
        /// <param name="currentCost">Cost of the travelled path until now</param>
        /// <param name="comment">Comment of the step</param>
        public AlgorithmStep(Action<Graphics> drawStep, int explored, int explorable, int currentCost, string comment)
        {
            Comment = comment;
            CurrentCost = currentCost;
            Explorable = explorable;
            Explored = explored;
            DrawStep = drawStep;
        }

        #endregion
    }
}
