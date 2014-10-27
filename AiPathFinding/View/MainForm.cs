﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AiPathFinding.Algorithm;
using AiPathFinding.Model;
using AiPathFinding.Properties;

namespace AiPathFinding.View
{
    public partial class MainForm : Form
    {
        #region Fields

        private const string AutosaveMapName = "autosave.map";

        private Action<Graphics> DrawAlgorithmStep { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                // disable flickering on redraw
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }

        // pens
        private readonly Pen _gridPen = new Pen(Color.LightGray, 1);
        private readonly Pen _fogPen = new Pen(Color.Black, 1);
        private readonly Pen _selectionPen = new Pen(Color.HotPink, 3);
        private readonly Pen _fineSelectionPen = new Pen(Color.LightPink, 1);
        
        // brushes
        private readonly Brush _streetBrush = Brushes.Gray;
        private readonly Brush _plainsBrush = Brushes.SandyBrown;
        private readonly Brush _forestBrush = Brushes.Green;
        private readonly Brush _hillBrush = Brushes.SaddleBrown;
        private readonly Brush _mountainBrush = Brushes.Black;
        private readonly Brush[] _landscapeBrushes;

        // important stuff
        public readonly Map Map;
        public readonly Controller.Controller Controller;
        private static MainForm _instance;

        // stuff for handling cell selection
        private bool _drawSelection;
        private readonly Point[] _selectionRange = new Point[2];
        private Point? _mouseDownLocation;

        #endregion

        #region Events

        public delegate void OnSelectedPointsChanged(Point[] points);

        public event OnSelectedPointsChanged SelectedPointsChanged;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            _instance = this;

            // instantiate objects
            _landscapeBrushes = new[] { _streetBrush, _plainsBrush, _forestBrush, _hillBrush, _mountainBrush };

            Map = File.Exists(AutosaveMapName) ? Map.FromMapFile(AutosaveMapName) : new Map(Graph.EmptyGraph(mapSettings.MapWidth, mapSettings.MapHeight));
            Controller = new Controller.Controller(Map, this, mapSettings);
            
            // register events
            // get paint hook
            _canvas.Paint += DrawMap;
            // track changes in the settings
            mapSettings.CellSizeChanged += OnCellSizeChanged;
            // track changes in the model
            Map.CellTerrainChanged += OnCellTerrainChanged;
            Map.MapSizeChanged += OnMapSizeChanged;
            Map.EntityNodeChanged += OnEntityNodeChanged;
            Map.FogChanged += OnFogChanged;
            Map.MapLoaded += OnMapLoaded;
            // track user input
            _canvas.MouseDown += CanvasOnMouseDown;
            _canvas.MouseUp += CanvasOnMouseUp;
            _canvas.MouseMove += CanvasOnMouseMove;

            mapSettings.butLoadMap.Click += (s, e) => LoadMap();
            mapSettings.butSaveMap.Click += (s, e) => SaveMap();
            status.PlayerPosition = Entity.Player.Node.Location;
            status.TargetPosition = Entity.Target.Node.Location;

            algorithmSettings1.RegisterMap(Map);
            algorithmSettings1.AlgorithmStepChanged += OnAlgorithmStepChanged;

            SelectedPointsChanged += UpdatePointSelection;

            // prepare GUI (depending on whether map was loaded
            if (File.Exists(AutosaveMapName))
                OnMapLoaded(); 
            else
                SetCanvasSize();
        }

        #endregion

        #region Methods

        private Point[] GetPointRange(Point location)
        {
            if (_mouseDownLocation == null)
                throw new Exception();

            return new[]
            {
                new Point(location.X < _mouseDownLocation.Value.X ? location.X : _mouseDownLocation.Value.X,
                    location.Y < _mouseDownLocation.Value.Y ? location.Y : _mouseDownLocation.Value.Y),
                new Point(location.X > _mouseDownLocation.Value.X ? location.X : _mouseDownLocation.Value.X,
                    location.Y > _mouseDownLocation.Value.Y ? location.Y : _mouseDownLocation.Value.Y)
            };
        }

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

