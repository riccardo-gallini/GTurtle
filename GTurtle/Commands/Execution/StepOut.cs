using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
{
    [CommandDefinition]
    public class StepOut : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.StepOut";
            }
        }

        public override string Text
        {
            get
            {
                return "Step Out";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Step Out";
            }
        }
    }

}
