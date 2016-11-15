using WeifenLuo.WinFormsUI.Docking;

namespace GTurtle
{
    public partial class OutputWindow : DockContent
    {
        private MainForm _mainForm;

        public OutputWindow(MainForm mainForm)
        {
            InitializeComponent();

            this._mainForm = mainForm;
        }
    }
}
