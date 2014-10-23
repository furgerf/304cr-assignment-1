using System;
using System.Drawing;
using System.Windows.Forms;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class Status : UserControl
    {
        private Point? _playerPosition;
        private Point? _targetPosition;

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

        #endregion
    }
}
