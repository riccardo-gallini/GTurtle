using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
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
    }
    
}