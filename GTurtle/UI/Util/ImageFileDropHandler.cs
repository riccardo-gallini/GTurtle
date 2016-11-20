using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GTurtle
{
    public class ImageFileDropHandler
    {
        private TextArea textArea;
        public Func<DataObjectPastingEventArgs, string> GetTextFromDataObject { get; set; }
       
        public ImageFileDropHandler(TextArea _textArea)
        {
            textArea = _textArea;

            textArea.DragEnter += this.textArea_DragEnter;
            textArea.DragOver += this.textArea_DragOver;
            textArea.DragLeave += this.textArea_DragLeave;
            textArea.Drop += this.textArea_Drop;
            textArea.GiveFeedback += this.textArea_GiveFeedback;
            textArea.QueryContinueDrag += this.textArea_QueryContinueDrag;
        }

        void textArea_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = GetEffect(e);
                textArea.Caret.Show();
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        void textArea_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = GetEffect(e);
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        void textArea_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (!textArea.IsKeyboardFocusWithin)
                    textArea.Caret.Hide();
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }
        
        void textArea_Drop(object sender, DragEventArgs e)
        {
            try
            {
                DragDropEffects effect = GetEffect(e);
                e.Effects = effect;
                if (effect != DragDropEffects.None)
                {
                    int start = textArea.Caret.Offset;
                    
                    var pastingEventArgs = new DataObjectPastingEventArgs(e.Data, true, DataFormats.FileDrop);
                    textArea.RaiseEvent(pastingEventArgs);
                    if (pastingEventArgs.CommandCancelled)
                        return;

                    string text = GetTextFromDataObject?.Invoke(pastingEventArgs);
                    if (text == null)
                        return;
                                                
                    textArea.Document.Insert(start, text);
                    textArea.Selection = Selection.Create(textArea, start, start + text.Length);
                    
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        void OnDragException(Exception ex)
        {
            // WPF swallows exceptions during drag'n'drop or reports them incorrectly, so
            // we re-throw them later to allow the application's unhandled exception handler
            // to catch them
            textArea.Dispatcher.BeginInvoke(
                DispatcherPriority.Send,
                new Action(delegate {
                    throw new DragDropException("Exception during drag'n'drop", ex);
                }));
        }
                
        void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            try
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }
                
        void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            try
            {
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                }
                else if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.LeftMouseButton)
                {
                    e.Action = DragAction.Drop;
                }
                else
                {
                    e.Action = DragAction.Continue;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        DragDropEffects GetEffect(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Handled = true;
                int visualColumn;
                bool isAtEndOfLine;
                int offset = GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn, out isAtEndOfLine);
                if (offset >= 0)
                {
                    textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offset), visualColumn) { IsAtEndOfLine = isAtEndOfLine };
                    textArea.Caret.DesiredXPos = double.NaN;
                    if (textArea.ReadOnlySectionProvider.CanInsert(offset))
                    {
                        if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move
                            && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey)
                        {
                            return DragDropEffects.Move;
                        }
                        else
                        {
                            return e.AllowedEffects & DragDropEffects.Copy;
                        }
                    }
                }
            }
            return DragDropEffects.None;
        }

        int GetOffsetFromMousePosition(Point positionRelativeToTextView, out int visualColumn, out bool isAtEndOfLine)
        {
            visualColumn = 0;
            TextView textView = textArea.TextView;
            Point pos = positionRelativeToTextView;
            if (pos.Y < 0)
                pos.Y = 0;
            if (pos.Y > textView.ActualHeight)
                pos.Y = textView.ActualHeight;
            pos += textView.ScrollOffset;
            if (pos.Y >= textView.DocumentHeight)
                pos.Y = textView.DocumentHeight - 0.01;
            VisualLine line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line != null)
            {

                var textLine = line.GetTextLineByVisualYPosition(pos.Y);
                visualColumn = line.GetVisualColumn(textLine, pos.X, textArea.Selection.EnableVirtualSpace);
                isAtEndOfLine = (visualColumn >= line.GetTextLineVisualStartColumn(textLine) + textLine.Length);
                return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
            }
            isAtEndOfLine = false;
            return -1;
        }

    }
}
