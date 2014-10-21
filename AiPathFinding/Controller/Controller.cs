using System.Drawing;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Controller
{
    public class Controller
    {
        public Controller(Map map)
        {
            Map = map;

            MainForm.CellClicked += OnCellClick;
        }

        public Map Map { get; private set; }

        public void OnCellClick(Point location)
        {
            // TODO: Change cell click behavior here

            Map.SetNodeType(location, (NodeType)(((int)Map.GetNodeType(location) + 1) % (int)NodeType.Count));
        }

        public void OnMapSizeChanged(int width, int height)
        {
            Map.SetMapSize(width, height);
        }
    }
}
