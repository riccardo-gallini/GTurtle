﻿using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Modules.ErrorList;
using GScripting;
using GTurtle.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GTurtle
{
    [CommandHandler]
    public class ScriptViewModel : PersistedDocument,
                                   ICommandHandler<ToggleBreakpoint>,
                                   ICommandHandler<Play>,
                                   ICommandHandler<Pause>,
                                   ICommandHandler<StepInto>,
                                   ICommandHandler<StepOut>,
                                   ICommandHandler<StepOver>,
                                   ICommandHandler<Stop>
    {
        ParserService parserService;
        ExecutionService executionService;
        
        public ScriptStatus Status { get; internal set;}

        public string Text
        {
            get
            {
                return _view.Dispatcher.Invoke(()=>_view.Text);
            }
        }

        ScriptView _view;
        protected override void OnViewReady(object view)
        {
            _view = (ScriptView)view;

            _view.TextChanged += textChanged;

            //manage dropping image files on code text in order to import an image command
            _view.RegisterDropDataFormat(System.Windows.DataFormats.FileDrop);
            _view.GetDropData = getTextFromDataObject;

            //parser service
            parserService = IoC.Get<ScriptRunnerModule>().CreateParserService();
            parserService.RegisterGetSource(getSource);
            parserService.RegisterOnParseFinished(parseFinished);
            parserService.Start();

            executionService = new ExecutionService(this, IoC.Get<ScriptRunnerModule>());

            this.Status = ScriptStatus.Editing;

            base.OnViewReady(view);
        }

        private void textChanged(object sender, EventArgs e)
        {
            this.IsDirty = _view.IsModified;
        }

        #region PARSING

        private string getSource()
        {
            return _view.Dispatcher.Invoke(() => _view.Text);
        }

        private void parseFinished(ParseInfo info)
        {
            _view.Dispatcher.Invoke(() => _view.ParseInfo = info);

            var errorList = IoC.Get<IErrorList>();
            errorList.Items.Clear();
            foreach(var err in info.Errors)
            {
                errorList.AddItem(itemType: errorTypeMapping(err),
                                  description: err.Message,
                                  line: err.Line,
                                  path: this.DisplayName,
                                  column: err.Column,
                                  onClick: () => _view.NavigateToLine(err.Line));
            }
        }

        private static ErrorListItemType errorTypeMapping(ScriptError err)
        {
            ErrorListItemType errorType;
            if (err.Severity == ErrorSeverity.Error || err.Severity == ErrorSeverity.FatalError)
            {
                errorType = ErrorListItemType.Error;
            }
            else if (err.Severity == ErrorSeverity.Warning)
            {
                errorType = ErrorListItemType.Warning;
            }
            else
            {
                errorType = ErrorListItemType.Message;
            }

            return errorType;
        }

        #endregion
        
        #region PERSISTED DOCUMENT IMPLEMENTATION

        protected override Task DoLoad(string filePath)
        {
            _view.Load(filePath);
            return Task.CompletedTask;
        }
        
        protected override Task DoNew()
        {
            _view.Text = TurtleScript.NewTurtleScript();
            return Task.CompletedTask;
        }

        protected override Task DoSave(string filePath)
        {
            _view.Save(filePath);
            return Task.CompletedTask;
        }
        
        #endregion

        #region DEBUGGING

        public void MarkDebugLine(int currentLine, bool isError, string message = "")
        {
            _view.Dispatcher.Invoke(()=>_view.MarkDebugLine(currentLine, isError, message));
        }

        public void RemoveAllDebugMarks()
        {
            _view.Dispatcher.Invoke(() => _view.RemoveAllDebugMarks());
        }

        public IBreakpoint GetBreakpoint(int line)
        {
            return _view.Dispatcher.Invoke(()=>_view.GetBreakpoint(line));
        }

        #endregion

        #region COMMANDS IMPLEMENTATION

        Task ICommandHandler<ToggleBreakpoint>.Run(Command command)
        {
            _view.ToggleBreakpoint();
            return Task.CompletedTask;
        }

        async Task ICommandHandler<StepInto>.Run(Command command)
        {
            await executionService.StepInto();
        }

        async Task ICommandHandler<StepOver>.Run(Command command)
        {
            await executionService.StepOver();
        }

        async Task ICommandHandler<Stop>.Run(Command command)
        {
            await executionService.Stop();
        }

        Task ICommandHandler<StepOut>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        async Task ICommandHandler<Pause>.Run(Command command)
        {
            await executionService.Pause();
        }

        async Task ICommandHandler<Play>.Run(Command command)
        {
            await executionService.Play();
        }

        #endregion

        #region COMMANDS ENABLE/DISABLE

        void ICommandHandler<ToggleBreakpoint>.Update(Command command)
        {

        }

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
            command.Enabled = canRun();
        }

        private bool canRun()
        {
            switch (this.Status)
            {
                case ScriptStatus.Editing:
                    return true;

                case ScriptStatus.Running:
                    return false;

                case ScriptStatus.Paused:
                    return true;

                case ScriptStatus.ExecutionError:
                    return false;
            }
            return false;
        }

        private bool canPause()
        {
            switch (this.Status)
            {
                case ScriptStatus.Editing:
                    return false;

                case ScriptStatus.Running:
                    return true;

                case ScriptStatus.Paused:
                    return false;

                case ScriptStatus.ExecutionError:
                    return false;
            }
            return false;
        }

        private bool canStop()
        {
            switch (this.Status)
            {
                case ScriptStatus.Editing:
                    return false;

                case ScriptStatus.Running:
                    return true;

                case ScriptStatus.Paused:
                    return true;

                case ScriptStatus.ExecutionError:
                    return true;
            }
            return false;
        }

        #endregion

        protected override void OnDeactivate(bool close)
        {
            parserService.Stop();
        }

        #region "DROP IMG FILES INTO SCRIPT"

        private string getTextFromDataObject(DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] data = (string[])e.DataObject.GetData(System.Windows.DataFormats.FileDrop);

                if (data.Length > 0)
                {
                    var uri = new System.Uri(data[0]);
                    return "\"" + uri.AbsoluteUri + "\"";
                }
            }
            return null;
        }

        #endregion

    }

    public enum ScriptStatus
    {
        Editing = 1,
        Running = 2,
        Paused = 3,
        ExecutionError = 4
    }
}