using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTurtle.Surface
{
    /// <summary>
    /// Interaction logic for TurtleCanvas.xaml
    /// </summary>
    public partial class TurtleCanvas : UserControl
    {
        public TurtleCanvas()
        {
            InitializeComponent();
            this.drawingCanvas.MouseMove += DrawingCanvas_MouseMove;
        }

        public Action<Point> ReportMouseMove = null;

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            ReportMouseMove?.Invoke(e.GetPosition(this.drawingCanvas));
        }

        public void Clear()
        {
            this.drawingCanvas.Children.Clear();
        }

        public void SetDrawingCanvasSize(double height, double width)
        {
            this.canvasBorder.Height = height;
            this.canvasBorder.Width = width;

            this.zoomBorder.Initialize(this.canvasBorder);

            Clear();
        }
    }
}
