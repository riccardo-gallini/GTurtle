using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands.Editor
{
    public class NewScript : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Editor.NewScript";
            }
        }

        public override string Text
        {
            get
            {
                return "New Script";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "New Script";
            }
        }
    }

    public class NewScriptCommandHandler : CommandHandlerBase<NewScript>
    {
        public override Task Run(Command command)
        {

            return Task.CompletedTask;
        }
    }
}
