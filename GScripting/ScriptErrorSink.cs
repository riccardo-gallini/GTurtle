using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace GScripting
{

    /// <summary>
    /// Supresses exceptions thrown by the PythonParser when it finds a parsing error.
    /// Errors are stored in Errors collection.
    /// </summary>
    public class ScriptErrorSink : ErrorSink
    {
        public List<ScriptError> Errors { get; }

        public ScriptErrorSink()
        {
            Errors = new List<ScriptError>();
            Listener = new ErrorSinkProxyListener(this);
        }

        public ErrorListener Listener { get; }

        private bool ContainsError(int line, string message)
        {
            return Errors.Where((err) => (err.Line == line) && (err.Message == message)).Any();
        }

        public override void Add(SourceUnit source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            int line = GetLine(span.Start.Line);

            //ensure errors are unique by line/message
            if (ContainsError(line, message) == false)
            { 
                Errors.Add(new ScriptError(source.Path, message, source.GetCodeLine(line), span, errorCode, severity));
            }
        }

        /// <summary>
        /// Ensure the line number is valid.
        /// </summary>
        static int GetLine(int line)
        {
            if (line > 0)
            {
                return line;
            }
            return 1;
        }
    }
}

