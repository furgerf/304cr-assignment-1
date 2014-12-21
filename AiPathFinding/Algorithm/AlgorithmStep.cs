using System;
using System.Drawing;

namespace AiPathFinding.Algorithm
{
    public class AlgorithmStep
    {
        #region Fields

        public Action<Graphics> DrawStep { get; private set; }

        public int Explored { get; private set; }

        public int Explorable { get; private set; }

        public double ExplorationPercentage { get { return Explorable == 0 ? 0 : (double)Explored/Explorable; } }

        #endregion

        #region Constructor

        public AlgorithmStep(Action<Graphics> drawStep, int explored, int explorable)
        {
            Explorable = explorable;
            Explored = explored;
            DrawStep = drawStep;
        }

        #endregion

        #region Main Methods

        

        #endregion

        #region Event Handling



        #endregion
    }
}
