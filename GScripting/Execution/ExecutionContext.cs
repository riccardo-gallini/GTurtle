using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace GScripting
{
    public class ExecutionContext
    {
        private ScriptScope _variableScope;
        private Dictionary<string, object> _defaultCommands;
        
        //////////////
        private bool request_pause = false;
        private bool request_stop = false;
        private AutoResetEvent pause_waiter;
        private DebugAction debugger_action;

        private Engine _engine;
        private ExecutionStatus _executionStatus;
        private ExecutionOutput execOutput;
        private string _source;

        public int TraceDepth { get; private set; }
        public int DebugDepth { get; private set; }
            
        internal Action<DebugInfo> _debuggerStep;
        internal Action<DebugInfo> _onTrace;
        internal Action<DebugInfo> _onError;
        internal Action<string> _onOutput;
        internal Action<DebugInfo> _onScriptEnd;
        internal Action<DebugInfo> _onStop;
        internal Func<int, IBreakpoint> _onCheckBreakpoint;
        internal Func<string> _getSource;
        //////////////

        internal ExecutionContext(Engine Engine, ScriptScope VariableScope)
        {
            _engine = Engine;
            _variableScope = VariableScope;
            _defaultCommands = new Dictionary<string, object>();
        }

        //////////////

        public void RegisterDebuggerStep(Action<DebugInfo> action)
        {
            _debuggerStep = action;
        }

        public void RegisterOnTrace(Action<DebugInfo> action)
        {
            _onTrace = action;
        }
                
        public void RegisterOnError(Action<DebugInfo> action)
        {
            _onError = action;
        }

        public void RegisterOnOutput(Action<string> action)
        {
            _onOutput = action;
        }

        public void RegisterOnScriptEnd(Action<DebugInfo> action)
        {
            _onScriptEnd = action;
        }

        public void RegisterOnStop(Action<DebugInfo> action)
        {
            _onStop = action;
        }

        public void RegisterOnCheckBreakpoint(Func<int, IBreakpoint> func)
        {
            _onCheckBreakpoint = func;
        }
        
        public void RegisterGetSource(Func<string> func)
        {
            _getSource = func;
        }

        internal ScriptEngine ScriptEngine
        {
            get
            {
                return _engine.scriptEngine;
            }
        }

        public ExecutionStatus ExecutionStatus
        {
            get
            {
                return _executionStatus;
            }
        }
                
        public string Source
        {
            get
            {
                return _source;
            }
        }

        public void AddToScope(string variable_name, object variable_value)
        {
            _variableScope.SetVariable(variable_name, variable_value);
            _defaultCommands.Add(variable_name, variable_value);
        }

        public void AddToScope(IDictionary<string,object> commands)
        {
            foreach(var cmd in commands)
            {
                AddToScope(cmd.Key, cmd.Value);
            }
        }
        
        public IList<ExecutionVariable> GetVariables()
        {
            var list = new List<ExecutionVariable>();

            foreach (string v_name in _variableScope.GetVariableNames())
            {
                if (!_defaultCommands.ContainsKey(v_name) && !v_name.StartsWith("__"))
                {
                    list.Add(GetVariable(v_name));
                }
            }

            return list;
        }

        public ExecutionVariable GetVariable(string v_name)
        {
            if (_variableScope.ContainsVariable(v_name))
            {
                var ev = new ExecutionVariable();
                ev.Name = v_name;
                ev.Value = _variableScope.GetVariable(v_name);
                ev.Type = ev.Value.GetType();
                return ev;
            }
            else
            {
                return null;
            }
        }
        
        public void RequestPause()
        {
            request_pause = true;
        }

        public void StepInto()
        {
            debugger_action = DebugAction.StepInto;
            resume();
        }

        public void StepOver()
        {
            debugger_action = DebugAction.StepOver;
            resume();
        }

        public void StepOut()
        {
            debugger_action = DebugAction.StepOver;
            resume();
        }

        public void Continue()
        {
            request_pause = false;
            debugger_action = DebugAction.None;
            resume();
        }

        private void resume()
        {
            pause_waiter?.Set();
        }

        private void debuggerStepPause(DebugInfo info)
        {
            _executionStatus = ExecutionStatus.Paused;
            DebugDepth = TraceDepth;

            _debuggerStep?.Invoke(info);

            //suspend execution waiting for signal
            using (pause_waiter = new AutoResetEvent(false))
            {
                pause_waiter.WaitOne();
            }
            pause_waiter = null;
            
            _executionStatus = ExecutionStatus.InternalTrace;
        }

        public void Stop()
        {
            request_stop = true;
            resume();
        }

        public void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        public async Task RunAsync()
        {
            await Task.Run(() => runScript());
        }
        
        private void runScript()
        {
            _source = _getSource();
            ScriptSource scriptSource = ScriptEngine.CreateScriptSourceFromString(_source, SourceCodeKind.File);

            TraceDepth = 0;
            DebugDepth = 0;

            using (execOutput = new ExecutionOutput(this))
            {
                this._executionStatus = ExecutionStatus.Running;
                ScriptEngine.SetTrace(this.traceHook);

                try
                {
                    scriptSource.Execute(_variableScope);
                    _onScriptEnd?.Invoke(DebugInfo.CreateEmpty(this));
                }
                catch (ThreadAbortException)
                {
                    if (request_stop)
                    {
                        Thread.ResetAbort();
                        _onStop?.Invoke(DebugInfo.CreateEmpty(this));
                    }
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(DebugInfo.CreateError(ex, this));
                }
                finally
                {
                    this._executionStatus = ExecutionStatus.Stopped;
                    
                    ScriptEngine.SetTrace(null);
                }
                                
            }
            
        }

        private void checkDebuggerStop()
        {
            if (request_stop)
            {
                Thread.CurrentThread.Abort();
            }
        }

        private bool _first = true;

        private TracebackDelegate traceHook(TraceBackFrame frame, string result, object payload)
        {
            //TODO: better management of this hook quirk: first trace is always a call; we miss the first line hook
            if (result == "call" && _first)
            {
                result = "line";
                _first = false;  
            }

            this._executionStatus = ExecutionStatus.InternalTrace;
            checkDebuggerStop();

            //temp: log on output
            execOutput.WriteLine(result + " line=" + frame.f_lineno.ToString());

            //send trace to client
            var info = DebugInfo.Create(frame, this);
            _onTrace?.Invoke(info);

            if (result == "call") TraceDepth++;
            if (result == "return") TraceDepth--;

            //decide whether we must pause execution and return control to client ide
            var must_pause = request_pause;

            if (debugger_action == DebugAction.StepInto)
            {
                must_pause = true;
            }
            else if (debugger_action == DebugAction.StepOver && TraceDepth<=DebugDepth)
            {
                must_pause = true;
            }
            else if (debugger_action == DebugAction.StepOut)
            {

            }
            else if (_onCheckBreakpoint?.Invoke(info.CurrentLine) != null)
            {
                must_pause = true;
            }

            //
            if (must_pause) debuggerStepPause(info);

            this._executionStatus = ExecutionStatus.Running;
            return traceHook;
        }
               
    }

    public enum ExecutionStatus
    {
        Stopped = 0,
        Running = 1,
        InternalTrace = 2,
        Paused = 3
    }

    public enum DebugAction
    {
        None = 0,
        StepOver = 1,
        StepInto = 2,
        StepOut = 3
    }
     
}



