using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
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
    }
}
