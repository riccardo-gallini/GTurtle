using Cyotek.Windows.Forms;
using GScripting;
using GScripting.CodeEditor;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace GTurtle
{
    public partial class MainForm : Form
    {
        public readonly static Color COLOR_ORANGE = Color.FromArgb(202, 81, 0);
        public readonly static Color COLOR_BLUE = Color.FromArgb(0, 122, 204);
        public readonly static Color COLOR_RED = Color.FromArgb(193, 0, 0);

        public GScripting.Engine Engine;

        public CodeEditorWindow codeEditorWindow;
        public SurfaceWindow surfaceWindow;
        public OutputWindow outputWindow;
        public CodeErrorsWindow codeErrorsWindow;
        public WatchWindow watchWindow;

        //
        private EditorControl editor;
        private ImageBox surface;
        private ParserService parserService;

        //
        private WorkbenchStatus _status;
        private ExecutionContext executionContext;

        private string script_filename = "";

        public MainForm()
        {
            InitializeComponent();

            //the script engine
            Engine = new GScripting.Engine();

            //the docking component and windows
            codeEditorWindow = new CodeEditorWindow(this);
            surfaceWindow = new SurfaceWindow(this);
            outputWindow = new OutputWindow(this);
            codeErrorsWindow = new CodeErrorsWindow(this);
            watchWindow = new WatchWindow(this);

            surfaceWindow.Show(dockPanel, DockState.Document);
            outputWindow.Show(dockPanel, DockState.DockBottom);
            codeEditorWindow.Show(dockPanel, DockState.Document);
            codeErrorsWindow.Show(dockPanel, DockState.DockBottom);
            watchWindow.Show(dockPanel, DockState.DockBottom);

            //editor stuff
            editor = codeEditorWindow.Editor;
            surface = surfaceWindow.ImageBox;

            //surface sizes
            cmbSurfaceSize.Items.AddRange(SurfaceSize.List().ToArray());
            cmbSurfaceSize.SelectedIndex = 0;

            //parser service stuff
            parserService = Engine.CreateParserService();
            parserService.GetSource = getSourceUI;
            parserService.ParseFinished = parseFinishedUI;

            //setup workbench
            setWorkbenchStatusUI(WorkbenchStatus.Editing);

        }

        #region " WORKBENCH MANAGEMENT "

        void setWorkbenchStatusUI(WorkbenchStatus status)
        {
            if (status == _status) { return; }

            if (status == WorkbenchStatus.Editing)
            {
                editor.IsReadOnly = false;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = true;
                this.btnPlay.Enabled = true;
                this.btnStop.Enabled = false;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = true;
                this.btnStepOver.Enabled = true;
                this.btnStepOut.Enabled = false;

                parserService.Start();

                editor.RemoveAllDebugMarks();
                watchWindow.Clear();

                this.statusStrip.BackColor = COLOR_BLUE;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_BLUE;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Editing";

            }
            else if (status == WorkbenchStatus.Running)
            {
                editor.IsReadOnly = true;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = false;
                this.btnPlay.Enabled = false;
                this.btnStop.Enabled = true;
                this.btnPause.Enabled = true;
                this.btnStepInto.Enabled = false;
                this.btnStepOver.Enabled = false;
                this.btnStepOut.Enabled = false;

                parserService.Stop();

                editor.RemoveAllDebugMarks();

                this.statusStrip.BackColor = COLOR_ORANGE;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_ORANGE;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Running";

            }
            else if (status == WorkbenchStatus.Paused)
            {
                editor.IsReadOnly = true;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = false;
                this.btnPlay.Enabled = true;
                this.btnStop.Enabled = true;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = true;
                this.btnStepOver.Enabled = true;
                this.btnStepOut.Enabled = true;

                parserService.Stop();

                this.statusStrip.BackColor = COLOR_ORANGE;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_ORANGE;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Paused";

            }
            else if (status == WorkbenchStatus.ExecutionError)
            {
                editor.IsReadOnly = true;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = false;
                this.btnPlay.Enabled = false;
                this.btnStop.Enabled = true;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = false;
                this.btnStepOver.Enabled = false;
                this.btnStepOut.Enabled = false;

                parserService.Stop();

                this.statusStrip.BackColor = COLOR_RED;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_RED;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Execution Error";

            }
            else if (status == WorkbenchStatus.ParserError)
            {
                editor.IsReadOnly = false;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = true;
                this.btnPlay.Enabled = false;
                this.btnStop.Enabled = false;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = false;
                this.btnStepOver.Enabled = false;
                this.btnStepOut.Enabled = false;

                parserService.Start();

                editor.RemoveAllDebugMarks();
                watchWindow.Clear();

                this.statusStrip.BackColor = COLOR_RED;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_RED;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Editing - Error";

            }

            _status = status;
        }


        #endregion
        
        private string getSourceUI()
        {
            string source = "";
            this.InvokeAction(() => source = editor.Text);
            return source;
        }


        private IBreakpoint getBreakpointUI(int line)
        {
            return this.InvokeFunc(() => editor.GetBreakpoint(line));
        }

        private void parseFinishedUI(ParseInfo info)
        {
            this.InvokeAction(
                () =>
                {
                    codeErrorsWindow.ShowErrors(info.Errors);
                    codeEditorWindow.MarkErrors(info.Errors);

                    if (_status == WorkbenchStatus.Editing || _status == WorkbenchStatus.ParserError)
                    {
                        if (info.Errors.Count > 0)
                        {
                            setWorkbenchStatusUI(WorkbenchStatus.ParserError);
                        }
                        else
                        {
                            setWorkbenchStatusUI(WorkbenchStatus.Editing);
                        }
                    }

                });
        }
        
        #region " PLAY & DEBUG "

        private async void btnPlay_Click(object sender, EventArgs e)
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await Play(false);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                executionContext.Continue();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_status == WorkbenchStatus.Running)
            {
                executionContext.Stop();
            }
            else if (_status == WorkbenchStatus.ExecutionError)
            {
                setWorkbenchStatusUI(WorkbenchStatus.Editing);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            executionContext.RequestPause();
        }

        private async void btnStepInto_Click(object sender, EventArgs e)
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await Play(true);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                executionContext.StepInto();
            }

        }

        private async void btnStepOver_Click(object sender, EventArgs e)
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await Play(true);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                executionContext.StepOver();
            }
        }

        private void btnStepOut_Click(object sender, EventArgs e)
        {
            executionContext.StepOut();
        }


        public async Task Play(bool requestPause)
        {
            setWorkbenchStatusUI(WorkbenchStatus.Running);

            using (var g = Graphics.FromImage(surface.Image))
            {
                g.Clear(surfaceWindow.ImageBox.BackColor);

                //prepare the turtle
                var turtle = new Turtle(g, surface.Image.Size, surfaceWindow.ImageBox);

                executionContext = Engine.CreateExecutionContext();
                executionContext.Source = editor.Text;
                executionContext.RegisterCommands(turtle.GetCommands());
                executionContext.RegisterOnCheckBreakpoint(getBreakpointUI);
                executionContext.RegisterDebuggerStep(debuggerUpdateUI);
                executionContext.RegisterOnOutput(outputUpdateUI);
                executionContext.RegisterOnError(errorUpdateUI);
                executionContext.RegisterOnScriptEnd(scriptEndUpdateUI);
                
                                
                if (requestPause) { executionContext.RequestPause(); }

                //exec the script
                await executionContext.RunAsync();
            }

            surfaceWindow.ImageBox.Invalidate();
          
        }


        private void outputUpdateUI(string value)
        {
            this.InvokeAction(
                () =>
                    {
                        outputWindow.OutputBox.AppendText(value);
                    });
        }
        
        private void debuggerUpdateUI(DebugInfo info)
        {
            this.InvokeAction(
                () =>
                    {
                        editor.MarkCurrentDebugLine(info.CurrentLine, isError:false);
                        surfaceWindow.ImageBox.Invalidate();
                        watchWindow.ShowScope(info.ExecutionScope);
                        setWorkbenchStatusUI(WorkbenchStatus.Paused);
                    });
        }

        private void errorUpdateUI(DebugInfo info)
        {
            this.InvokeAction(
                () =>
                {
                    editor.MarkCurrentDebugLine(info.CurrentLine, info.IsError, message:info.Message);
                    surfaceWindow.ImageBox.Invalidate();
                    watchWindow.ShowScope(info.ExecutionScope);
                    setWorkbenchStatusUI(WorkbenchStatus.ExecutionError);
                });
        }

        private void scriptEndUpdateUI(DebugInfo info)
        {
            this.InvokeAction( () => setWorkbenchStatusUI(WorkbenchStatus.Editing) );
        }

        #endregion

        #region " FILE MANAGEMENT "

        private void btnNewScript_Click(object sender, EventArgs e)
        {
            if (check_modified())
            {
                editor.Clear();
                script_filename = "";
            }
        }

        private void btnOpenScript_Click(object sender, EventArgs e)
        {
            if (check_modified())
            {
                var file_dialog = new OpenFileDialog();
                file_dialog.CheckFileExists = true;
                file_dialog.CheckPathExists = true;
                file_dialog.AddExtension = true;
                file_dialog.DefaultExt = "*.py";
                file_dialog.Multiselect = false;
                var resp = file_dialog.ShowDialog();

                if (resp == DialogResult.OK)
                {
                    editor.Load(file_dialog.FileName);
                }

            }
        }

        private bool check_modified()
        {
            if (editor.IsModified)
            {
                var resp = MessageBox.Show("Want to save your changes?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (resp == DialogResult.Yes)
                {
                    save();
                }
                else if (resp == DialogResult.No)
                {
                    return true;
                }

                return false;

            }

            return true;

        }

        private void save()
        {
            if (script_filename == "")
            {
                var file_dialog = new SaveFileDialog();
                file_dialog.CheckFileExists = false;
                file_dialog.CheckPathExists = true;
                file_dialog.AddExtension = true;
                file_dialog.DefaultExt = "*.py";
                file_dialog.OverwritePrompt = true;
                var resp = file_dialog.ShowDialog();

                if (resp == DialogResult.OK)
                {
                    script_filename = file_dialog.FileName;
                }
            }

            if (script_filename != "")
            {
                editor.Save(script_filename);
            }
        }

        private void btnSaveScript_Click(object sender, EventArgs e)
        {
            save();
        }

        #endregion

        #region " SURFACE "

        private void cmbSurfaceSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSurfaceSize.SelectedItem != null)
            {
                var sz = (SurfaceSize)cmbSurfaceSize.SelectedItem;
                var img = new Bitmap(sz.ActualSize.Width, sz.ActualSize.Height);
                
                surfaceWindow.ImageBox.Image = img;

                using (var g = Graphics.FromImage(surface.Image))
                {
                    g.Clear(surfaceWindow.ImageBox.BackColor);
                }

                surfaceWindow.ImageBox.Invalidate(); // Trigger redraw of the control.

            }

    }

    #endregion

}

    public enum WorkbenchStatus
    {
        Editing = 1,
        Running = 2,
        Paused = 3,
        ExecutionError = 4,
        ParserError = 5
    }
}
