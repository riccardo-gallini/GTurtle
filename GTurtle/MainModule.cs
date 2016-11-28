using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Output;
using Gemini.Modules.StatusBar;
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
    [Export(typeof(MainModule))]
    public class MainModule : ModuleBase
    {
        public Engine GScriptEngine;

       
        public override void Initialize()
        {
            GScriptEngine = new Engine();
            
            var statusBar = IoC.Get<IStatusBar>();
            
            //statusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
            statusBar.AddItem("Ln 44", new GridLength(100));
            statusBar.AddItem("Col 79", new GridLength(100));
            

            Shell.ToolBars.Visible = true;
            
            //Shell.ToolBars.Items.Add(new ToolBarModel
            //{
            //    new ToolBarItem("Open", OpenFile).WithIcon(),
            //    ToolBarItemBase.Separator,
            //    new UndoToolBarItem(),
            //    new RedoToolBarItem()
            //});




            var errorList = IoC.Get<IErrorList>();
            Shell.ShowTool(errorList);

            var output = IoC.Get<IOutput>();
            Shell.ShowTool(output);
        }

        public ParserService CreateParserService()
        {
            return this.GScriptEngine.CreateParserService();
        }
                
        public bool canExecute()
        {
            return true;
        }

    }
}
