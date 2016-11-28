using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Output;
using GScripting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTurtle
{
    [Export(typeof(IModule))]
    [Export(typeof(ScriptRunnerModule))]
    public class ScriptRunnerModule : ModuleBase
    {
        public Engine GScriptEngine { get; private set; }
        public StatusBar StatusBar { get; private set; }
        public ScriptRunner ScriptRunner { get; private set; }
        public IOutput Output { get; private set; }
        public IErrorList ErrorList { get; private set; }
        
        public WorkbenchStatus CurrentStatus { get; set; }

        public override void Initialize()
        {
            Shell.ActiveDocumentChanged += Shell_ActiveDocumentChanged;

            //create scripting engine
            GScriptEngine = new Engine();

            //create the script runner (will handle the script execution UI)
            ScriptRunner = new GTurtle.ScriptRunner(this);

            //status and tool bars
            StatusBar = new StatusBar();
            StatusBar.Initialize();
            GlobalStatus = GlobalStatus.Editing;
                
            Shell.ToolBars.Visible = true;

            //show error window by default
            ErrorList = IoC.Get<IErrorList>();
            Shell.ShowTool(ErrorList);

            //show output window by default
            Output = IoC.Get<IOutput>();
            Shell.ShowTool(Output);
                        
        }
        
        private void Shell_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (Shell.ActiveItem is ScriptEditorViewModel)
            {
                _activeScript = (ScriptEditorViewModel)Shell.ActiveItem;
            }
        }

        private ScriptEditorViewModel _activeScript;
        public ScriptEditorViewModel ActiveScript
        {
            get
            {
                return _activeScript;
            }
        }

        private GlobalStatus globalStatus;
        public GlobalStatus GlobalStatus
        {
            get
            {
                return globalStatus;                
            }
            set
            {
                globalStatus = value;
                StatusBar.SetGlobalStatusMessage(globalStatus);
            }
        }

        public ParserService CreateParserService()
        {
            return this.GScriptEngine.CreateParserService();
        }

        public ExecutionContext CreateExecutionContext()
        {
            return this.GScriptEngine.CreateExecutionContext();
        }

        public Turtle CreateTurtle()
        {
            return new Turtle(SurfaceWindow.DrawingCanvas, SurfaceWindow.CanvasHeight, SurfaceWindow.CanvasWidth);
        }
    }

    public enum GlobalStatus
    {
        Editing = 1,
        Running = 2,
        Paused = 3,
        ExecutionError = 4
    }
}
