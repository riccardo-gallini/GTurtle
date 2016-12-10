using Caliburn.Micro;
using GScripting.SimpleIDE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class TurtleScriptViewModel : ScriptViewModel
    {
        public TurtleScriptViewModel() : base()
        {
            GTurtleModule module = IoC.Get<GTurtleModule>();
            this.Scope.Add("createturtle", new Func<Turtle>(module.CreateTurtle));
        }

        protected override string NewScriptText()
        {
            return TurtleScript.NewTurtleScript();
        }

    }
}
