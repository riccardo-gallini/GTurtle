using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using GScripting;

namespace GTurtle
{
    public partial class CodeErrorsWindow : DockContent
    {
        private MainForm _mainForm;

        public CodeErrorsWindow(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;
            list.DoubleClick += List_DoubleClick;
        }

        private void List_DoubleClick(object sender, System.EventArgs e)
        {
            var focusedItem = list.FocusedItem;
            if (focusedItem != null)
            {
                var line = int.Parse(focusedItem.SubItems[0].Text);
                _mainForm.codeEditorWindow.NavigateToLine(line);
            }
            

        }

        public void ShowErrors(IList<ScriptError> errorList)
        {
            list.Items.Clear();

            foreach(var err in errorList)
            {
                string[] row = { err.Line.ToString(), err.Message, err.Severity.ToString() };
                list.Items.Add(new ListViewItem(row));
            }
        }
               
    }
}
