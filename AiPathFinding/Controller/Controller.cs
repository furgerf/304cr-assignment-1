using System;
using System.Drawing;
using System.Windows.Forms;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Controller
{
    public class Controller
    {
        #region Fields

        private readonly ContextMenu _mapCellContextMenu = new ContextMenu();

        public Map Map { get; private set; }

        #endregion

        #region Constructor

        public Controller(Map map, MainForm form, MapSettings settings)
        {
            // assign fields
            Map = map;

            // create context menu
            for (var i = 0; i < (int) NodeType.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Change terrain to &" + (NodeType)i, (s, e) => Map.SetNodeType(_lastRightClickLocation, (NodeType) i1)));
            }
            _mapCellContextMenu.MenuItems.Add(new MenuItem("-"));

            _mapCellContextMenu.MenuItems.Add(new MenuItem("Toggle &fog", (s, e) => Map.ToggleFog(_lastRightClickLocation)));

            _mapCellContextMenu.MenuItems.Add(new MenuItem("-"));
            for (var i = 0; i < (int) EntityType.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Set &" + (EntityType)i1 + " entity here",
                    (s, e) => Map.SetEntityLocation(Entity.Entities[i1], _lastRightClickLocation)));
            }

            form.ContextMenu = _mapCellContextMenu;

            // register events
            form.CellClicked += OnCellClick;
            settings.MapSizeChanged += OnMapSizeChanged;
        }

        #endregion

        #region Event Handling

        private Point _lastRightClickLocation;

        public void OnCellClick(Point location, MouseButtons button)
        {
            // left button: change terrain type
            if (button == MouseButtons.Left)
                Map.SetNodeType(location, (NodeType) (((int) Map.GetNodeType(location) + 1)%(int) NodeType.Count));

            // right button: open context menu
            _lastRightClickLocation = location;
        }

        public void OnMapSizeChanged(int width, int height)
        {
            // tell model to adjust map size
            Map.SetMapSize(width, height);
        }

        #endregion 
    }
}
