using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework.Services;
using GScripting;
using Caliburn.Micro;

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

        private readonly BindableCollection<WatchItem> _items;
        
        public IObservableCollection<WatchItem> Items
        {
            get { return _items; }
        }

        public WatchViewModel()
        {
            DisplayName = "Watch";
            
            _items = new BindableCollection<WatchItem>();
            _items.CollectionChanged += (sender, e) =>
            {
                NotifyOfPropertyChange("Items");
            };


        }

        public void AddItem(string variableName, dynamic value, Type type)
        {
            var wi = new WatchItem()
            {
                VariableName = variableName,
                Value = value,
                Type = type
            };
            this.Items.Add(wi);
        }


        public void ShowScope(ExecutionContext info)
        {
            this.Items.Clear();
            foreach (var v in info.GetVariables())
            {
                this.AddItem(v.Name, v.Value, v.Type);
            }
        }
    }
}