            if (dlg.ShowDialog() == DialogResult.OK)
                Map.LoadMap(dlg.FileName);
        }

        private void SetCanvasSize()
        {
            _canvas.Size = new Size(mapSettings.CellSize * mapSettings.MapWidth + 3, mapSettings.CellSize * mapSettings.MapHeight + 3);
        }

        public static Rectangle MapPointToCanvasRectangle(Point point)
        {
            return new Rectangle(new Point(point.X * _instance.mapSettings.CellSize + 1, point.Y * _instance.mapSettings.CellSize + 1), new Size(_instance.mapSettings.CellSize - 1, _instance.mapSettings.CellSize - 1));
        }

        private Point CanvasPointToMapPoint(Point point)
        {
            return new Point(point.X / mapSettings.CellSize, point.Y / mapSettings.CellSize);
        }

        private bool IsPointOnGrid(Point location)
        {
            return location.X % mapSettings.CellSize == 0 || location.Y % mapSettings.CellSize == 0;
        }

        private void DrawMap(object sender, PaintEventArgs e)
        {
            using (var g = e.Graphics)
            {
                DrawGrid(g);
                DrawLandscape(g);
                DrawEntities(g);
                DrawFog(g);
                DrawSelectedCells(g);

                if (DrawAlgorithmStep != null)
                    DrawAlgorithmStep(g);
            }
        }

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

        private void DrawFog(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    if (Map.HasFog(new Point(i, j)))
                    {
                        var rect = MapPointToCanvasRectangle(new Point(i, j));
                        for (var k = rect.Left; k < rect.Right; k += 4)
                            g.DrawLine(_fogPen, k, rect.Top, k, rect.Bottom - 1);
                        for (var k = rect.Top; k < rect.Bottom; k += 4)
                            g.DrawLine(_fogPen, rect.Left, k, rect.Right - 1, k);
                    }  
        }

        private void DrawEntities(Graphics g)
        {
            foreach (var e in Entity.Entities.Where(ee => ee.IsVisible))
                g.DrawIcon(e.Icon, MapPointToCanvasRectangle(new Point(e.Node.Location.X, e.Node.Location.Y)));
        }

        private void DrawLandscape(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    g.FillRectangle(_landscapeBrushes[(int)Map.GetTerrain(new Point(i, j))], MapPointToCanvasRectangle(new Point(i, j)));
        }

        private void DrawGrid(Graphics g)
        {
            for (var i = 0; i < _canvas.Height; i += mapSettings.CellSize)
                g.DrawLine(_gridPen, 0, i, _canvas.Width, i);

            for (var i = 0; i < _canvas.Width; i += mapSettings.CellSize)
                g.DrawLine(_gridPen, i, 0, i, _canvas.Height);
        }

        #endregion

        #region Event Handling

        private void OnFogChanged(Point location, bool hasFog)
        {
            _canvas.Invalidate();
        }

        private void OnEntityNodeChanged(Node oldnode, Node newnode, Entity entity)
        {
            switch (entity.Type)
            {
                case EntityType.Player:
                    if (newnode == null)
                        status.PlayerPosition = null;
                    else
                        status.PlayerPosition = newnode.Location;
                    break;
                case EntityType.Target:
                    if (newnode == null)
                        status.TargetPosition = null;
                    else
                        status.TargetPosition = newnode.Location;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _canvas.Invalidate();
        }

        private void OnAlgorithmStepChanged(AlgorithmStep step)
        {
            DrawAlgorithmStep = step == null ? null : step.DrawStep;

            _canvas.Invalidate();
        }

        private void OnCellSizeChanged(int cellSize)
        {
            SetCanvasSize();

            _canvas.Invalidate();
        }

        private void OnCellTerrainChanged(Point location, Terrain oldType, Terrain newType)
        {
            _canvas.Invalidate();
        }

        private void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            SetCanvasSize();

            _canvas.Invalidate();
        }

        private void OnMapLoaded()
        {
            mapSettings.SetMapSize(Map.Width, Map.Height, (int)Settings.Default["CellSize"]);
            SetCanvasSize();

            _canvas.Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DoubleBuffered = true;
        }

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

        private void UpdatePointSelection(Point[] points)
        {
            _canvas.Invalidate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save map
            Map.SaveMap(AutosaveMapName);

            // save settings
            Settings.Default["CellSize"] = mapSettings.CellSize;
            Settings.Default.Save();
        }

        #endregion
    }
}
