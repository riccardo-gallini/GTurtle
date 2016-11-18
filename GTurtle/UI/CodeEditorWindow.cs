using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using GScripting;
using GScripting.CodeEditor;

namespace GTurtle
{
    public partial class CodeEditorWindow : DockContent
    {
        public EditorControl Editor;
        public ElementHost wpf_host;

        private MainForm _mainForm;

        public CodeEditorWindow(MainForm mainForm)
        {
            InitializeComponent();

            this._mainForm = mainForm;

            wpf_host = new ElementHost();
            this.Controls.Add(wpf_host);
            wpf_host.Dock = DockStyle.Fill;

            //THIS IS NEEDED IF HOSTED INSIDE MDI!!
            wpf_host.TabStop = false;

            Editor = new EditorControl();
            wpf_host.Child = Editor;
                        
        }
               

        public void MarkErrors(IList<ScriptError> errorList)
        {
            Editor.RemoveAllErrorMarks();

            foreach(var err in errorList)
            {
                Editor.MarkError(err.SpanStartIndex, err.SpanLength, err.Message);
            }
        }
              
        public void NavigateToLine(int line)
        {
            Editor.NavigateToLine(line);
            Editor.Focus();
        }

    }
}
