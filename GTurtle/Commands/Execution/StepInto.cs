﻿using Gemini.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle.Commands
{
    [CommandDefinition]
    public class StepInto : CommandDefinition
    {
        public override string Name
        {
            get
            {
                return "Execution.StepInto";
            }
        }

        public override string Text
        {
            get
            {
                return "Step Into";
            }
        }

        public override string ToolTip
        {
            get
            {
                return "Step Into";
            }
        }
    }
    
}