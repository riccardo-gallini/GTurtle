using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Cyotek.Windows.Forms;
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
                       
        }

        public Canvas DrawingCanvas
        {
            get
            {
                return canvasControl.drawingCanvas;
            }
        }

        public void SetDrawingCanvasSize(SurfaceSize sz)
        {
            surfaceSize = sz;
            canvasControl.canvasBorder.Height = surfaceSize.Height;
            canvasControl.canvasBorder.Width = surfaceSize.Width;
            Clear();
        }

        public SurfaceSize GetDrawingCanvasSize()
        {
            return surfaceSize;
        }

        public void Clear()
        {
            canvasControl.drawingCanvas.Children.Clear();
        }
              
    }
}
