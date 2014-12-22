using System;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;
using AiPathFinding.Properties;

namespace AiPathFinding.View
{
    /// <summary>
    /// Control that provides an interface to change settings related to the algorithms.
    /// </summary>
    public sealed partial class AlgorithmSettings : UserControl
    {
        #region Fields

        /// <summary>
        /// All available algorithms
        /// </summary>
        private AbstractAlgorithm[] _algorithms;

        /// <summary>
        /// Index of the currently displayed step in the array of calculated steps from the algorithm.
        /// </summary>
        private int StepIndex
        {
            get { return _stepIndex; }
            set
            {
                _stepIndex = value;

                // update progress bar
                if (StepIndex >= 0)
                    progressSteps.Value = StepIndex;

                // update text labels
                labStep.Text = "Step " + (StepIndex + 1) + "/" + _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count;
                labExplored.Text = "Visited " + _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explored + " of " + _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explorable + " passible cells (" + Math.Round(100 * _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]
                                       .ExplorationPercentage, 2) + "%)";
            }
        }

        /// <summary>
        /// StepIndex backing field.
        /// </summary>
        private int _stepIndex;

        #endregion

        #region Events

        /// <summary>
        /// Triggered whenever the current algorithm step changes
        /// </summary>
        /// <param name="step">Current step</param>
        public delegate void OnAlgorithmStepChanged(AlgorithmStep step);

        public event OnAlgorithmStepChanged AlgorithmStepChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
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

        /// <summary>
        /// Registers a new graph and makes sure that the algorithms use the most recent graph.
        /// </summary>
        /// <param name="graph">New graph</param>
        public void RegisterGraph(Graph graph)
        {
            // clean up old data
            butClear_Click();

            // instantiate algorithms if necessary
            if (_algorithms == null)
                _algorithms = new AbstractAlgorithm[] { new DijkstraAlgorithm(), new AStarAlgorithm() };

            // tell algorithms about new graph
            AbstractAlgorithm.Graph = graph;

            // also register to event so the algorithm can only be started if both entities are in play
            Entity.NodeChanged += (o, n, e) => SetStartButtonEnabled();

            SetStartButtonEnabled();
        }

        /// <summary>
        /// Enables the start button if all entities are set.
        /// </summary>
        private void SetStartButtonEnabled()
        {
            butStart.Enabled = Entity.Player.Node != null && Entity.Target.Node != null;
        }

        /// <summary>
        /// Enables the step buttons if algorithm steps are available to be displayed.
        /// </summary>
        private void SetStepButtonsEnabled()
        {
            butFirst.Enabled = StepIndex != 0;
            butPrevious.Enabled = StepIndex != 0;
            butNext.Enabled = StepIndex != _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;
            butLast.Enabled = StepIndex != _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Clears the previous data and starts a new pathfinding.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butRestart_Click(object sender = null, EventArgs e = null)
        {
            // reset algorithm
            _algorithms[comPrimaryAlgorithm.SelectedIndex].ResetAlgorithm();

            butStart_Click();
        }

        /// <summary>
        /// Starts a new pathfinding.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
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
            _algorithms[comPrimaryAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);

            // set progress bar stuff
            // set value to 0 to avoid outofrangeexception
            progressSteps.Value = 0;
            progressSteps.Maximum = _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count == 0 ? 0 : _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;

            // auto-load last step
            butLast_Click();
        }

        /// <summary>
        /// Clears data from the last pathfinding
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butClear_Click(object sender = null, EventArgs e = null)
        {
            // reset algorithm
            if (_algorithms != null)
                _algorithms[comPrimaryAlgorithm.SelectedIndex].ResetAlgorithm();

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

        /// <summary>
        /// Moves to the first algorithm step.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butFirst_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the previous algorithm step.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butPrevious_Click(object sender = null, EventArgs e = null)
        {
            StepIndex--;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the next algorithm step.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butNext_Click(object sender = null, EventArgs e = null)
        {
            StepIndex++;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the last algorithm step.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butLast_Click(object sender = null, EventArgs e = null)
        {
            StepIndex = _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _algorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Count > 0)
                AlgorithmStepChanged(_algorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comPrimaryAlgorithm_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.PrimaryAlgorithm = comPrimaryAlgorithm.SelectedIndex;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comSecondaryAlgorithm_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.SecondaryAlgorithm = comSecondaryAlgorithm.SelectedIndex;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comFogMethod_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.FogMethod = comFogMethod.SelectedIndex;
            Settings.Default.Save();
        }

        #endregion
    }
}
