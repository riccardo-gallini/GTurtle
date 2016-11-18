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
        private DebugAction next_action;

        private Engine _engine;
        private ExecutionStatus _executionStatus;
        private string _source;

        internal Action<DebugInfo> _debuggerStep;
        internal Action<DebugInfo> _onTrace;
        internal Action<DebugInfo> _onError;
        internal Action<string> _onOutput;
        internal Action<DebugInfo> _onScriptEnd;
        internal Func<int, IBreakpoint> _onCheckBreakpoint;
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

        public void RegisterOnCheckBreakpoint(Func<int, IBreakpoint> func)
        {
            _onCheckBreakpoint = func;
        }
        
        
        public Engine Engine
        {
            get
            {
                return _engine;
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
            set
            {
                if (_executionStatus==ExecutionStatus.Stopped)
                {
                    _source = value;
                }
            }
        }

        public void RegisterCommands(Dictionary<string, object> commands)
        {
            foreach (var cmd in commands)
            {
                _variableScope.SetVariable(cmd.Key, cmd.Value);
                _defaultCommands.Add(cmd.Key, cmd.Value);
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
            request_pause = true;
            next_action = DebugAction.StepInto;
            resume_execution();
        }

        public void StepOver()
        {
            request_pause = true;
            next_action = DebugAction.StepOver;
            resume_execution();
        }

        public void StepOut()
        {
            request_pause = true;
            next_action = DebugAction.StepOver;
            resume_execution();
        }

        public void Continue()
        {
            request_pause = false;
            next_action = DebugAction.None;
            resume_execution();
        }

        private void resume_execution()
        {
            pause_waiter?.Set();
        }

        private void pause_execution()
        {
            using (pause_waiter = new AutoResetEvent(false))
            {
                pause_waiter.WaitOne();
            }
            pause_waiter = null;
        }

        public void Stop()
        {

        }

        public async Task RunAsync()
        {
            await Task.Run(() => runScript());
        }

        private void runScript()
        {
            ScriptSource scriptSource = Engine.scriptEngine.CreateScriptSourceFromString(_source, SourceCodeKind.File);

            using (var execOutput = new ExecutionOutput(this))
            {
                var ms = new MemoryStream();
                Engine.scriptEngine.Runtime.IO.SetOutput(ms, execOutput);

                this._executionStatus = ExecutionStatus.Running;
                Engine.scriptEngine.SetTrace(this.traceHook);

                try
                {
                    scriptSource.Execute(_variableScope);
                    _onScriptEnd?.Invoke(DebugInfo.CreateEmpty(this));
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(DebugInfo.CreateError(ex, this));
                }
                finally
                {
                    this._executionStatus = ExecutionStatus.Stopped;
                    Engine.scriptEngine.SetTrace(null);
                }
                
                                
            }
            
        }

        private void checkDebuggerPause(DebugInfo info)
        {
            if (_debuggerStep==null) { return; }
            
            if (request_stop)
            {

            }
            else if (request_pause || _onCheckBreakpoint?.Invoke(info.CurrentLine)!=null)
            {
                _executionStatus = ExecutionStatus.Paused;
                _debuggerStep(info);
                pause_execution();
               
                _executionStatus = ExecutionStatus.InternalTrace;

            }
        }

        private TracebackDelegate traceHook(TraceBackFrame frame, string result, object payload)
        {
            this._executionStatus = ExecutionStatus.InternalTrace;

            var info = DebugInfo.Create(frame, this);
            checkDebuggerPause(info);
            _onTrace?.Invoke(info);

            if (next_action == DebugAction.StepInto)
            {
                this._executionStatus = ExecutionStatus.Running;
                return traceHook;
            }
            else if (next_action == DebugAction.StepOver)
            {
                this._executionStatus = ExecutionStatus.Running;
                return null;
            }
            else if (next_action == DebugAction.StepOut)
            {
                this._executionStatus = ExecutionStatus.Running;
                return traceHook;
            }

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



