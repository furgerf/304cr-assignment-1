using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class AlgorithmSettings : UserControl
    {
        private readonly static AbstractAlgorithm[] AbstractAlgorithms = {new AStarAbstractAlgorithm(), new DijkstraAbstractAlgorithm()};

        public AlgorithmSettings()
        {
            InitializeComponent();

            for (var i = 0; i < (int) Algorithm.Algorithm.Count; i++)
                comKnownAreaAlgorithm.Items.Add((Algorithm.Algorithm) i);
            comKnownAreaAlgorithm.SelectedIndex = 0;
        }

        public void RegisterMap(Map map)
        {
            map.EntityNodeChanged += (o, n, e) => SetButtonEnabled();
            SetButtonEnabled();
        }

        private void SetButtonEnabled()
        {
            butStart.Enabled = Entity.Player.Node != null && Entity.Target.Node != null;
        }

        private void butStart_Click(object sender, System.EventArgs e)
        {
            AbstractAlgorithms[comKnownAreaAlgorithm.SelectedIndex].FindPath(Entity.Player.Node, Entity.Target.Node);
        }
    }
}
