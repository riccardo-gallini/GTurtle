using Gemini.Framework.Commands;
using GScripting;
using GTurtle.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class ScriptRunner :
                        ICommandHandler<Play>,
                        ICommandHandler<Pause>,
                        ICommandHandler<StepInto>,
                        ICommandHandler<StepOut>,
                        ICommandHandler<StepOver>,
                        ICommandHandler<Stop>

    {
        public ScriptEditorViewModel RunningScript { get; private set; }
        private ScriptRunnerModule module;
        private ExecutionContext executionContext;

        public ScriptRunner(ScriptRunnerModule MainModule)
        {
            module = MainModule;
        }
        
        Task ICommandHandler<StepInto>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<StepOver>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<Stop>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<StepOut>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<Pause>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        async Task ICommandHandler<Play>.Run(Command command)
        {
            if (module.GlobalStatus == GlobalStatus.Editing)
            {
                RunningScript = module.ActiveScript;
                await doPlay(false);
            }
            else if (module.GlobalStatus == GlobalStatus.Paused)
            {
                module.GlobalStatus = GlobalStatus.Running;
                executionContext.Continue();
            }
        }

        private async Task doPlay(bool requestPause)
        {
            module.GlobalStatus = GlobalStatus.Running;

            module.SurfaceWindow.Clear();

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
            module.Output.AppendLine(value);
        }

        private void debuggerStep(DebugInfo info)
        {
            this.InvokeAction(
                () =>
                {
                    RunningScript.MarkDebugLine(info.CurrentLine, isError: false);
                    module.Watch.ShowScope(info.ExecutionScope);
                    module.GlobalStatus = GlobalStatus.Paused;
                });
        }

        private IBreakpoint getBreakpoint(int line)
        {
            return RunningScript.GetBreakpoint(line);
        }

        private void errorUpdateUI(DebugInfo info)
        {
            RunningScript.MarkDebugLine(info.CurrentLine, info.IsError, message: info.Message);
            module.Watch.ShowScope(info.ExecutionScope);
            module.GlobalStatus = GlobalStatus.ExecutionError;
        }

        private void scriptEndUpdateUI(DebugInfo info)
        {
            module.GlobalStatus = GlobalStatus.Editing;
        }


        #region EXECUTION COMMANDS ENABLE/DISABLE

        void ICommandHandler<StepInto>.Update(Command command)
        {
            command.Enabled = canRun();
        }

        void ICommandHandler<StepOver>.Update(Command command)
        {
            command.Enabled = canRun();
        }

        void ICommandHandler<Stop>.Update(Command command)
        {
            command.Enabled = canStop();            
        }

        void ICommandHandler<StepOut>.Update(Command command)
        {
            command.Enabled = canRun();
        }

        void ICommandHandler<Pause>.Update(Command command)
        {
            command.Enabled = canPause();
        }

        void ICommandHandler<Play>.Update(Command command)
        {
            command.Enabled = module.ActiveScript != null && canRun();
        }

        private bool canRun()
        {
            switch (module.GlobalStatus)
            {
                case GlobalStatus.Editing:
                    return true;

                case GlobalStatus.Running:
                    return false;

                case GlobalStatus.Paused:
                    return true;

                case GlobalStatus.ExecutionError:
                    return false;
            }
            return false;
        }

        private bool canPause()
        {
            switch (module.GlobalStatus)
            {
                case GlobalStatus.Editing:
                    return false;

                case GlobalStatus.Running:
                    return true;

                case GlobalStatus.Paused:
                    return false;

                case GlobalStatus.ExecutionError:
                    return false;
            }
            return false;
        }

        private bool canStop()
        {
            switch (module.GlobalStatus)
            {
                case GlobalStatus.Editing:
                    return false;

                case GlobalStatus.Running:
                    return true;

                case GlobalStatus.Paused:
                    return true;

                case GlobalStatus.ExecutionError:
                    return true;
            }
            return false;
        }

        #endregion
    }
}
