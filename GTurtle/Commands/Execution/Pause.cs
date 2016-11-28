using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
{
    [CommandDefinition]
    public class Pause : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.Pause";
            }
        }

        public override string Text
        {
            get
            {
                return "Pause";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Pause";
            }
        }
    }
}
