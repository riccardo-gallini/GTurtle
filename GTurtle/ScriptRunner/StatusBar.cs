using Caliburn.Micro;
using Gemini.Modules.StatusBar;
using Gemini.Modules.StatusBar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GTurtle
{
    public class StatusBar
    {
        private IStatusBar statusBar;

        private StatusBarItemViewModel ln;
        private StatusBarItemViewModel col;
        private StatusBarItemViewModel globalStatusMessage;

        public void Initialize()
        {
            statusBar = IoC.Get<IStatusBar>();
                        
            ln = new StatusBarItemViewModel("", new GridLength(150));
            statusBar.Items.Add(ln);

            col = new StatusBarItemViewModel("", new GridLength(150));
            statusBar.Items.Add(col);

            globalStatusMessage = new StatusBarItemViewModel("", new GridLength(1, GridUnitType.Star));
            statusBar.Items.Add(globalStatusMessage);
        }

        public void SetCurrentLnCol(int line, int column)
        {
            ln.Message = "Ln " + line.ToString();
            col.Message = "Col " + column.ToString();
        }

        public void SetGlobalStatusMessage(GlobalStatus gs)
        {
            switch(gs)
            {
                case GlobalStatus.Editing:
                    globalStatusMessage.Message = "Editing";
                    break;

                case GlobalStatus.Running:
                    globalStatusMessage.Message = "Running";
                    break;

                case GlobalStatus.Paused:
                    globalStatusMessage.Message = "Paused";
                    break;

                case GlobalStatus.ExecutionError:
                    globalStatusMessage.Message = "Execution Error";
                    break;

            }
        }

    }
}
