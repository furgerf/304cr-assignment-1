using System;
using AiPathFinding.Model;

namespace AiPathFinding.Algorithm
{
    public class AStarAbstractAlgorithm : AbstractAlgorithm
    {
        #region Public Fields



        #endregion

        #region Private Fields



        #endregion

        #region Constructor

        public AStarAbstractAlgorithm(Graph graph)
            : base(graph)
        {
        }

        #endregion

        #region Main Methods

        override public void FindPath(Node from, Node to)
        {
            throw new NotImplementedException();
        }

        protected override void ResetAlgorithm()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
