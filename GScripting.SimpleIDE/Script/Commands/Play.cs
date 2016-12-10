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
    public class Play : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.Play";
            }
        }

        public override string Text
        {
            get
            {
                return "Play";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Play";
            }
        }

        [Export]
        public static CommandKeyboardShortcut PlayShortcut = new CommandKeyboardShortcut<Play>(new KeyGesture(Key.F5));

    }
}
