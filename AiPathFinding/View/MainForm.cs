using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AiPathFinding.Model;

namespace AiPathFinding.View
{
    public partial class MainForm : Form
    {
        #region Fields

        private const string AutosaveMapName = "autosave.map";

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

        #endregion

        #region Events

        public delegate void OnCellClicked(Point location, MouseButtons button);

        public event OnCellClicked CellClicked;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

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
            Map.CellTypeChanged += OnCellTypeChanged;
            Map.MapSizeChanged += OnMapSizeChanged;
            Map.EntityNodeChanged += OnEntityNodeChanged;
            Map.MapLoaded += OnMapLoaded;
            // track user input
            _canvas.Click += OnClick;
            mapSettings.butLoadMap.Click += (s, e) => LoadMap();
            mapSettings.butSaveMap.Click += (s, e) => SaveMap();

            algorithmSettings1.RegisterMap(Map);
            
            // prepare GUI (depending on whether map was loaded
            if (File.Exists(AutosaveMapName))
                OnMapLoaded(); 
            else
                SetCanvasSize();
        }

        #endregion

        #region Methods

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

        private Rectangle MapPointToCanvasRectangle(Point point)
        {
            return new Rectangle(new Point(point.X * mapSettings.CellSize + 1, point.Y * mapSettings.CellSize + 1), new Size(mapSettings.CellSize - 1, mapSettings.CellSize - 1));
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
            }
        }

        private void DrawEntities(Graphics g)
        {
            foreach (var e in Entity.Entities.Where(ee => ee.IsVisible))
                g.FillEllipse(e.Brush, MapPointToCanvasRectangle(new Point(e.Node.Location.X, e.Node.Location.Y)));
        }

        private void DrawLandscape(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    g.FillRectangle(_landscapeBrushes[(int)Map.GetNodeType(new Point(i, j))], MapPointToCanvasRectangle(new Point(i, j)));
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

        private void OnEntityNodeChanged(Node oldnode, Node newnode, Entity entity)
        {
            switch (entity.Type)
            {
                case EntityType.Player:
                    status.PlayerPosition = newnode.Location;
                    break;
                case EntityType.Target:
                    status.TargetPosition = newnode.Location;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _canvas.Invalidate();
        }

        private void OnCellSizeChanged(int cellSize)
        {
            SetCanvasSize();

            _canvas.Invalidate();
        }

        private void OnCellTypeChanged(Point location, NodeType oldType, NodeType newType)
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
            mapSettings.SetMapSize(Map.Width, Map.Height);
            SetCanvasSize();

            _canvas.Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DoubleBuffered = true;
        }

        private void OnClick(object sender, EventArgs e)
        {
            var me = e as MouseEventArgs;

            if (me == null)
                throw new Exception("e should be me...");

            if (IsPointOnGrid(me.Location)) return;

            CellClicked(CanvasPointToMapPoint(me.Location), me.Button);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Map.SaveMap(AutosaveMapName);
        }

        #endregion
    }
}
