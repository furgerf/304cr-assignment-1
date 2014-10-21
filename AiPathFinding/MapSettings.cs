using System.Windows.Forms;

namespace AiPathFinding
{
    public partial class MapSettings : UserControl
    {
        public delegate void OnMapSizeChanged(int width, int height);

        public event OnMapSizeChanged MapSizeChanged;

        public int MapWidth { get { return (int)numMapWidth.Value; } }

        public int MapHeight { get { return (int)numMapHeight.Value; } }

        public MapSettings()
        {
            InitializeComponent();

            numMapWidth.ValueChanged += (s, e) => MapSizeChanged((int)numMapWidth.Value, (int)numMapHeight.Value);
            numMapHeight.ValueChanged += (s, e) => MapSizeChanged((int)numMapWidth.Value, (int)numMapHeight.Value);
        }
    }
}
