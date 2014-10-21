using System;
using System.Drawing;

namespace AiPathFinding
{
    public class Controller
    {
        public Controller(Model model)
        {
            Model = model;

            MainForm.CellClicked += OnCellClick;
        }

        public Model Model { get; private set; }

        public void OnCellClick(Point location)
        {
            // TODO: Change cell click behavior here

            Model.SetNodeType(location, (NodeType)(((int)Model.GetNodeType(location) + 1) % (int)NodeType.Count));
        }

        public void OnMapSizeChanged(int width, int height)
        {
            Model.SetMapSize(width, height);
        }
    }
}
