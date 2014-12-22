using System;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;
using AiPathFinding.Properties;

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
                _stepIndex = value;

                if (StepIndex >= 0)
                    progressSteps.Value = StepIndex;

                labStep.Text = "Step " + (StepIndex + 1) + "/" + _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count;
                labExplored.Text = "Visited " + _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explored + " of " + _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explorable + " passible cells (" + Math.Round(100 * _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]
                                       .ExplorationPercentage, 2) + "%)";
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
                {
                    grpSecondaryAlgorithm.Enabled =
                        Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility,
                            (AlgorithmNames) comPrimaryAlgorithm.SelectedIndex) > -1;
                    grpFogSelection.Enabled = grpSecondaryAlgorithm.Enabled;
                };

            // add ALL algorithms to primary algorithm combobox
            for (var i = 0; i < (int) AlgorithmNames.Count; i++)
                comPrimaryAlgorithm.Items.Add((AlgorithmNames) i);
            comPrimaryAlgorithm.SelectedIndex = Settings.Default.PrimaryAlgorithm;

            // add only algorithms to secondary algo combo that work without visibility
            for (var i = 0; i < (int)AlgorithmNames.Count; i++)
                if (Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility, (AlgorithmNames)i) == -1)
                    comSecondaryAlgorithm.Items.Add((AlgorithmNames)i);
            comSecondaryAlgorithm.SelectedIndex = Settings.Default.SecondaryAlgorithm;

            // TODO: ADD FOG METHOD STUFF
        }

        #endregion

        #region Methods

        public void RegisterMap(Map map)
        {
            // clean up old data
            butClear_Click();

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
            // reset algorithm
            _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Reset();

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

            // set progress bar stuff
            // set value to 0 to avoid outofrangeexception
            progressSteps.Value = 0;
            progressSteps.Maximum = _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count == 0 ? 0 : _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;

            // auto-load last step
            butLast_Click();
        }

        private void butClear_Click(object sender = null, EventArgs e = null)
        {
            // reset algorithm
            if (_abstractAlgorithms != null)
                _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Reset();

            // reset control enabled/disabled
            grpPlayback.Enabled = false;
            butStart.Enabled = true;
            butRestart.Enabled = false;
            butClear.Enabled = false;
            grpPrimaryAlgorithm.Enabled = true;
            grpSecondaryAlgorithm.Enabled = Array.IndexOf(AbstractAlgorithm.AlgorithmsRequiringVisibility, (AlgorithmNames) comPrimaryAlgorithm.SelectedIndex) > -1;

            labStep.Text = "(No steps to show)";
            labExplored.Text = "(No exploration yet)";
            progressSteps.Value = 0;

            // clear the algorithmstep from the map
            if (AlgorithmStepChanged != null)
                AlgorithmStepChanged(null);
        }

        private void butFirst_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butPrevious_Click(object sender = null, EventArgs e = null)
        {
            StepIndex--;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butNext_Click(object sender = null, EventArgs e = null)
        {
            StepIndex++;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void butLast_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_abstractAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        private void comPrimaryAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.PrimaryAlgorithm = comPrimaryAlgorithm.SelectedIndex;
            Settings.Default.Save();
        }

        private void comSecondaryAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.SecondaryAlgorithm = comSecondaryAlgorithm.SelectedIndex;
            Settings.Default.Save();
        }

        private void comFogMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.FogMethod = comFogMethod.SelectedIndex;
            Settings.Default.Save();
        }

        #endregion
    }
}
