﻿using GScripting;
using GTurtle.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class ExecutionService
    {
        public ScriptViewModel RunningScript { get; private set; }
        private ScriptRunnerModule module;
        private ExecutionContext executionContext;
    
        internal ExecutionService(ScriptViewModel script, ScriptRunnerModule scriptRunnerModule)
        {
            RunningScript = script;
            module = scriptRunnerModule;
        }

        public async Task Play()
        {
            if (RunningScript.Status == ScriptStatus.Editing)
            {
                await doPlay(false);
            }
            else if (RunningScript.Status == ScriptStatus.Paused)
            {
                RunningScript.Status = ScriptStatus.Running;
                executionContext.Continue();
            }
        }

        public async Task StepInto()
        {
            if (RunningScript.Status == ScriptStatus.Editing)
            {
                await doPlay(true);
            }
            else if (RunningScript.Status == ScriptStatus.Paused)
            {
                RunningScript.RemoveAllDebugMarks();
                executionContext.StepInto();
            }
        }

        public async Task StepOver()
        {
            if (RunningScript.Status == ScriptStatus.Editing)
            {
                await doPlay(true);
            }
            else if (RunningScript.Status == ScriptStatus.Paused)
            {
                RunningScript.Status = ScriptStatus.Running;
                executionContext.StepOver();
            }
        }

        public Task Stop()
        {
            if (RunningScript.Status == ScriptStatus.Running || RunningScript.Status == ScriptStatus.Paused)
            {
                executionContext.Stop();
            }
            else if (RunningScript.Status == ScriptStatus.ExecutionError)
            {
                RunningScript.Status = ScriptStatus.Editing;
            }
            return Task.CompletedTask;
        }

        public Task Pause()
        {
            executionContext.RequestPause();
            return Task.CompletedTask;
        }

        public async Task doPlay(bool requestPause)
        {
            RunningScript.Status = ScriptStatus.Running;
            
            executionContext = module.CreateExecutionContext();
            executionContext.RegisterGetSource(getSource);
            executionContext.RegisterOnCheckBreakpoint(getBreakpoint);
            executionContext.RegisterDebuggerStep(debuggerStep);
            executionContext.RegisterOnOutput(outputUpdateUI);
            executionContext.RegisterOnError(errorUpdateUI);
            executionContext.RegisterOnScriptEnd(scriptEndUpdateUI);
            executionContext.RegisterOnStop(scriptEndUpdateUI);
            registerUtilityCommands(executionContext);

            if (requestPause) { executionContext.RequestPause(); }

            //exec the script
            await executionContext.RunAsync();

        }
        
        private void registerUtilityCommands(ExecutionContext executionContext)
        {
            executionContext.RegisterCommand("createturtle", new Func<Turtle>(module.CreateTurtle));
            executionContext.RegisterCommand("sleep", new Action<int>((t) => executionContext.Sleep(t)));
            executionContext.RegisterCommand("stop", new Action(() => executionContext.Stop()));
            executionContext.RegisterCommand("pause", new Action(() => executionContext.RequestPause()));
        }

        private string getSource()
        {
            return RunningScript.Text;
        }

        private void outputUpdateUI(string value)
        {
            module.Output.Append(value);
        }

        private void debuggerStep(DebugInfo info)
        {
            RunningScript.MarkDebugLine(info.CurrentLine, isError: false);
            module.Watch.ShowScope(info.ExecutionScope);
            RunningScript.Status = ScriptStatus.Paused;
        }

        private IBreakpoint getBreakpoint(int line)
        {
            return RunningScript.GetBreakpoint(line);
        }

        private void errorUpdateUI(DebugInfo info)
        {
            RunningScript.MarkDebugLine(info.CurrentLine, info.IsError, message: info.Message);
            module.Watch.ShowScope(info.ExecutionScope);
            RunningScript.Status = ScriptStatus.ExecutionError;
        }

        private void scriptEndUpdateUI(DebugInfo info)
        {
            RunningScript.Status = ScriptStatus.Editing;
        }


       
    }
}