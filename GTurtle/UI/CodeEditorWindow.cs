using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using MResolver.UI.CodeEditor;
using System.Windows.Forms.Integration;
using GScripting;
using System.Collections.Generic;

namespace GTurtle
{
    public partial class CodeEditorWindow : DockContent
    {
        public ResolverCodeEditor Editor;
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

            Editor = new ResolverCodeEditor();
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
