using GScripting;
using GTurtle.Surface;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        
        private ParserService parserService;

        //
        private WorkbenchStatus _status;
        private ExecutionContext executionContext;
        
        public MainForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            
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
            
            //surface sizes
            cmbSurfaceSize.Items.AddRange(SurfaceSize.List().ToArray());
            cmbSurfaceSize.SelectedIndex = 0;

            //parser service stuff
            parserService = Engine.CreateParserService();
            parserService.RegisterGetSource(getSourceUI);
            parserService.RegisterOnParseFinished(parseFinishedUI);

            //setup workbench
            setWorkbenchStatusUI(WorkbenchStatus.Editing);

        }

        #region " WORKBENCH MANAGEMENT "

        void setWorkbenchStatusUI(WorkbenchStatus status)
        {
            if (status == _status) { return; }

            if (status == WorkbenchStatus.Editing)
            {
                codeEditorWindow.IsReadOnly = false;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = true;
                this.btnPlay.Enabled = true;
                this.btnStop.Enabled = false;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = true;
                this.btnStepOver.Enabled = true;
                this.btnStepOut.Enabled = false;

                parserService.Start();

                codeEditorWindow.RemoveAllDebugMarks();
                watchWindow.Clear();

                this.statusStrip.BackColor = COLOR_BLUE;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_BLUE;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Editing";

            }
            else if (status == WorkbenchStatus.Running)
            {
                codeEditorWindow.IsReadOnly = true;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = false;
                this.btnPlay.Enabled = false;
                this.btnStop.Enabled = true;
                this.btnPause.Enabled = true;
                this.btnStepInto.Enabled = false;
                this.btnStepOver.Enabled = false;
                this.btnStepOut.Enabled = false;

                parserService.Stop();

                codeEditorWindow.RemoveAllDebugMarks();

                this.statusStrip.BackColor = COLOR_ORANGE;
                this.statusStrip.ForeColor = Color.White;
                this.stLabel.BackColor = COLOR_ORANGE;
                this.stLabel.ForeColor = Color.White;
                this.stLabel.Text = "Running";

            }
            else if (status == WorkbenchStatus.Paused)
            {
                codeEditorWindow.IsReadOnly = true;
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
                codeEditorWindow.IsReadOnly = true;
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
                codeEditorWindow.IsReadOnly = false;
                this.btnSaveScript.Enabled = true;
                this.btnOpenScript.Enabled = true;
                this.btnPlay.Enabled = false;
                this.btnStop.Enabled = false;
                this.btnPause.Enabled = false;
                this.btnStepInto.Enabled = false;
                this.btnStepOver.Enabled = false;
                this.btnStepOut.Enabled = false;

                parserService.Start();

                codeEditorWindow.RemoveAllDebugMarks();
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

        #region " PARSER "
        
        private string getSourceUI()
        {
            return this.InvokeFunc(() => codeEditorWindow.EditorText);
        }
        
        private void parseFinishedUI(ParseInfo info)
        {
            this.InvokeAction(
                () =>
                {
                    codeErrorsWindow.ShowErrors(info.Errors);

                    codeEditorWindow.SetParseInfo(info);

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

        #endregion

        #region " TOOLBAR "

        private async void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                await PlayUI();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F10)
            {
                await StepOverUI();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F11)
            {
                await StepIntoUI();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F9)
            {
                codeEditorWindow.ToggleBreakpoint();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.S && e.Control)
            {
                save();
                e.Handled = true;
            }
        }
        
        private async void btnPlay_Click(object sender, EventArgs e)
        {
            await PlayUI();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_status == WorkbenchStatus.Running || _status == WorkbenchStatus.Paused)
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
            await StepIntoUI();
        }

        private async void btnStepOver_Click(object sender, EventArgs e)
        {
            await StepOverUI();
        }

        private void btnStepOut_Click(object sender, EventArgs e)
        {
            setWorkbenchStatusUI(WorkbenchStatus.Running);
            executionContext.StepOut();
        }

        #endregion

        #region " PLAY & DEBUG "

        private async Task StepIntoUI()
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await doPlay(true);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                codeEditorWindow.RemoveAllDebugMarks();
                executionContext.StepInto();
            }
        }

        private async Task StepOverUI()
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await doPlay(true);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                setWorkbenchStatusUI(WorkbenchStatus.Running);
                executionContext.StepOver();
            }
        }

        private async Task PlayUI()
        {
            if (_status == WorkbenchStatus.Editing)
            {
                await doPlay(false);
            }
            else if (_status == WorkbenchStatus.Paused)
            {
                setWorkbenchStatusUI(WorkbenchStatus.Running);
                executionContext.Continue();
            }
        }

        private async Task doPlay(bool requestPause)
        {
            setWorkbenchStatusUI(WorkbenchStatus.Running);
            
            surfaceWindow.Clear();  
            
            executionContext = Engine.CreateExecutionContext();
            executionContext.RegisterGetSource(getSourceUI);
            executionContext.RegisterCommand("createturtle", new Func<Turtle>(createTurtle));
            executionContext.RegisterOnCheckBreakpoint(getBreakpointUI);
            executionContext.RegisterDebuggerStep(debuggerUpdateUI);
            executionContext.RegisterOnOutput(outputUpdateUI);
            executionContext.RegisterOnError(errorUpdateUI);
            executionContext.RegisterOnScriptEnd(scriptEndUpdateUI);
            executionContext.RegisterOnStop(scriptEndUpdateUI);
                                
            if (requestPause) { executionContext.RequestPause(); }

            //exec the script
            await executionContext.RunAsync();
                      
        }

        private Turtle createTurtle()
        {
            return new Turtle(surfaceWindow.DrawingCanvas, surfaceWindow.GetDrawingCanvasSize());
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
                        codeEditorWindow.MarkDebugLine(info.CurrentLine, isError:false);
                        watchWindow.ShowScope(info.ExecutionScope);
                        setWorkbenchStatusUI(WorkbenchStatus.Paused);
                    });
        }

        private IBreakpoint getBreakpointUI(int line)
        {
            return this.InvokeFunc(() => codeEditorWindow.GetBreakpoint(line));
        }

        private void errorUpdateUI(DebugInfo info)
        {
            this.InvokeAction(
                () =>
                {
                    codeEditorWindow.MarkDebugLine(info.CurrentLine, info.IsError, message:info.Message);
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
                codeEditorWindow.NewFile();
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
                file_dialog.AutoUpgradeEnabled = true;
                file_dialog.Filter = "Python Scripts (*.py)|*.py|Any file (*.*)|*.*";
                file_dialog.ValidateNames = true;
                file_dialog.Multiselect = false;
                var resp = file_dialog.ShowDialog();

                if (resp == DialogResult.OK)
                {
                    codeEditorWindow.LoadFile(file_dialog.FileName);
                }

            }
        }

        private bool check_modified()
        {
            if (codeEditorWindow.IsModified)
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
            if (codeEditorWindow.FileName == "")
            {
                var file_dialog = new SaveFileDialog();
                file_dialog.CheckFileExists = false;
                file_dialog.CheckPathExists = true;
                file_dialog.AddExtension = true;
                file_dialog.DefaultExt = "*.py";
                file_dialog.AutoUpgradeEnabled = true;
                file_dialog.Filter = "Python Scripts (*.py)|*.py|Any file (*.*)|*.*";
                file_dialog.ValidateNames = true;
                file_dialog.OverwritePrompt = true;
                var resp = file_dialog.ShowDialog();

                if (resp == DialogResult.OK)
                {
                    codeEditorWindow.SaveFileAs(file_dialog.FileName);
                }
            }

            if (codeEditorWindow.FileName != "")
            {
                codeEditorWindow.SaveFile();
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
                surfaceWindow.SetDrawingCanvasSize(sz);
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
