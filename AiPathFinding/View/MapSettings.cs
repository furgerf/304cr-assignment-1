using System.Windows.Forms;

namespace AiPathFinding.View
{
    public partial class MapSettings : UserControl
    {
        public delegate void OnMapSizeChanged(int width, int height);
        public event OnMapSizeChanged MapSizeChanged;

        public delegate void OnCellSizeChanged(int cellSize);
        public event OnCellSizeChanged CellSizeChanged;

        public int MapWidth { get { return (int)numMapWidth.Value; } }

        public int MapHeight { get { return (int)numMapHeight.Value; } }

        public int CellSize { get { return (int)numCellSize.Value; } }

        public MapSettings()
        {
            InitializeComponent();

            numMapWidth.ValueChanged += (s, e) => MapSizeChanged(MapWidth, MapHeight);
            numMapHeight.ValueChanged += (s, e) => MapSizeChanged(MapWidth, MapHeight);
            numCellSize.ValueChanged += (s, e) => CellSizeChanged(CellSize);
        }
    }
}
