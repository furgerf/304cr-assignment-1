using System;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Fog;
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
        private AbstractPathFindAlgorithm[] _pathFindAlgorithms;

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
                labStep.Text = "Step " + (StepIndex + 1) + "/" + _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length +
                    "               Cost: " + _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].CurrentCost;
                labExplored.Text = "Visited " + _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explored + " of " + _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Explorable + " passible cells (" + Math.Round(100 * _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]
                                       .ExplorationPercentage, 2) + "%)";
                labComment.Text = _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex].Comment ?? "*no comment*";
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
            //comPrimaryAlgorithm.SelectedIndexChanged +=
            //    (s, e) =>
            //    {
            //        grpSecondaryAlgorithm.Enabled =
            //            Array.IndexOf(AbstractPathFindAlgorithm.AlgorithmsRequiringVisibility,
            //                (PathFindName) comPrimaryAlgorithm.SelectedIndex) > -1;
            //        grpFogSelection.Enabled = grpSecondaryAlgorithm.Enabled;
            //    };

            // add ALL algorithms to primary algorithm combobox
            for (var i = 0; i < (int) PathFindName.Count; i++)
                comPrimaryAlgorithm.Items.Add((PathFindName) i);
            comPrimaryAlgorithm.SelectedIndex = Settings.Default.PrimaryAlgorithm;

            // add only algorithms to secondary algo combo that work without visibility
            for (var i = 0; i < (int)FogExploreName.Count; i++)
                comSecondaryAlgorithm.Items.Add((FogExploreName)i);
            comSecondaryAlgorithm.SelectedIndex = Settings.Default.SecondaryAlgorithm;

            // add ALL fog methods to fog method combobox
            for (var i = 0; i < (int)FogMethod.Count; i++)
                comFogMethod.Items.Add((FogMethod)i);
            comFogMethod.SelectedIndex = Settings.Default.FogMethod;

            Console.WriteLine("AlgorithmSettings created");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers a new graph and makes sure that the algorithms use the most recent graph.
        /// </summary>
        /// <param name="graph">New graph</param>
        public void RegisterGraph(Graph graph)
        {
            Console.WriteLine("Registering new graph for algorithms");

            // clean up old data
            butClear_Click();

            // instantiate algorithms if necessary
            if (_pathFindAlgorithms == null)
                _pathFindAlgorithms = new AbstractPathFindAlgorithm[] { new DijkstraAlgorithm(), new AStarAlgorithm() };
            // TODO
            //_pathFindAlgorithms = new AbstractPathFindAlgorithm[] { new DijkstraAlgorithm(), new AStarAlgorithm() };

            // tell algorithms about new graph
            AbstractPathFindAlgorithm.Graph = graph;

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
            butNext.Enabled = StepIndex != _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1;
            butLast.Enabled = StepIndex != _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1;
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
            _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].ResetAlgorithm();

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
            grpFogSelection.Enabled = false;

            // start calculation
            _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node, (FogMethod)comFogMethod.SelectedIndex, (FogExploreName)comSecondaryAlgorithm.SelectedIndex);

            // set progress bar stuff
            // set value to 0 to avoid outofrangeexception
            progressSteps.Value = 0;
            progressSteps.Maximum = _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length == 0 ? 0 : _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1;

            // auto-load last step
            butLast.Enabled = true;
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
            if (_pathFindAlgorithms != null)
                _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].ResetAlgorithm();

            // reset control enabled/disabled
            grpPlayback.Enabled = false;
            butStart.Enabled = true;
            butRestart.Enabled = false;
            butClear.Enabled = false;
            grpPrimaryAlgorithm.Enabled = true;
            grpSecondaryAlgorithm.Enabled = true;
            grpFogSelection.Enabled = true;

            labStep.Text = "(No steps to show)";
            labExplored.Text = "(No exploration yet)";
            labComment.Text = "(No comment)";
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
            if (!butPrevious.Enabled)
                return;

            StepIndex = 0;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length > 0)
                AlgorithmStepChanged(_pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the previous algorithm step.
        /// </summary>
        /// <param name="countObject">number of steps to advance</param>
        /// <param name="e">unused</param>
        public void butPrevious_Click(object countObject = null, EventArgs e = null)
        {
            if (!butPrevious.Enabled)
                return;

            var count = !(countObject is int) ? 1 : (int)countObject;

            if (StepIndex - count < 0)
                StepIndex = 0;
            else
                StepIndex -= count;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length> 0)
                AlgorithmStepChanged(_pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the next algorithm step.
        /// </summary>
        /// <param name="countObject">number of steps to advance</param>
        /// <param name="e">unused</param>
        public void butNext_Click(object countObject = null, EventArgs e = null)
        {
            if (!butNext.Enabled)
                return;

            var count = !(countObject is int) ? 1 : (int)countObject;

            if (StepIndex + count > _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1)
                StepIndex = _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1;
            else
                StepIndex += count;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length > 0)
                AlgorithmStepChanged(_pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
        }

        /// <summary>
        /// Moves to the last algorithm step.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butLast_Click(object sender = null, EventArgs e = null)
        {
            if (!butLast.Enabled)
                return;

            StepIndex = _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length - 1;

            SetStepButtonsEnabled();

            if (AlgorithmStepChanged != null && _pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps.Length > 0)
                AlgorithmStepChanged(_pathFindAlgorithms[comPrimaryAlgorithm.SelectedIndex].Steps[StepIndex]);
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
