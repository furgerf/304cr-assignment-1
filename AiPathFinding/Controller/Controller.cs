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

        private Point[] _selectedPoints;

        #endregion

        #region Constructor

        public Controller(Map map, MainForm form, MapSettings settings)
        {
            // assign fields
            Map = map;

            // create context menu
            for (var i = 0; i < (int) Terrain.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Change terrain to &" + (Terrain)i, (s, e) =>
                {
                    if (_selectedPoints == null) return;
                    for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                        for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                            Map.SetTerrain(new Point(k, l), (Terrain) i1);
                }));
            }
            _mapCellContextMenu.MenuItems.Add(new MenuItem("-"));

            _mapCellContextMenu.MenuItems.Add(new MenuItem("Toggle &fog", (s, e) =>
            {
                if (_selectedPoints == null) return;
                for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                    for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                        Map.SetFog(new Point(k, l), !Map.GetFog(new Point(k, l)));
            }));
            _mapCellContextMenu.MenuItems.Add(new MenuItem("Clear &fog", (s, e) =>
            {
                if (_selectedPoints == null) return;
                for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                    for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                        Map.SetFog(new Point(k, l), false);
            }));
            _mapCellContextMenu.MenuItems.Add(new MenuItem("Set &fog", (s, e) =>
            {
                if (_selectedPoints == null) return;
                for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                    for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                        Map.SetFog(new Point(k, l), true);
            }));

            _mapCellContextMenu.MenuItems.Add(new MenuItem("-"));
            for (var i = 0; i < (int) EntityType.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Set &" + (EntityType)i1 + " entity here",
                    (s, e) =>
                    {
                        if (_selectedPoints[0] != _selectedPoints[1])
                            throw new Exception();
                        Map.SetEntityLocation(Entity.Entities[i1], _selectedPoints[0]);
                    }){ Name = "Entity" + i });
            }

            form.ContextMenu = _mapCellContextMenu;

            // register events
            //form.CellClicked += OnCellClick;
            form.SelectedPointsChanged += OnSelectedPointsChanged;
            settings.MapSizeChanged += OnMapSizeChanged;
        }

        #endregion

        #region Event Handling

        public void OnSelectedPointsChanged(Point[] points)
        {
            if (points.Length != 2)
                throw new ArgumentException();

            _selectedPoints = points;

            for (var i = 0; i < (int) EntityType.Count; i++)
                _mapCellContextMenu.MenuItems["Entity"+i].Enabled = points[0] == points[1];
        }

        public void OnMapSizeChanged(int width, int height)
        {
            // tell model to adjust map size
            Map.SetMapSize(width, height);
        }

        #endregion 
    }
}
