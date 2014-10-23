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

        private int _stepIndex = 0;

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
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[_stepIndex]);
        }

        private void butFirst_Click(object sender, EventArgs e)
        {
            _stepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[_stepIndex]);
        }

        private void butPrevious_Click(object sender, EventArgs e)
        {
            _stepIndex--;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[_stepIndex]);
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            _stepIndex++;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[_stepIndex]);
        }

        private void butLast_Click(object sender, EventArgs e)
        {
            _stepIndex = _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[_stepIndex]);
        }

        private void SetStepButtonsEnabled()
        {
            butFirst.Enabled = _stepIndex != 0;
            butPrevious.Enabled = _stepIndex != 0;
            butNext.Enabled = _stepIndex != _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;
            butLast.Enabled = _stepIndex != _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;
        }

    }
}
