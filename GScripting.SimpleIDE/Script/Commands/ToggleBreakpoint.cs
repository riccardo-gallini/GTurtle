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
    public class ToggleBreakpoint : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Editor.ToggleBreakpoint";
            }
        }

        public override string Text
        {
            get
            {
                return "Toggle Breakpoint";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Toggle Breakpoint";
            }
        }

        [Export]
        public static CommandKeyboardShortcut ToggleBreakpointShortcut = new CommandKeyboardShortcut<ToggleBreakpoint>(new KeyGesture(Key.F9));

    }

}

