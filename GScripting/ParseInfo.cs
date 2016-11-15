using System.Collections.Generic;
using IronPython.Compiler.Ast;

namespace GScripting
{
    /// <summary>
    /// Contains info obtained by the parser the formula text
    /// Info:  - errors from parser
    ///        - the complete python ast
    ///        - variable dependecies from formula extracted from python ast
    /// </summary>
    public class ParseInfo
    {
        public IList<ScriptError> Errors { get; }
        public PythonAst Ast { get; }

        public bool IsValid { get; }
        
        public ParseInfo(List<ScriptError> errors, PythonAst python_ast)
        {
            this.Errors = errors;
            this.Ast = python_ast;
            
            if (errors.Count == 0) { this.IsValid = true; }
        }

    }
}
