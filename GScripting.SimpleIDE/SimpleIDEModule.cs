using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
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

namespace GScripting.SimpleIDE
{
    [Export(typeof(IModule))]
    [Export(typeof(SimpleIDEModule))]
    public class SimpleIDEModule : ModuleBase

    {
        public Engine GScriptEngine { get; private set; }
        public StatusBar StatusBar { get; private set; }
        public IOutput Output { get; private set; }
        public IErrorList ErrorList { get; private set; }
        public WatchViewModel Watch { get; private set; }
        


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
            

        }
        
        public ParserService CreateParserService()
        {
            return this.GScriptEngine.CreateParserService();
        }

        public ExecutionContext CreateExecutionContext()
        {
            return this.GScriptEngine.CreateExecutionContext();
        }

       
        
    }


}
