using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
{
    [CommandDefinition]
    public class Stop : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.Stop";
            }
        }

        public override string Text
        {
            get
            {
                return "Stop";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Stop";
            }
        }
    }
    
}
