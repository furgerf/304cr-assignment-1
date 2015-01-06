using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Common;
using AiPathFinding.Model;
using AiPathFinding.Properties;

namespace AiPathFinding.View
{
    /// <summary>
    /// Main form of the application.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        #region Fields

        /// <summary>
        /// Name of the file where the map should be autosaved to.
        /// </summary>
        private const string AutosaveMapName = "autosave.map";

        /// <summary>
        /// Action that draws the algorithm step.
        /// </summary>
        private Action<Graphics> DrawAlgorithmStep { get; set; }

        /// <summary>
        /// Disables flickering when drawing on canvas.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }

        // pens
        private readonly Pen _gridPen = new Pen(Color.LightGray, 1);
        private readonly Pen _selectionPen = new Pen(Color.HotPink, 3);
        private readonly Pen _fineSelectionPen = new Pen(Color.LightPink, 1);
        
        // brushes
        private readonly Brush _fogBrush = new SolidBrush(Color.FromArgb(125, 0, 0, 0));

        // images
        private readonly Image _streetImage = Resources.street;
        private readonly Image _plainsImage = Resources.plains;
        private readonly Image _forestImage = Resources.forest;
        private readonly Image _hillImage = Resources.hill;
        private readonly Image _mountainImage = Resources.mountain;
        /// <summary>
        /// Contains all images.
        /// </summary>
        private readonly Image[] _landscapeImages;

        // important stuff
        /// <summary>
        /// Model: Map.
        /// </summary>
        public readonly Map Map;
        /// <summary>
        /// Controller.
        /// </summary>
        //public readonly Controller.Controller Controller;
        /// <summary>
        /// View: MainForm instance.
        /// </summary>
        private static MainForm _instance;

        // fields for handling cell selection
        private bool _drawSelection;
        private readonly Point[] _selectionRange = new Point[2];
        private Point? _mouseDownLocation;

        // field for handling shift/control key
        private bool _controlKeyActive;
        private bool _shiftKeyActive;


        /// <summary>
        /// ContextMenu that is displayed when cell on the map is right-clicked.
        /// </summary>
        private readonly ContextMenu _mapCellContextMenu = new ContextMenu();

        /// <summary>
        /// Start- and endpoint of the range of points that have been selected by the user.
        /// </summary>
        private Point[] _selectedPoints;

        #endregion

        #region Events

        /// <summary>
        /// Triggered when a new range of nodes has been selected.
        /// </summary>
        /// <param name="points">Start- and endpoint of selection</param>
        public delegate void OnSelectedPointsChanged(Point[] points);

