using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScripting
{
    public class DebugInfo
    {
        public int CurrentLine { get; private set; }
        public ExecutionContext ExecutionScope { get; private set; }
        public bool IsError { get; private set; }
        public string Message { get; private set; }
        public string ErrorTypeName { get; private set; }
        public string FunctionName { get; private set; }

        private DebugInfo()
        {
        }

        internal static DebugInfo CreateError(Exception ex, ExecutionContext executionScope)
        {
            var info = new DebugInfo();
            
            info.ExecutionScope = executionScope;

            ExceptionOperations eo = info.ExecutionScope.Engine.scriptEngine.GetService<ExceptionOperations>();

            string message;
            string errorTypeName;
            eo.GetExceptionMessage(ex, out message, out errorTypeName);
            info.Message = message;
            info.ErrorTypeName = errorTypeName;

            var frames = eo.GetStackFrames(ex);
            info.CurrentLine = frames[0].GetFileLineNumber();
            
            info.IsError = true;
            return info;
        }

        internal static DebugInfo Create(TraceBackFrame frame, ExecutionContext executionScope)
        {
            var info = new DebugInfo();
            info.CurrentLine = (int)frame.f_lineno;
            info.ExecutionScope = executionScope;
            info.FunctionName = frame.f_code.ToString();
            info.IsError = false;
            return info;
        }

        internal static DebugInfo CreateEmpty(ExecutionContext executionScope)
        {
            var info = new DebugInfo();
            info.ExecutionScope = executionScope;
            return info;
        }
    }
}
