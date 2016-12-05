using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Output;
using GScripting;
using GTurtle.Commands;
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
        public IOutput Output { get; private set; }
        public IErrorList ErrorList { get; private set; }
        public WatchViewModel Watch { get; private set; }
        
        //TODO: move away from here ==> have an all new turtle module
        public TurtleAreaViewModel TurtleArea { get; private set; }

        public override void Initialize()
        {
            //create scripting engine
            GScriptEngine = new Engine();

            //status and tool bars
            StatusBar = new StatusBar();
            StatusBar.Initialize();
                           
            Shell.ToolBars.Visible = true;

            //show error window by default
            ErrorList = IoC.Get<IErrorList>();
            Shell.ShowTool(ErrorList);

            //show output window by default
            Output = IoC.Get<IOutput>();
            Shell.ShowTool(Output);

            //watch tool
            Watch = IoC.Get<WatchViewModel>();
            Shell.ShowTool(Watch);

            //show a new doc
            var doc = new ScriptViewModel();
            doc.New("Untitled.py");
            Shell.OpenDocument(doc);

            //show the turtle surface
            TurtleArea = IoC.Get<TurtleAreaViewModel>();
            Shell.ShowTool(TurtleArea);
            
            MainWindow.Title = "GTurtle";
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
            return new Turtle(TurtleArea);
        }
        
    }


}
