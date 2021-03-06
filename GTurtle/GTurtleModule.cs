﻿using Caliburn.Micro;
using Gemini.Framework;
using GScripting.SimpleIDE;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    [Export(typeof(IModule))]
    [Export(typeof(GTurtleModule))]
    public class GTurtleModule : ModuleBase
    {
        
        public TurtleAreaViewModel TurtleArea { get; private set; }

        public SimpleIDEModule IDEModule { get; }

        public override void Initialize()
        {
            MainWindow.Title = "GTurtle";

            //show the turtle surface
            TurtleArea = IoC.Get<TurtleAreaViewModel>();
            Shell.ShowTool(TurtleArea);

            //show a new doc
            var doc = new TurtleScriptViewModel();
            doc.New("Untitled.pyt");
            Shell.OpenDocument(doc);
        }

        public Turtle CreateTurtle()
        {
            return new Turtle(TurtleArea);
        }

    }
}
