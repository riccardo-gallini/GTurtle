using System.IO;
using System.Text;

namespace GScripting
{
    public class ExecutionOutput : TextWriter
    {
        private ExecutionScope _executionScope;

        public ExecutionOutput(ExecutionScope execScope) : base()
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
            _executionScope.Output?.Invoke(value.ToString());
        }

        public override void Write(string value)
        {
            _executionScope.Output?.Invoke(value);
        }
    }
}

