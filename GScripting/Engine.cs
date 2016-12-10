using System;
using System.Collections.Generic;
using IronPython;
using IronPython.Hosting;
using IronPython.Compiler;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using System.Threading.Tasks;

namespace GScripting
{
    public class Engine
    {
        internal ScriptEngine scriptEngine { get; }

        public Engine()
        {
            scriptEngine = Python.CreateEngine();
        }

        //spawns the python compiler and analyzes a script
        // outputs: ParseInfo class containing:
        //          errors
        //          python ast
        public ParseInfo Parse(string source)
        {
            var parseErrorSink = new ScriptErrorSink();

            ScriptSource script_source = scriptEngine.CreateScriptSourceFromString(source, SourceCodeKind.File);
            var source_unit = HostingHelpers.GetSourceUnit(script_source);

            var language_context = HostingHelpers.GetLanguageContext(scriptEngine);
            Parser parser = Parser.CreateParser(
                    new CompilerContext(source_unit, language_context.GetCompilerOptions(), parseErrorSink),
                    (PythonOptions)language_context.Options);

            var python_ast = parser.ParseFile(false);

            var info = new ParseInfo(parseErrorSink.Errors, python_ast);

            //no need for that
            //var walker = new TurtleScriptAstWalker(info);
            //python_ast.Walk(walker);

            return info;

        }

        public ExecutionContext CreateExecutionContext()
        {
            var variableScope = scriptEngine.CreateScope();
            var context = new ExecutionContext(this, variableScope);
            return context;
        }
        
    }
    
}
