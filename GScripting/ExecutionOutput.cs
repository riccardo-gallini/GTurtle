using System.IO;
using System.Text;

namespace GScripting
{
    internal class ExecutionOutput : TextWriter
    {
        private ExecutionContext _executionScope;

        internal ExecutionOutput(ExecutionContext execScope) : base()
        {
            _executionScope = execScope;
        }

        public override Encoding Encoding
        {
            get
            {
                return new ASCIIEncoding();
            }
        }

        public override void Write(char value)
        {
            _executionScope._onOutput?.Invoke(value.ToString());
        }

        public override void Write(string value)
        {
            _executionScope._onOutput?.Invoke(value);
        }
    }
}

