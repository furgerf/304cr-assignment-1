using System;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class AlgorithmSettings : UserControl
    {
        #region Fields

        private AbstractAlgorithm[] _abstractAlgorithms;

        // ensures that progress and label text are updated along with the step index
        private int StepIndex
        {
            get { return _stepIndex; }
            set
            {
                if (StepIndex == value)
                    return;

                _stepIndex = value;

                progressSteps.Value = StepIndex;
                labStep.Text = "Step " + (StepIndex + 1) + "/" + _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count;
            }
        }

        private int _stepIndex;

        #endregion

        #region Events

        public delegate void OnAlgorithmStepChanged(AlgorithmStep step);

        public event OnAlgorithmStepChanged AlgorithmStepChanged;

        #endregion

        #region Constructor

        public AlgorithmSettings()
        {
            InitializeComponent();

            // register to event to make sure secondary algorithm can only be selected if necessary
            comPrimaryAlgorithm.SelectedIndexChanged +=
                (s, e) =>
                    grpSecondaryAlgorithm.Enabled =
                        Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility,
                            (AlgorithmNames)comPrimaryAlgorithm.SelectedIndex) > -1;

            // add ALL algorithms to primary algorithm combobox
            for (var i = 0; i < (int) AlgorithmNames.Count; i++)
                comPrimaryAlgorithm.Items.Add((AlgorithmNames) i);
            comPrimaryAlgorithm.SelectedIndex = 0;

            // add only algorithms to secondary algo combo that work without visibility
            for (var i = 0; i < (int)AlgorithmNames.Count; i++)
                if (Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility, (AlgorithmNames)i) == -1)
                    comSecondaryAlgorithm.Items.Add((AlgorithmNames)i);
            comSecondaryAlgorithm.SelectedIndex = 0;
        }

        #endregion

        #region Methods

        public void RegisterMap(Map map)
        {
            // required to create algorithms since they need the graph
            _abstractAlgorithms = new AbstractAlgorithm[] { new DijkstraAbstractAlgorithm(map.GetGraph()), new AStarAbstractAlgorithm(map.GetGraph()) };

            // also register to event so the algorithm can only be started if both entities are in play
            map.EntityNodeChanged += (o, n, e) => SetStartButtonEnabled();

            SetStartButtonEnabled();
        }

        private void SetStartButtonEnabled()
        {
            butStart.Enabled = Entity.Player.Node != null && Entity.Target.Node != null;
        }

        private void SetStepButtonsEnabled()
        {
            butFirst.Enabled = StepIndex != 0;
            butPrevious.Enabled = StepIndex != 0;
            butNext.Enabled = StepIndex != _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;
            butLast.Enabled = StepIndex != _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;
        }

        #endregion

        #region Event Handling

        private void butRestart_Click(object sender, EventArgs e)
        {
            butClear_Click();
            butStart_Click();
        }

        private void butStart_Click(object sender = null, EventArgs e = null)
        {
            // set controls enabled/disabled
            grpPlayback.Enabled = true;
            butStart.Enabled = false;
            butRestart.Enabled = true;
            butClear.Enabled = true;
            grpPrimaryAlgorithm.Enabled = false;
            grpSecondaryAlgorithm.Enabled = false;

            // starts calculation
            _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);

            // set control values
            progressSteps.Maximum = _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;
            labStep.Text = "Step " + (StepIndex + 1) + "/" + _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count;

            // auto-load last step
            butLast_Click();
        }

        private void butClear_Click(object sender = null, EventArgs e = null)
        {
            // reset algorithm
            _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Reset();

            // reset control enabled/disabled
            grpPlayback.Enabled = false;
            butStart.Enabled = true;
            butRestart.Enabled = false;
            butClear.Enabled = false;
            grpPrimaryAlgorithm.Enabled = true;
            grpSecondaryAlgorithm.Enabled = Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility, (AlgorithmNames) comPrimaryAlgorithm.SelectedIndex) > -1;
            labStep.Text = "(No steps to show)";
            progressSteps.Value = 0;

            // clear the algorithmstep from the map
            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(null);
        }

        private void butFirst_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butPrevious_Click(object sender = null, EventArgs e = null)
        {
            StepIndex--;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butNext_Click(object sender = null, EventArgs e = null)
        {
            StepIndex++;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butLast_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        #endregion
    }
}
