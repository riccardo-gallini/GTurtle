using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GTurtle
{
    public class TurtleAreaViewModel : Document
    {
        TurtleAreaView _view;

        public TurtleAreaViewModel()
        {
            this.DisplayName = "TURTLE SURFACE";
        }

        protected override void OnViewReady(object view)
        {
            _view = (TurtleAreaView)view;
            base.OnViewReady(view);
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

    }
}
