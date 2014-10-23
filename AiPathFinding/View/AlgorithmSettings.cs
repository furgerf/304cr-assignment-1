using System;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class AlgorithmSettings : UserControl
    {
        public delegate void OnAlgorithmStepChanged(AlgorithmStep step);

        public event OnAlgorithmStepChanged AlgorithmStepChanged;

        private AbstractAlgorithm[] _abstractAlgorithms;

        public AlgorithmSettings()
        {
            InitializeComponent();

            for (var i = 0; i < (int) AlgorithmNames.Count; i++)
                comKnownAreaAlgorithm.Items.Add((AlgorithmNames) i);
            comKnownAreaAlgorithm.SelectedIndex = 0;
        }

        public void RegisterMap(Map map)
        {
            _abstractAlgorithms = new AbstractAlgorithm[] { new DijkstraAbstractAlgorithm(map.GetGraph()), new AStarAbstractAlgorithm(map.GetGraph()) };

            map.EntityNodeChanged += (o, n, e) => SetButtonEnabled();
            SetButtonEnabled();
        }

        private void SetButtonEnabled()
        {
            butStart.Enabled = Entity.Player.Node != null && Entity.Target.Node != null;
        }

        private void butStart_Click(object sender, EventArgs e)
        {
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);

            grpPlayback.Enabled = true;
            butStart.Enabled = false;
            grpKnownAreaAlgorithm.Enabled = false;

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FirstStep);
        }

        private void butFirst_Click(object sender, EventArgs e)
        {
            if (_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep ==
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FirstStep)
                return;

            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep =
                       _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FirstStep;

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep);
        }

        private void butPrevious_Click(object sender, EventArgs e)
        {
            if (_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep ==
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.PreviousStep ||
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.PreviousStep == null)
                return;
            
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep =
                       _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.PreviousStep;

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep);
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            if (_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep ==
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.NextStep ||
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.NextStep == null)
                return;
            
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep =
                       _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep.NextStep;

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep);
        }

        private void butLast_Click(object sender, EventArgs e)
        {
            if (_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep ==
                _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].LastStep)
                return;
            
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep =
                       _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].LastStep;

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].CurrentStep);
        }
    }
}
