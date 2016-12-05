using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class WatchItem : PropertyChangedBase
    {
        
        private string _variableName;
        public string VariableName
        {
            get { return _variableName; }
            set
            {
                _variableName = value;
                NotifyOfPropertyChange(() => VariableName);
            }
        }

        private dynamic _value;
        public dynamic Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        private Type _type;
        public Type Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }
        
        public System.Action OnClick { get; set; }


    }

}