        public event OnSelectedPointsChanged SelectedPointsChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainForm()
        {
            Console.WriteLine("Creating MainForm");

            InitializeComponent();

            _instance = this;

            // maximize window
            WindowState = FormWindowState.Maximized;

            // instantiate objects
            _landscapeImages = new[] {_streetImage, _plainsImage, _forestImage, _hillImage, _mountainImage};

            // create map
            Map = File.Exists(AutosaveMapName)
                ? Map.FromMapFile(AutosaveMapName)
                : new Map(Graph.EmptyGraph(mapSettings.MapWidth, mapSettings.MapHeight));

            // create context menu
            CreateContextMenu();

            // create tooltip
            var toolTip = new ToolTip {BackColor = Color.LightGreen, ForeColor = Color.DarkGreen};
            _canvas.MouseMove += (s, e) =>
            {
                var p = CanvasPointToMapPoint(e.Location);
                if (toolTip.Tag != null && (Point)toolTip.Tag == p)
                    return;

                if (p.X >= Map.Width || p.Y >= Map.Height)
                {
                    toolTip.Tag = null;
                    toolTip.Hide(_canvas);
                    return;
                }

                var rect = MapPointToCanvasRectangle(p);
                toolTip.Show(p.X + "/" + p.Y, _canvas, new Point(rect.Left + rect.Width/2, rect.Top + rect.Height / 2));
                toolTip.Tag = p;
            };

            // REGISTER EVENTS
            // get paint hook
            _canvas.Paint += DrawMap;

            // track changes in the settings
            mapSettings.CellSizeChanged += OnCellSizeChanged;

            // track changes in the model
            Map.CellTerrainChanged += (a, b, c) => _canvas.Invalidate();
            Map.CellTerrainChanged += (l, o, n) => status.UpdateMapStatistics(Map, new[] {o, n});
            Map.MapSizeChanged += OnMapSizeChanged;
            Map.MapSizeChanged += (a, b, c, d) => status.UpdateMapStatistics(Map, null, true);
            Map.FogChanged += (a, b) => _canvas.Invalidate();
            Map.FogChanged += (a, b) => status.UpdateMapStatistics(Map, null, false, true);
            Map.MapLoaded += OnMapLoaded;
            Map.MapLoaded += () => status.UpdateMapStatistics(Map, null, true, true);

            // track changes from the entities
            Entity.NodeChanged += (a, b, c) => _canvas.Invalidate();

            // track user input
            _canvas.MouseDown += CanvasOnMouseDown;
            _canvas.MouseUp += CanvasOnMouseUp;
            _canvas.MouseMove += CanvasOnMouseMove;
            MouseWheel += OnMouseWheel;
            KeyDown += (s, e) =>
            {
                _controlKeyActive = e.Control;
                _shiftKeyActive = e.Shift;
            };
            KeyUp += (s, e) =>
            {
                _controlKeyActive = e.Control;
                _shiftKeyActive = e.Shift;
            };

            // settings buttons event handling
            mapSettings.butLoadMap.Click += (s, e) =>
            {
                algorithmSettings.ResetAlgorithm();
                LoadMap();
            };
            mapSettings.butSaveMap.Click += (s, e) => SaveMap();
            mapSettings.butGenerate.Click += (s, e) =>
            {
                algorithmSettings.ResetAlgorithm();
                RegenerateMap();
            };
            mapSettings.MapSizeChanged += Map.SetMapSize;

            // algorithm stuff
            algorithmSettings.AlgorithmStepChanged += OnAlgorithmStepChanged;
            algorithmSettings.RegisterGraph(Map.Graph);

            SelectedPointsChanged += UpdateSelectedPoints;
            SelectedPointsChanged += a =>
            {
                UpdateSelectedPoints(a);
                _canvas.Invalidate();
            };

            // prepare GUI (depending on whether map was loaded
            if (File.Exists(AutosaveMapName))
                OnMapLoaded(); 
            else
                SetCanvasSize();

            // update status (map)
            status.UpdateMapStatistics(Map, null, true, true);

            Console.WriteLine("MainForm created!");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns start- and endpoint of range, depending on location and _mouseDownLocation.
        /// </summary>
        /// <param name="location">Location of the release point</param>
        /// <returns>Start- and endpoint of the range</returns>
        private Point[] GetPointRange(Point location)
        {
            if (_mouseDownLocation == null)
                throw new Exception();

            // return array with 2 points, p1 containing the lowest x/y values while p2 contains the highest x/y values
            return new[]
            {
                new Point(location.X < _mouseDownLocation.Value.X ? location.X : _mouseDownLocation.Value.X,
                    location.Y < _mouseDownLocation.Value.Y ? location.Y : _mouseDownLocation.Value.Y),
                new Point(location.X > _mouseDownLocation.Value.X ? location.X : _mouseDownLocation.Value.X,
                    location.Y > _mouseDownLocation.Value.Y ? location.Y : _mouseDownLocation.Value.Y)
            };
        }

        /// <summary>
        /// Saves the current map to a file.
        /// </summary>
        private void SaveMap()
        {
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "map",
                Filter = "Map Files|*.map",
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Select File to save"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
                Map.SaveMap(dlg.FileName);
        }

        /// <summary>
        /// Loads a map from a file.
        /// </summary>
        private void LoadMap()
        {
            var dlg = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = "map",
                Filter = "Map Files|*.map",
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Select File to load"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            // load map
            Map.LoadMap(dlg.FileName);

            // update entity locations
            foreach (var e in Entity.Entities)
                e.Node = Map.Graph.Nodes[e.Node.Location.X][e.Node.Location.Y];

            // re-create algorithms with new map
            algorithmSettings.RegisterGraph(Map.Graph);
        }

        /// <summary>
        /// Creates a new, random map.
        /// </summary>
        private void RegenerateMap()
        {
            // generate new map
            Map.RegenerateMap(mapSettings.MapWidth, mapSettings.MapHeight, mapSettings.StreetWeight, mapSettings.PlainsWeight, mapSettings.ForestWeight, mapSettings.HillWeight, mapSettings.MountainWeight, mapSettings.FogPercentage);

            // re-create algorithms with new map
            algorithmSettings.RegisterGraph(Map.Graph);
        }

        /// <summary>
        /// Sets the size of the canvas.
        /// </summary>
        private void SetCanvasSize()
        {
            _canvas.Size = new Size(mapSettings.CellSize * mapSettings.MapWidth + 3, mapSettings.CellSize * mapSettings.MapHeight + 3);
        }

        /// <summary>
        /// Converts a location on the map to a canvas rectangle.
        /// </summary>
        /// <param name="point">Location of the cell on the map</param>
        /// <returns>Rectangle on the canvas</returns>
        public static Rectangle MapPointToCanvasRectangle(Point point)
        {
            return new Rectangle(new Point(point.X * _instance.mapSettings.CellSize + 1, point.Y * _instance.mapSettings.CellSize + 1), new Size(_instance.mapSettings.CellSize - 1, _instance.mapSettings.CellSize - 1));
        }

        /// <summary>
        /// Converts a location on the canvas to the corresponding map node.
        /// </summary>
        /// <param name="point">Location on the canvas</param>
        /// <returns>Location on the map</returns>
        private Point CanvasPointToMapPoint(Point point)
        {
            return new Point(point.X / mapSettings.CellSize, point.Y / mapSettings.CellSize);
        }

        /// <summary>
        /// Determines whether a point on the canvas is on a cell or on the grid.
        /// </summary>
        /// <param name="location">Location on the map</param>
        /// <returns>True if the point is on the grid</returns>
        private bool IsPointOnGrid(Point location)
        {
            return location.X % mapSettings.CellSize == 0 || location.Y % mapSettings.CellSize == 0;
        }

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">PaintEventArgs that contains the graphics used for drawing</param>
        private void DrawMap(object sender, PaintEventArgs e)
        {
            // ensure the graphics will be disposed properly
            using (var g = e.Graphics)
            {
                // draw stuff in proper order
                DrawGrid(g);
                DrawLandscape(g);
                DrawEntities(g);
                DrawFog(g);
                DrawSelectedCells(g);

                if (DrawAlgorithmStep != null)
                    DrawAlgorithmStep(g);
            }
        }

        /// <summary>
        /// Makes the cell selection visible.
        /// </summary>
        /// <param name="g">Graphics used for drawing</param>
        private void DrawSelectedCells(Graphics g)
        {
            if (!_drawSelection)
                return;

            var rect = new Rectangle(MapPointToCanvasRectangle(_selectionRange[0]).X + 1,
                MapPointToCanvasRectangle(_selectionRange[0]).Y + 1,
                MapPointToCanvasRectangle(_selectionRange[1]).X -
                MapPointToCanvasRectangle(_selectionRange[0]).X +
                MapPointToCanvasRectangle(_selectionRange[1]).Width - 3,
                MapPointToCanvasRectangle(_selectionRange[1]).Y -
                MapPointToCanvasRectangle(_selectionRange[0]).Y +
                MapPointToCanvasRectangle(_selectionRange[1]).Height - 3);
            g.DrawRectangle(_mouseDownLocation == null ? _selectionPen : _fineSelectionPen, rect);
        }

        /// <summary>
        /// Draws the fog.
        /// </summary>
        /// <param name="g">Graphics used for drawing</param>
        private void DrawFog(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    if (Map.HasFog(new Point(i, j)))
                        g.FillRectangle(_fogBrush, MapPointToCanvasRectangle(new Point(i, j)));
        }

        /// <summary>
        /// Draws the entities.
        /// </summary>
        /// <param name="g">Graphics used for drawing</param>
        private static void DrawEntities(Graphics g)
        {
            foreach (var e in Entity.Entities.Where(ee => ee.IsVisible))
                g.DrawIcon(e.Icon, MapPointToCanvasRectangle(new Point(e.Node.Location.X, e.Node.Location.Y)));
        }

        /// <summary>
        /// Draws the terrain.
        /// </summary>
        /// <param name="g">Graphics used for drawing</param>
        private void DrawLandscape(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    g.DrawImage(_landscapeImages[(int)Map.GetTerrain(new Point(i, j))], MapPointToCanvasRectangle(new Point(i, j)));
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="g">Graphics used for drawing</param>
        private void DrawGrid(Graphics g)
        {
            for (var i = 0; i < _canvas.Height; i += mapSettings.CellSize)
                g.DrawLine(_gridPen, 0, i, _canvas.Width, i);

            for (var i = 0; i < _canvas.Width; i += mapSettings.CellSize)
                g.DrawLine(_gridPen, i, 0, i, _canvas.Height);
        }

        /// <summary>
        /// Arranges the 4 main controls depending on which are visible.
        /// </summary>
        private void ArrangeControls()
        {
            _canvas.Left = !algorithmSettings.Visible && !mapSettings.Visible && !status.Visible
                ? toolStrip.Left + toolStrip.Width + 6
                : algorithmSettings.Left + algorithmSettings.Width + 6;

            if (mapSettings.Visible)
            {
                mapSettings.Top = algorithmSettings.Visible ? algorithmSettings.Top + algorithmSettings.Height + 6 : 12;

                if (status.Visible)
                    status.Top = mapSettings.Top + mapSettings.Height + 6;
            }
            else if (status.Visible)
                status.Top = algorithmSettings.Visible ? algorithmSettings.Top + algorithmSettings.Height + 6 : 12;
        }

        /// <summary>
        /// Creates the context menu and sets it to the canvas.
        /// </summary>
        private void CreateContextMenu()
        {
            for (var i = 0; i < (int)Terrain.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Change terrain to &" + (Terrain)i, (s, e) =>
                {
                    if (_selectedPoints == null) return;
                    for (var k = _selectedPoints[0].X; k <= _selectedPoints[1].X; k++)
                        for (var l = _selectedPoints[0].Y; l <= _selectedPoints[1].Y; l++)
                            Map.SetTerrain(new Point(k, l), (Terrain)i1);
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
            for (var i = 0; i < (int)EntityType.Count; i++)
            {
                var i1 = i;
                _mapCellContextMenu.MenuItems.Add(new MenuItem("Set &" + (EntityType)i1 + " entity here",
                    (s, e) =>
                    {
                        if (_selectedPoints == null) return;
                        if (_selectedPoints[0] != _selectedPoints[1])
                            throw new Exception();
                        Map.SetEntityLocation(Entity.Entities[i1], _selectedPoints[0]);
                    }) { Name = "Entity" + i });
            }
            _canvas.ContextMenu = _mapCellContextMenu;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Updates the algorithm step draw method and redraws.
        /// </summary>
        /// <param name="step">Algorithm step</param>
        private void OnAlgorithmStepChanged(AlgorithmStep step)
        {
            DrawAlgorithmStep = step == null ? null : step.DrawStep;

            _canvas.Invalidate();
        }

        /// <summary>
        /// Updates the canvas size and redraws.
        /// </summary>
        /// <param name="cellSize">unused</param>
        private void OnCellSizeChanged(int cellSize)
        {
            SetCanvasSize();

            _canvas.Invalidate();
        }

        /// <summary>
        /// Updates the canvas size and redraws.
        /// </summary>
        /// <param name="oldWidth">unused</param>
        /// <param name="oldHeight">unused</param>
        /// <param name="newWidth">unused</param>
        /// <param name="newHeight">unused</param>
        private void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            SetCanvasSize();

            _canvas.Invalidate();
        }

        /// <summary>
        /// Updates stuff and redraws.
        /// </summary>
        private void OnMapLoaded()
        {
            mapSettings.SetMapSize(Map.Width, Map.Height, Settings.Default.CellSize);

            SetCanvasSize();

            _canvas.Invalidate();
        }

        /// <summary>
        /// Sets double buffering for flicker-less drawing.
        /// </summary>
        /// <param name="e">unused (here)</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DoubleBuffered = true;
        }

        /// <summary>
        /// Called when the user uses the mouse wheel.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">MouseEventArgs</param>
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                algorithmSettings.butNext_Click((_controlKeyActive ? 5 : 1) * (_shiftKeyActive ? 10 : 1));
            if (e.Delta < 0)
                algorithmSettings.butPrevious_Click((_controlKeyActive ? 5 : 1) * (_shiftKeyActive ? 10 : 1));
        }

        /// <summary>
        /// Called when a mouse button is pressed on the canvas. Used to update cell selection.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="me">MouseEventArgs</param>
        private void CanvasOnMouseDown(object sender, MouseEventArgs me)
        {
            // ignore if click was on grid
            if (IsPointOnGrid(me.Location))
                return;

            // all we do here is designed to work with left clicks only
            if (me.Button != MouseButtons.Left)
                return;

            _drawSelection = true;

            _mouseDownLocation = me.Location;
        }

        /// <summary>
        /// Called when a mouse button is released on the canvas. Used to update cell selection.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="me">MouseEventArgs</param>
        private void CanvasOnMouseUp(object sender, MouseEventArgs me)
        {
            // ignore if click was on grid or started on grid
            if (IsPointOnGrid(me.Location) || _mouseDownLocation == null)
                return;

            // all we do here is designed to work with left clicks only
            if (me.Button != MouseButtons.Left)
                return;

            var pt = new Point(me.Location.X > _canvas.Width - 4 ? _canvas.Width - 4 : me.Location.X,
                me.Location.Y > _canvas.Height - 4 ? _canvas.Height - 4 : me.Location.Y);

            var pts = GetPointRange(pt);
            _selectionRange[0] = CanvasPointToMapPoint(pts[0]);
            _selectionRange[1] = CanvasPointToMapPoint(pts[1]);

            if (SelectedPointsChanged != null)
                SelectedPointsChanged(_selectionRange);

            _mouseDownLocation = null;
        }

        /// <summary>
        /// Called when the mouse is moved on the canvas. Used to update cell selection.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="me">MouseEventArgs</param>
        private void CanvasOnMouseMove(object sender, MouseEventArgs me)
        {
            // all we do here is designed to work with left clicks only
            if (me.Button != MouseButtons.Left)
                return;

            // only update if necessary
            if (_mouseDownLocation == null)
                return;

            // ignore if we're off grid
            if (me.Location.X > _canvas.Width - 2 || me.Location.Y > _canvas.Height - 2)
                return;

            var pts  = GetPointRange(me.Location);
            _selectionRange[0] = CanvasPointToMapPoint(pts[0]);
            _selectionRange[1] = CanvasPointToMapPoint(pts[1]);

            if (SelectedPointsChanged != null)
                SelectedPointsChanged(_selectionRange);
        }

        /// <summary>
        /// Called when the form is closing, saves the map.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save map
            Map.SaveMap(AutosaveMapName);
        }

        /// <summary>
        /// Toggles visibility of the algorithm settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butAlgorithmSettings_Click(object sender = null, EventArgs e = null)
        {
            butAlgorithmSettings.Checked = !butAlgorithmSettings.Checked;

            algorithmSettings.Visible = butAlgorithmSettings.Checked;

            ArrangeControls();
        }

        /// <summary>
        /// Toggles visibility of the map settings.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butMapSettings_Click(object sender = null, EventArgs e = null)
        {
            butMapSettings.Checked = !butMapSettings.Checked;

            mapSettings.Visible = butMapSettings.Checked;

            ArrangeControls();
        }

        /// <summary>
        /// Toggles visibility of the status overview.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void butStatus_Click(object sender = null, EventArgs e = null)
        {
            butStatus.Checked = !butStatus.Checked;

            status.Visible = butStatus.Checked;

            ArrangeControls();
        }

        /// <summary>
        /// Called whenever the selected points change
        /// </summary>
        /// <param name="points"></param>
        public void UpdateSelectedPoints(Point[] points)
        {
            // ensure data is valid
            if (points.Length != 2)
                throw new ArgumentException();

            // update data
            _selectedPoints = points;

            // the user can only move entities if just one point is selected
            for (var i = 0; i < (int)EntityType.Count; i++)
                _mapCellContextMenu.MenuItems["Entity" + i].Enabled = points[0] == points[1];
        }

        #endregion
    }
}
