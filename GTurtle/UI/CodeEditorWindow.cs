using WeifenLuo.WinFormsUI.Docking;
using System.Collections.Generic;
using GScripting;
using GScripting.CodeEditor;
using System.Windows;
using System.Windows.Forms;
using System;

namespace GTurtle
{
    public partial class CodeEditorWindow : DockContent
    {
        public EditorControl editor;
        public KeyPreviewElementHost wpf_host;

        private MainForm _mainForm;
        private string script_file_name = "";
        
        public CodeEditorWindow(MainForm mainForm)
        {
            InitializeComponent();

            this._mainForm = mainForm;

            wpf_host = new KeyPreviewElementHost();
            this.Controls.Add(wpf_host);
            wpf_host.Dock = DockStyle.Fill;

            //THIS IS NEEDED IF HOSTED INSIDE MDI!!
            wpf_host.TabStop = false;

            editor = new EditorControl();
            wpf_host.Child = editor;

            //manage dropping image files on code text in order to import an image command
            editor.RegisterDropDataFormat(System.Windows.DataFormats.FileDrop);
            editor.GetDropData = getTextFromDataObject;            
            
            editor.TextChanged += Editor_TextChanged;
            
            updateWindowCaption();
                        
        }

        private string getTextFromDataObject(DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] data = (string[])e.DataObject.GetData(System.Windows.DataFormats.FileDrop);

                if (data.Length > 0)
                {
                    var uri = new System.Uri(data[0]);
                    return "image(\"" + uri.AbsoluteUri + "\")";
                }
            }
            return null;
        }

       
       
        private void Editor_TextChanged(object sender, System.EventArgs e)
        {
            updateWindowCaption();
        }

        public void MarkErrors(IList<ScriptError> errorList)
        {
            editor.RemoveAllErrorMarks();

            foreach(var err in errorList)
            {
                editor.MarkError(err.SpanStartIndex, err.SpanLength, err.Message);
            }
        }
              
        public void NavigateToLine(int line)
        {
            editor.NavigateToLine(line);
            editor.Focus();
        }

        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(script_file_name);
            }
        }

        private void setFileName(string file_name)
        {
            script_file_name = file_name;
            updateWindowCaption();
        }

        private void updateWindowCaption()
        {
            string caption;
            
            if (this.FileName != "")
            {
                caption = this.FileName;
            }
            else
            {
                caption = "New Script";
            }

            if (this.IsModified)
            {
                caption += "*";
            }

            this.TabText = caption;
            this.Text = caption;
        }

        public void LoadFile(string file_name)
        {
            editor.Load(file_name);
            setFileName(file_name);
        }

        public void SaveFileAs(string file_name)
        {
            editor.Save(file_name);
            setFileName(file_name);
        }

        public void SaveFile()
        {
            if (script_file_name != "")
            {
                editor.Save(script_file_name);
                updateWindowCaption();
            }
        }

        public void NewFile()
        {
            editor.Clear();
            editor.IsModified = false;
            setFileName("");
        }

        public bool IsModified
        {
            get
            {
                return editor.IsModified;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return editor.IsReadOnly;
            }
            set
            {
                editor.IsReadOnly = value;
            }
        }

        public void RemoveAllDebugMarks()
        {
            editor.RemoveAllDebugMarks();
        }

        public void MarkDebugLine(int line, bool isError, string message = "")
        {
            editor.MarkDebugLine(line, isError, message);
        }

        public string EditorText
        {
            get
            {
                return editor.Text;
            }
        }

        public IBreakpoint GetBreakpoint(int line)
        {
            return editor.GetBreakpoint(line);
        }

        public void ToggleBreakpoint()
        {
            editor.ToggleBreakpoint();
        }
    }
}
