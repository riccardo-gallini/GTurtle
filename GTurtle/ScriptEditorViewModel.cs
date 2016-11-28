using Caliburn.Micro;
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

namespace GTurtle
{
    public class ScriptEditorViewModel : PersistedDocument, 
                                         ICommandHandler<ToggleBreakpoint>
    {
        ParserService parserService;
        ScriptEditorView _view;

        protected override void OnViewReady(object view)
        {
            _view = (ScriptEditorView)view;

            _view.TextChanged += textChanged;

            parserService = IoC.Get<MainModule>().CreateParserService();
            parserService.RegisterGetSource(getSource);
            parserService.RegisterOnParseFinished(parseFinished);
            parserService.Start();
        
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

        private string newScriptText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "GTurtle.UI.newscript.py";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    return null;
                }

            }
        }

        protected override Task DoNew()
        {
            _view.Text = newScriptText();
            return Task.CompletedTask;
        }

        protected override Task DoSave(string filePath)
        {
            _view.Save(filePath);
            return Task.CompletedTask;
        }

        #endregion

        void ICommandHandler<ToggleBreakpoint>.Update(Command command)
        {

        }

        Task ICommandHandler<ToggleBreakpoint>.Run(Command command)
        {
            _view.ToggleBreakpoint();
            return Task.CompletedTask;
        }

        
    }
}
