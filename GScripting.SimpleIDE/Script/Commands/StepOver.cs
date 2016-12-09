using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GScripting.SimpleIDE.Commands
{
    [CommandDefinition]
    public class StepOver : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.StepOver";
            }
        }

        public override string Text
        {
            get
            {
                return "Step Over";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Step Over";
            }
        }

        [Export]
        public static CommandKeyboardShortcut StepOverShortcut = new CommandKeyboardShortcut<StepOver>(new KeyGesture(Key.F10));

    }

}