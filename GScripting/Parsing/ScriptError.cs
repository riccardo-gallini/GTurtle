using Microsoft.Scripting;
using System;

namespace GScripting
{
    /// <summary>
	/// Saves information about a parser error reported by the FormulaErrorSink.
	/// </summary>
	public class ScriptError
    {
        public string Path { get; }
        public string Message { get; }
        public string LineText { get; }
        private SourceSpan Span { get; }
        public int ErrorCode { get; }
        public ErrorSeverity Severity { get; }

        public int Line
        {
            get
            {
                return Span.Start.Line;
            }
        }

        public int Column
        {
            get
            {
                return Span.Start.Column;
            }
        }

        public int SpanStartIndex
        {
            get
            {
                return Span.Start.Index;
            }
        }

        public int SpanLength
        {
            get
            {
                return Span.Length;
            }
        }

        internal ScriptError(string path, string message, string lineText, SourceSpan span, int errorCode, Severity severity)
        {
            this.Path = path;
            this.Message = message;
            this.LineText = lineText;
            this.Span = span;
            this.ErrorCode = errorCode;
            this.Severity = (ErrorSeverity)severity;
        }

        public override string ToString()
        {
            return String.Concat("[", ErrorCode, "][Sev:", Severity.ToString(), "]", Message, "\r\nLine: ", Span.Start.Line, LineText, "\r\n", Path, "\r\n");
        }
    }

    public enum ErrorSeverity
    {
        Ignore = 0,
        Warning = 1,
        Error = 2,
        FatalError = 3
    }
}