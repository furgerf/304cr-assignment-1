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
        private int _stepIndex;

        private int StepIndex
        {
            get { return _stepIndex; }
            set
            {
                if (StepIndex == value)
                    return;

                _stepIndex = value;

                progressSteps.Value = StepIndex;
                labStep.Text = "Step " + (StepIndex + 1) + "/" + _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count;
            }
        }

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

            map.EntityNodeChanged += (o, n, e) => SetStartButtonEnabled();
            SetStartButtonEnabled();
        }

        private void SetStartButtonEnabled()
        {
            butStart.Enabled = Entity.Player.Node != null && Entity.Target.Node != null;
        }

        private void butStart_Click(object sender, EventArgs e)
        {
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);

            grpPlayback.Enabled = true;
            butStart.Enabled = false;
            grpKnownAreaAlgorithm.Enabled = false;

            progressSteps.Maximum = _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;
            labStep.Text = "Step " + (StepIndex + 1) + "/" + _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count;

            butLast_Click(null, null);
        }

        private void butFirst_Click(object sender, EventArgs e)
        {
            StepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butPrevious_Click(object sender, EventArgs e)
        {
            StepIndex--;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            StepIndex++;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butLast_Click(object sender, EventArgs e)
        {
            StepIndex = _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void SetStepButtonsEnabled()
        {
            butFirst.Enabled = StepIndex != 0;
            butPrevious.Enabled = StepIndex != 0;
            butNext.Enabled = StepIndex != _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;
            butLast.Enabled = StepIndex != _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].Steps.Count - 1;
        }

    }
}
