using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Cyotek.Windows.Forms;
using System.Drawing;

namespace GTurtle
{
    public partial class SurfaceWindow : DockContent
    {
        public ImageBox ImageBox;

        private MainForm _mainForm;

        public SurfaceWindow(MainForm mainForm)
        {
            InitializeComponent();

            this._mainForm = mainForm;

            ImageBox = new ImageBox();
            this.Controls.Add(ImageBox);
            ImageBox.Dock = DockStyle.Fill;
                       

            //style
            ImageBox.ImageBorderColor = Color.Black;
            ImageBox.ImageBorderStyle = ImageBoxBorderStyle.FixedSingle;
        }
                
    }
}
