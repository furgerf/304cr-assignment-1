using System;
using System.Drawing;
using System.Windows.Forms;

namespace AiPathFinding
{
    public partial class MainForm : Form
    {
        // constants
        private const int GridSize = 20;

        // pens
        private readonly Pen _gridPen = new Pen(Color.LightGray, 1);
        private readonly Pen _startPen = new Pen(Color.Blue, 1);
        private readonly Pen _goalPen = new Pen(Color.Red, 1);

        private readonly Brush _streetBrush = Brushes.Gray;
        private readonly Brush _plainsBrush = Brushes.SandyBrown;
        private readonly Brush _forestBrush = Brushes.Green;
        private readonly Brush _hillBrush = Brushes.SaddleBrown;
        private readonly Brush _mountainBrush = Brushes.Black;
        private readonly Brush[] _landscapeBrushes;

        public delegate void OnCellClicked(Point location);

        public static event OnCellClicked CellClicked;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }
        public MainForm()
        {
            InitializeComponent();

            _landscapeBrushes = new[] {_streetBrush, _plainsBrush, _forestBrush, _hillBrush, _mountainBrush};

            Model = new Model(Graph.EmptyGraph(mapSettings.MapWidth, mapSettings.MapHeight));

            _canvas.Size = new Size(GridSize * mapSettings.MapWidth + 3, GridSize * mapSettings.MapHeight + 3);

            Controller = new Controller(Model);

            _canvas.Paint += DrawMap;

            Model.CellTypeChanged += OnCellTypeChanged;
            Model.MapSizeChanged += OnMapSizeChanged;

            mapSettings.MapSizeChanged += Controller.OnMapSizeChanged;

            // for controller
            _canvas.Click += OnClick;
        }

        private void OnCellTypeChanged(Point location, NodeType oldType, NodeType newType)
        {
            _canvas.Invalidate();
        }
        private void OnMapSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            _canvas.Size = new Size(GridSize * newWidth+ 3, GridSize * newHeight + 3);

            _canvas.Invalidate();
        }

        private Rectangle MapPointToCanvasRectangle(Point point)
        {
            return new Rectangle(new Point(point.X * GridSize + 1, point.Y * GridSize + 1), new Size(GridSize - 1, GridSize - 1));
        }

        private Point CanvasPointToMapPoint(Point point)
        {
            return new Point(point.X / GridSize, point.Y / GridSize);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DoubleBuffered = true;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (!IsPointOnGrid(((MouseEventArgs) e).Location))
                CellClicked(CanvasPointToMapPoint(((MouseEventArgs)e).Location));
        }

        private bool IsPointOnGrid(Point location)
        {
            return location.X%GridSize == 0 || location.Y%GridSize == 0;
        }

        public readonly Model Model;

        public readonly Controller Controller;

        private void DrawMap(object sender, PaintEventArgs e)
        {
            using (var g = e.Graphics)
            {
                DrawGrid(g);
                DrawLandscape(g);
            }
        }

        private void DrawLandscape(Graphics g)
        {
            for (var i = 0; i < mapSettings.MapWidth; i++)
                for (var j = 0; j < mapSettings.MapHeight; j++)
                    g.FillRectangle(_landscapeBrushes[(int)Model.GetNodeType(new Point(i, j))], MapPointToCanvasRectangle(new Point(i, j)));
        }

        private void DrawGrid(Graphics g)
        {
            for (var i = 0; i < _canvas.Height; i += GridSize)
                g.DrawLine(_gridPen, 0, i, _canvas.Width, i);

            for (var i = 0; i < _canvas.Width; i += GridSize)
                g.DrawLine(_gridPen, i, 0, i, _canvas.Height);
        }
    }
}
