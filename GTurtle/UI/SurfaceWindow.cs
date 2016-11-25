using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;
using GTurtle.Surface;
using System.Windows.Controls;

namespace GTurtle
{
    public partial class SurfaceWindow : DockContent
    {
        private TurtleCanvas canvasControl;
        private KeyPreviewElementHost wpf_host;
        private SurfaceSize surfaceSize;

        private MainForm _mainForm;

        public SurfaceWindow(MainForm mainForm)
        {
            InitializeComponent();

            this._mainForm = mainForm;

            wpf_host = new KeyPreviewElementHost();
            this.Controls.Add(wpf_host);
            wpf_host.Dock = DockStyle.Fill;

            //THIS IS NEEDED IF HOSTED INSIDE MDI!!
            wpf_host.TabStop = false;

            canvasControl = new TurtleCanvas();
            wpf_host.Child = canvasControl;

            canvasControl.ReportMouseMove =
                    (point) =>
                    {
                        _mainForm.stMousePosition.Text = "(" + ((int)point.X).ToString() + "," + ((int)point.Y).ToString() + ")";
                    };

        }

        public void Clear()
        {
            canvasControl.Clear();
        }

        public double CanvasHeight { get; private set; }
        public double CanvasWidth { get; private set;  }

        public Canvas DrawingCanvas
        {
            get
            {
                return canvasControl.drawingCanvas;
            }
        }

        public void SetCanvasSize(SurfaceSize sz)
        {
            CanvasHeight = sz.Height;
            CanvasWidth = sz.Width;
            canvasControl.SetDrawingCanvasSize(sz.Height, sz.Width);
        }
        
              
    }
}
