using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class AlgorithmSettings : UserControl
    {
        private AbstractAlgorithm[] _abstractAlgorithms;

        public AlgorithmSettings()
        {
            InitializeComponent();

            for (var i = 0; i < (int) Algorithm.AlgorithmNames.Count; i++)
                comKnownAreaAlgorithm.Items.Add((Algorithm.AlgorithmNames) i);
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

        private void butStart_Click(object sender, System.EventArgs e)
        {
            _abstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);
        }
    }
}
