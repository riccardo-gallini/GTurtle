using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using GScripting;

namespace GTurtle
{
    public partial class WatchWindow : DockContent
    {
        private MainForm _mainForm;

        public WatchWindow(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }
        
        public void ShowScope(ExecutionContext execScope)
        {
            Clear();
            
            foreach(var v in execScope.GetVariables())
            {
                string[] row = { v.Name, v.Value.ToString(), v.Type.ToString() };
                list.Items.Add(new ListViewItem(row));
            }
        }

        public void Clear()
        {
            list.Items.Clear();
        }
               
    }
}
