using Caliburn.Micro;
using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    [Export(typeof(IModule))]
    public class GTurtleModule : ModuleBase
    {
        
        public TurtleAreaViewModel TurtleArea { get; private set; }

        public override void Initialize()
        {

            //show the turtle surface
            TurtleArea = IoC.Get<TurtleAreaViewModel>();
            Shell.ShowTool(TurtleArea);

            MainWindow.Title = "GTurtle";

        }

        public Turtle CreateTurtle()
        {
            return new Turtle(TurtleArea);
        }

    }
}
