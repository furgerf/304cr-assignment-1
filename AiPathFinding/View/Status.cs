using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class Status : UserControl
    {
        private Point? _playerPosition;
        private Point? _targetPosition;

        private readonly TextBox[] _terrainBoxes;

        #region Fields

        public Point? PlayerPosition
        {
            get { return _playerPosition; }
            set
            {
                _playerPosition = value;

                txtPlayerPosition.Text = _playerPosition == null ? "N/A" : _playerPosition.Value.ToString();

                CalculateDistances();
            }
        }


        public Point? TargetPosition
        {
            get { return _targetPosition; }
            set
            {
                _targetPosition = value;

                txtTargetPosition.Text = _targetPosition == null ? "N/A" : _targetPosition.Value.ToString();

                CalculateDistances();
            }
        }

        #endregion

        #region Constructor

        public Status()
        {
            InitializeComponent();

            if ((int) EntityType.Count != 2)
                throw new ArgumentException();

            _terrainBoxes = new[] {txtStreet, txtPlains, txtForest, txtHill, txtMountain};
        }

        #endregion

        #region Methods

        private void CalculateDistances()
        {
            if (PlayerPosition == null || TargetPosition == null)
            {
                txtMinimalDistance.Text = "N/A";
                txtManhattenDistance.Text = "N/A";
                return;
            }

            txtMinimalDistance.Text =
                Math.Round(Math.Sqrt(Math.Pow(TargetPosition.Value.X - PlayerPosition.Value.X, 2) +
                          Math.Pow(TargetPosition.Value.Y - PlayerPosition.Value.Y, 2)), 3).ToString();
            var dist = Math.Abs(TargetPosition.Value.X - PlayerPosition.Value.X) +
                       Math.Abs(TargetPosition.Value.Y - PlayerPosition.Value.Y);
            txtManhattenDistance.Text = dist.ToString();
        }

        public void UpdateMapStatistics(Map map, Terrain[] terrains = null, bool allTerrain = false, bool fog = false)
        {
            var cellCount = map.Width*map.Height;

            // terrain
            if (allTerrain)
                terrains = new[] {Terrain.Street, Terrain.Plains, Terrain.Forest, Terrain.Hill, Terrain.Mountain};

            if (terrains != null)
                foreach (var t in terrains)
                {
                    var count = map.GetGraph().Nodes.Sum(n => n == null ? 0 : n.Count(nn => nn != null && nn.Terrain == t));
                    _terrainBoxes[(int) t].Text = count + "/" + cellCount + " (" +
                                                  Math.Round((double) 100*count/cellCount, 1) + "%)";
                }

            // fog
            if (!fog)
                return;

            var foggy = map.GetGraph().Nodes.Sum(n => n.Count(nn => !nn.KnownToPlayer));
            txtFog.Text = foggy + "/" + cellCount + " (" + Math.Round((double) 100*foggy/cellCount, 1) + "%)";
        }

        #endregion
    }
}
