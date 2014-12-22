using System.Windows.Forms;
using AiPathFinding.Properties;

namespace AiPathFinding.View
{
    public partial class MapSettings : UserControl
    {
        #region Fields

        public int MapWidth
        {
            get { return (int) numMapWidth.Value; }
        }

        public int MapHeight
        {
            get { return (int) numMapHeight.Value; }
        }

        public int CellSize
        {
            get { return (int) numCellSize.Value; }
        }

        public int StreetWeight
        {
            get { return (int)numStreet.Value; }
        }

        public int PlainsWeight
        {
            get { return (int)numPlains.Value; }
        }

        public int ForestWeight
        {
            get { return (int)numForest.Value; }
        }

        public int HillWeight
        {
            get { return (int)numHill.Value; }
        }

        public int MountainWeight
        {
            get { return (int)numMountain.Value; }
        }

        public double FogPercentage
        {
            get { return (double)numFog.Value; }
        }

        private bool _suppressMapSizeChangedEvent;

        #endregion

        #region Events

        public delegate void OnMapSizeChanged(int width, int height);

        public event OnMapSizeChanged MapSizeChanged;

        public delegate void OnCellSizeChanged(int cellSize);

        public event OnCellSizeChanged CellSizeChanged;

        #endregion

        #region Constructor

        public MapSettings()
        {
            InitializeComponent();

            numMapWidth.ValueChanged += (s, e) => { if (MapSizeChanged != null && !_suppressMapSizeChangedEvent) MapSizeChanged(MapWidth, MapHeight); };
            numMapHeight.ValueChanged += (s, e) => { if (MapSizeChanged != null && !_suppressMapSizeChangedEvent) MapSizeChanged(MapWidth, MapHeight); };
            numCellSize.ValueChanged += (s, e) => { if (CellSizeChanged != null) CellSizeChanged(CellSize); };

            numFog.Value = Settings.Default.FogPercentage;
            numStreet.Value = Settings.Default.StreetWeight;
            numPlains.Value = Settings.Default.PlainsWeight;
            numForest.Value = Settings.Default.ForestWeight;
            numHill.Value = Settings.Default.HillWeight;
            numMountain.Value = Settings.Default.MountainWeight;
        }

        #endregion

        #region Methods

        public void SetMapSize(int width, int height, int cellSize = 0)
        {
            // only call the event when both nums are set
            _suppressMapSizeChangedEvent = true;
            numMapWidth.Value = width;
            _suppressMapSizeChangedEvent = false;
            numMapHeight.Value = height;

            if (cellSize != 0)
                numCellSize.Value = cellSize;
        }

        #endregion

        #region Event Handling

        private void numFog_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.FogPercentage = numFog.Value;
            Settings.Default.Save();
        }

        private void numStreet_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.StreetWeight = (int)numStreet.Value;
            Settings.Default.Save();
        }

        private void numPlains_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.PlainsWeight = (int)numPlains.Value;
            Settings.Default.Save();
        }

        private void numForest_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.ForestWeight = (int)numForest.Value;
            Settings.Default.Save();
        }

        private void numHill_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.HillWeight = (int)numHill.Value;
            Settings.Default.Save();
        }

        private void numMountain_ValueChanged(object sender, System.EventArgs e)
        {
            Settings.Default.MountainWeight = (int) numMountain.Value;
            Settings.Default.Save();
        }

        #endregion
    }
}
