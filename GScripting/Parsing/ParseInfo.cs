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
        internal IDictionary<int, Node> codeMap { get; }

        public bool IsValid { get; }
        
        internal ParseInfo(List<ScriptError> errors, PythonAst python_ast)
        {
            this.Errors = errors;
            this.Ast = python_ast;
            codeMap = new Dictionary<int, Node>();

            if (errors.Count == 0) { this.IsValid = true; }
            
            var script_walker = new ScriptAstWalker(this);
            python_ast.Walk(script_walker);
        }

        public Node AstNodeAtOffset(int offset)
        {
            Node n;
            codeMap.TryGetValue(offset, out n);

            return n;
        }
        
    }
}
