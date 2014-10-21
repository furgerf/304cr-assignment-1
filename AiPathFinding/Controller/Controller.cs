using System.Drawing;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Controller
{
    public class Controller
    {
        #region Fields

        public Map Map { get; private set; }

        #endregion

        #region Constructor

        public Controller(Map map, MainForm form, MapSettings settings)
        {
            // assign fields
            Map = map;

            // register events
            form.CellClicked += OnCellClick;
            settings.MapSizeChanged += OnMapSizeChanged;
        }

        #endregion

        #region Event Handling

        public void OnCellClick(Point location)
        {
            // TODO: Change cell click behavior here

            Map.SetNodeType(location, (NodeType)(((int)Map.GetNodeType(location) + 1) % (int)NodeType.Count));
        }

        public void OnMapSizeChanged(int width, int height)
        {
            // tell model to adjust map size
            Map.SetMapSize(width, height);
        }

        #endregion 
    }
}
