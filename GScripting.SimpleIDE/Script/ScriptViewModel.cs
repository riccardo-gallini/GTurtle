using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Modules.ErrorList;
using Gemini.Modules.Shell.Commands;
using GScripting;
using GScripting.SimpleIDE.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Framework.Services;

namespace GScripting.SimpleIDE
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

        public IDictionary<string, object> Scope = new Dictionary<string, object>();

        public ScriptStatus Status { get; internal set;}

        public string Text
        {
            get
            {
                return _view.editor.Text;
            }
        }
        
        protected virtual string NewScriptText()
        {
            return string.Empty;
        }

        ScriptView _view;
        protected override void OnViewReady(object view)
        {
            _view = (ScriptView)view;

            _view.editor.TextChanged += textChanged;

            _view.editor.Text = NewScriptText();

            //manage dropping image files on code text in order to import an image command
            _view.editor.RegisterDropDataFormat(DataFormats.FileDrop);
            _view.editor.GetDropData = getTextFromDataObject;

            //parser service
            parserService = IoC.Get<SimpleIDEModule>().CreateParserService();
            parserService.RegisterGetSource(GetSource);
            parserService.RegisterOnParseFinished(parseFinished);
            parserService.Start();

            executionService = new ExecutionService(this, IoC.Get<SimpleIDEModule>());

            this.Status = ScriptStatus.Editing;

            base.OnViewReady(view);
        }

        private void textChanged(object sender, EventArgs e)
        {
            this.IsDirty = _view.editor.IsModified;
        }

        #region PARSING

        public string GetSource()
        {
            return _view.Dispatcher.Invoke(() => _view.editor.Text);
        }

        private void parseFinished(ParseInfo info)
        {
            _view.Dispatcher.Invoke(() => _view.editor.ParseInfo = info);

            var errorList = IoC.Get<IErrorList>();
            errorList.Items.Clear();
            foreach(var err in info.Errors)
            {
                errorList.AddItem(itemType: errorTypeMapping(err),
                                  description: err.Message,
                                  line: err.Line,
                                  path: this.DisplayName,
                                  column: err.Column,
                                  onClick: () => _view.editor.NavigateToLine(err.Line));
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
            _view.editor.Load(filePath);
            _view.editor.IsModified = false;
            this.IsDirty = false;
            return Task.CompletedTask;
        }
        
        protected override Task DoNew()
        {
            return Task.CompletedTask;
        }

        protected override Task DoSave(string filePath)
        {
            _view.editor.Save(filePath);
            return Task.CompletedTask;
        }

        public async override void CanClose(Action<bool> callback)
        {
            bool continue_closing = true;

            if (this.IsDirty)
            {
                var prompt = string.Format("There are unsaved changes in document {0}.\n\nDo you want to save them?", this.DisplayName);
                var title = IoC.Get<IMainWindow>().Title;
                var result = MessageBox.Show(prompt, title, MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    //save
                    var save_handler = this as ICommandHandler<SaveFileCommandDefinition>;
                    await save_handler?.Run(new Command(new SaveFileCommandDefinition()));

                    continue_closing = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    continue_closing = true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    continue_closing = false;
                }
            }
            else
            {
                continue_closing = true;
            }

            callback(continue_closing);
        }


        #endregion

        #region DEBUGGING

        public void MarkDebugLine(int currentLine, bool isError, string message = "")
        {
            _view.Dispatcher.Invoke(() => _view.editor.MarkDebugLine(currentLine, isError, message));
        }

        public void RemoveAllDebugMarks()
        {
            _view.Dispatcher.Invoke(() => _view.editor.RemoveAllDebugMarks());
        }

        public IBreakpoint GetBreakpoint(int line)
        {
            return _view.Dispatcher.Invoke(()=>_view.editor.GetBreakpoint(line));
        }

        #endregion

        #region COMMANDS IMPLEMENTATION

        Task ICommandHandler<ToggleBreakpoint>.Run(Command command)
        {
            _view.editor.ToggleBreakpoint();
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
        
        

        protected override void OnActivate()
        {
            executionService?.UpdateWatch();
            base.OnActivate();
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
