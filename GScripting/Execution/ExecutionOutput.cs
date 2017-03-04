using System;
using System.IO;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace GScripting
{
    internal class ExecutionOutput : TextWriter
    {
        private ExecutionContext _executionScope;
        private MemoryStream _stream;

        internal ExecutionOutput(ExecutionContext execScope) : base()
        {
            _executionScope = execScope;
            _stream = new MemoryStream();

            _executionScope.ScriptEngine.Runtime.IO.SetOutput(_stream, this);
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

        public override void WriteLine(string value)
        {
            Write(value + "\n");
        }

        public override void WriteLine()
        {
            Write("\n");
        }

    }
}

