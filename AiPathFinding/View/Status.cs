using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    /// <summary>
    /// Displays data about the entities and the map.
    /// </summary>
    public sealed partial class Status : UserControl
    {
        #region Fields

        /// <summary>
        /// Contains all terrain text boxes (for easier access).
        /// </summary>
        private readonly TextBox[] _terrainBoxes;

        /// <summary>
        /// The player's position.
        /// </summary>
        private Point? PlayerPosition
        {
            get { return _playerPosition; }
            set
            {
                if (PlayerPosition == value)
                    return;

                _playerPosition = value;

                txtPlayerPosition.Text = _playerPosition == null ? "N/A" : _playerPosition.Value.ToString();

                CalculateDistances();
            }
        }

        /// <summary>
        /// PlayerPosition backing field.
        /// </summary>
        private Point? _playerPosition;

        /// <summary>
        /// The target's position.
        /// </summary>
        private Point? TargetPosition
        {
            get { return _targetPosition; }
            set
            {
                if (TargetPosition == value)
                    return;

                _targetPosition = value;

                txtTargetPosition.Text = _targetPosition == null ? "N/A" : _targetPosition.Value.ToString();

                CalculateDistances();
            }
        }

        /// <summary>
        /// TargetPosition backing field.
        /// </summary>
        private Point? _targetPosition;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Status()
        {
            InitializeComponent();

            // throw error in case the entities get extended
            // and the status control doesn't get updated
            if ((int) EntityType.Count != 2)
                throw new ArgumentException();

            _terrainBoxes = new[] {txtStreet, txtPlains, txtForest, txtHill, txtMountain};

            // track changes from the entities
            Entity.NodeChanged += OnEntityNodeChanged;

            Console.WriteLine("Status created");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the manhatten distance between target and player and displays it in the textbox.
        /// </summary>
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
                          Math.Pow(TargetPosition.Value.Y - PlayerPosition.Value.Y, 2)), 3).ToString(CultureInfo.InvariantCulture);
            var dist = Math.Abs(TargetPosition.Value.X - PlayerPosition.Value.X) +
                       Math.Abs(TargetPosition.Value.Y - PlayerPosition.Value.Y);
            txtManhattenDistance.Text = dist.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Updates specific statistics about the map.
        /// </summary>
        /// <param name="map">Map that has been upated</param>
        /// <param name="terrains">Contains all terrain types for which the statistics have been changed</param>
        /// <param name="allTerrain">If true, all terrain types will be updated</param>
        /// <param name="fog">If true, fog statistics will be updated</param>
        public void UpdateMapStatistics(Map map, Terrain[] terrains = null, bool allTerrain = false, bool fog = false)
        {
            var cellCount = map.Width*map.Height;

            // terrain
            if (allTerrain)
                terrains = new[] {Terrain.Street, Terrain.Plains, Terrain.Forest, Terrain.Hill, Terrain.Mountain};

            if (terrains != null)
                foreach (var t in terrains)
                {
                    var count = map.Graph.Nodes.Sum(n => n == null ? 0 : n.Count(nn => nn != null && nn.Terrain == t));
                    _terrainBoxes[(int) t].Text = count + "/" + cellCount + " (" +
                                                  Math.Round((double) 100*count/cellCount, 1) + "%)";
                }

            // fog
            if (!fog)
                return;

            var foggy = map.Graph.Nodes.Sum(n => n.Count(nn => !nn.KnownToPlayer));
            txtFog.Text = foggy + "/" + cellCount + " (" + Math.Round((double) 100*foggy/cellCount, 1) + "%)";
        }

        /// <summary>
        /// Updates the entity status.
        /// </summary>
        /// <param name="oldnode">Old location of the entity</param>
        /// <param name="newnode">New location of the entity</param>
        /// <param name="entity">Entity that has been moved</param>
        private void OnEntityNodeChanged(Node oldnode, Node newnode, Entity entity)
        {
            switch (entity.Type)
            {
                case EntityType.Player:
                    if (newnode == null)
                        PlayerPosition = null;
                    else
                        PlayerPosition = newnode.Location;
                    break;
                case EntityType.Target:
                    if (newnode == null)
                        TargetPosition = null;
                    else
                        TargetPosition = newnode.Location;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #endregion
    }
}
