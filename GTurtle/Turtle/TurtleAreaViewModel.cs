using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Gemini.Framework.Services;
using System.ComponentModel.Composition;

namespace GTurtle
{
    [Export(typeof(TurtleAreaViewModel))]
    public class TurtleAreaViewModel : Tool
    {
        private TurtleAreaView _view
        {
            get
            {
                return (this.GetView() as TurtleAreaView);
            }
        }


        public TurtleAreaViewModel()
        {
            this.DisplayName = "TURTLE SURFACE";
        }

        public void Clear()
        {

        }

        public Canvas Content
        {
            get
            {
                return _view.content;
            }
        }

        public double ContentHeight
        {
            get
            {
                return _view.theGrid.Height;
            }
            set
            {
                _view.theGrid.Height = value;
            }
        }

        public double ContentWidth
        {
            get
            {
                return _view.theGrid.Width;
            }
            set
            {
                _view.theGrid.Width = value;
            }
        }

        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Right;
            }
        }
    }
}
