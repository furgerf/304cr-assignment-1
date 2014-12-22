using System.Windows.Forms;

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

            numMapWidth.ValueChanged += (s, e) => { if (MapSizeChanged != null) MapSizeChanged(MapWidth, MapHeight); };
            numMapHeight.ValueChanged += (s, e) => { if (MapSizeChanged != null) MapSizeChanged(MapWidth, MapHeight); };
            numCellSize.ValueChanged += (s, e) => { if (MapSizeChanged != null) CellSizeChanged(CellSize); };
        }

        #endregion

        #region Methods

        public void SetMapSize(int width, int height, int cellSize = 0)
        {
            numMapWidth.Value = width;
            numMapHeight.Value = height;

            if (cellSize != 0)
                numCellSize.Value = cellSize;
        }

        #endregion
    }
}
