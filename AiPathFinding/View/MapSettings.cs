using System;
using System.Windows.Forms;
using AiPathFinding.Properties;

namespace AiPathFinding.View
{
    /// <summary>
    /// Lets the user change settings about the map.
    /// </summary>
    public sealed partial class MapSettings : UserControl
    {
        #region Fields

        /// <summary>
        /// Gets the map's width.
        /// </summary>
        public int MapWidth
        {
            get { return (int) numMapWidth.Value; }
        }

        /// <summary>
        /// Gets the map's height.
        /// </summary>
        public int MapHeight
        {
            get { return (int) numMapHeight.Value; }
        }

        /// <summary>
        /// Gets the cell size.
        /// </summary>
        public int CellSize
        {
            get { return (int) numCellSize.Value; }
        }

        /// <summary>
        /// Gets the street weight.
        /// </summary>
        public int StreetWeight
        {
            get { return (int)numStreet.Value; }
        }

        /// <summary>
        /// Gets the plains weight.
        /// </summary>
        public int PlainsWeight
        {
            get { return (int)numPlains.Value; }
        }

        /// <summary>
        /// Gets the forest weight.
        /// </summary>
        public int ForestWeight
        {
            get { return (int)numForest.Value; }
        }

        /// <summary>
        /// Gets the hills weight.
        /// </summary>
        public int HillWeight
        {
            get { return (int)numHill.Value; }
        }

        /// <summary>
        /// Gets the mountain weight.
        /// </summary>
        public int MountainWeight
        {
            get { return (int)numMountain.Value; }
        }

        /// <summary>
        /// Gets the fog percentage.
        /// </summary>
        public double FogPercentage
        {
            get { return (double)numFog.Value; }
        }

        /// <summary>
        /// Can be used to suppress the map size changed event.
        /// </summary>
        private bool _suppressMapSizeChangedEvent;

        #endregion

        #region Events

        /// <summary>
        /// Triggered when the user changes the map size. This is needed to since this control doesn't know the map.
        /// </summary>
        /// <param name="width">New map width</param>
        /// <param name="height">New map height</param>
        public delegate void OnMapSizeChanged(int width, int height);

        public event OnMapSizeChanged MapSizeChanged;

        /// <summary>
        /// Triggered when the user changes the cell size.
        /// </summary>
        /// <param name="cellSize">New cell size</param>
        public delegate void OnCellSizeChanged(int cellSize);

        public event OnCellSizeChanged CellSizeChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapSettings()
        {
            InitializeComponent();

            // register events
            numMapWidth.ValueChanged += (s, e) => { if (MapSizeChanged != null && !_suppressMapSizeChangedEvent) MapSizeChanged(MapWidth, MapHeight); };
            numMapHeight.ValueChanged += (s, e) => { if (MapSizeChanged != null && !_suppressMapSizeChangedEvent) MapSizeChanged(MapWidth, MapHeight); };
            numCellSize.ValueChanged += (s, e) =>
            {
                if (CellSizeChanged != null) CellSizeChanged(CellSize);
                Settings.Default.CellSize = CellSize;
                Settings.Default.Save();
            };

            // load settings
            numFog.Value = Settings.Default.FogPercentage;
            numStreet.Value = Settings.Default.StreetWeight;
            numPlains.Value = Settings.Default.PlainsWeight;
            numForest.Value = Settings.Default.ForestWeight;
            numHill.Value = Settings.Default.HillWeight;
            numMountain.Value = Settings.Default.MountainWeight;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets a new map size.
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="cellSize">New cell size</param>
        public void SetMapSize(int width, int height, int cellSize = 0)
        {
            // only call the event when both nums are set
            // otherwise, some rows/columns get reset
            _suppressMapSizeChangedEvent = true;
            numMapWidth.Value = width;
            _suppressMapSizeChangedEvent = false;
            numMapHeight.Value = height;

            if (cellSize != 0)
                numCellSize.Value = cellSize;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numFog_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.FogPercentage = numFog.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numStreet_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.StreetWeight = (int)numStreet.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numPlains_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.PlainsWeight = (int)numPlains.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numForest_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.ForestWeight = (int)numForest.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numHill_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.HillWeight = (int)numHill.Value;
            Settings.Default.Save();
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void numMountain_ValueChanged(object sender = null, EventArgs e = null)
        {
            Settings.Default.MountainWeight = (int) numMountain.Value;
            Settings.Default.Save();
        }

        #endregion
    }
}
