using System;
using System.Drawing;

namespace AiPathFinding.Algorithm
{
    public class AlgorithmStep
    {
        #region Fields

        public AlgorithmStep NextStep { get; set; }

        public AlgorithmStep PreviousStep { get; set; }

        public Action<Graphics> DrawStep { get; private set; }

        #endregion

        #region Constructor

        public AlgorithmStep(Action<Graphics> drawStep)
        {
            DrawStep = drawStep;
        }

        #endregion

        #region Main Methods



        #endregion

        #region Event Handling



        #endregion
    }
}
