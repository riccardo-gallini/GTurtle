using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework.Services;
using GScripting;

namespace GTurtle
{
    public class WatchViewModel : Tool
    {
        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Bottom;
            }
        }

        public void ShowScope(ExecutionContext info)
        {

        }
    }
}
