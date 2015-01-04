using System;
using System.Drawing;
using System.Windows.Forms;
using AiPathFinding.Common;
using AiPathFinding.Model;
using AiPathFinding.View;

namespace AiPathFinding.Controller
{
    /// <summary>
    /// Class that handles user input.
    /// </summary>
    public sealed class Controller
    {
        #region Fields

        /// <summary>
        /// ContextMenu that is displayed when cell on the map is right-clicked.
        /// </summary>
        private readonly ContextMenu _mapCellContextMenu = new ContextMenu();

        /// <summary>
        /// Map instance to interact with.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Start- and endpoint of the range of points that have been selected by the user.
        /// </summary>
        private Point[] _selectedPoints;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="map">Map model</param>
        /// <param name="form">Form that offers some events</param>
        /// <param name="canvas">Visible map that can be clicked</param>
        /// <param name="settings">Settings that offer some events</param>
        public Controller(Map map, MainForm form, Control canvas, MapSettings settings)
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
            _mapCellContextMenu.MenuItems.AddRange(new[]
            {
                new MenuItem("-"),
                new MenuItem("Toggle &fog", (s, e) =>
                {
                    if (_selectedPoints == null) return;
                    for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                        for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                            Map.SetFog(new Point(k, l), !Map.HasFog(new Point(k, l)));
                }),
                new MenuItem("Clear &fog", (s, e) =>
                {
                    if (_selectedPoints == null) return;
                    for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                        for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                            Map.SetFog(new Point(k, l), false);
                }),
                new MenuItem("Set &fog", (s, e) =>
                {
                    if (_selectedPoints == null) return;
                    for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                        for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                            Map.SetFog(new Point(k, l), true);
                }),
                new MenuItem("-")
            });
            for (var i = 0; i < (int) EntityType.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Set &" + (EntityType)i1 + " entity here",
                    (s, e) =>
                    {
                        if (_selectedPoints == null) return;
                        if (_selectedPoints[0] != _selectedPoints[1])
                            throw new Exception();
                        Map.SetEntityLocation(Entity.Entities[i1], _selectedPoints[0]);
                    }){ Name = "Entity" + i });
            }

            canvas.ContextMenu = _mapCellContextMenu;

            // register events
            form.SelectedPointsChanged += OnSelectedPointsChanged;
            settings.MapSizeChanged += Map.SetMapSize;

            Console.WriteLine("Controller created!");
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Called whenever the selected points change
        /// </summary>
        /// <param name="points"></param>
        public void OnSelectedPointsChanged(Point[] points)
        {
            // ensure data is valid
            if (points.Length != 2)
                throw new ArgumentException();

            // update data
            _selectedPoints = points;

            // the user can only move entities if just one point is selected
            for (var i = 0; i < (int) EntityType.Count; i++)
                _mapCellContextMenu.MenuItems["Entity"+i].Enabled = points[0] == points[1];
        }

        #endregion 
    }
}
