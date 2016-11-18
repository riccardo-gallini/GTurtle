using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.ComponentModel.Design;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Controls;
using GScripting.CodeEditor.Internal;

namespace GScripting.CodeEditor
{
    public class EditorControl : TextEditor
    {
        
        public EditorControl() : base()
        {
            this.ShowLineNumbers = false;
            this.FontFamily = new FontFamily("Consolas");
            this.FontSize = 12.75f;
            this.FontWeight = FontWeights.Normal;

            this.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            this.TextArea.TextEntering += textEditor_TextArea_TextEntering;

            //Syntax highlighting -- Python.xshd
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "GScripting.CodeEditor.Python.xshd";
            
            Stream xshd_stream = assembly.GetManifestResourceStream(resourceName);
            if (xshd_stream!=null)
            {
                XmlTextReader xshd_reader = new XmlTextReader(xshd_stream);

                this.SyntaxHighlighting = HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);
                xshd_reader.Close();
                xshd_stream.Close();
            }
            
            //Text marker <simple>
            initializeTextMarkerService();

            //Text marker tooltip management
            var textView = this.TextArea.TextView;
            textView.MouseHover += ToolTipMouseHover;
            textView.MouseHoverStopped += ToolTipMouseHoverStopped;
            textView.VisualLinesChanged += ToolTipVisualLineChanged;

            //IconbarMargin
            initializeBookmarkManager();

            //Indentation strategy
            //textEditor.TextArea.IndentationStrategy

        }

        #region " Breakpoints "

        IconBarManager bookmarkMargin;

        void initializeBookmarkManager()
        {
            bookmarkMargin = new IconBarManager(this.Document);
            this.TextArea.LeftMargins.Add(new IconBarMargin(bookmarkMargin));

            IServiceContainer services = (IServiceContainer)this.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(IBookmarkMargin), bookmarkMargin);
        }

        public IBookmark GetBookmarkFromLine(int line)
        {
           return bookmarkMargin.GetBookmarkFromLine(line);
        }

        public bool IsBreakpoint(int line)
        {
            //any other kind of bookmarks?
            return (GetBookmarkFromLine(line) != null);
        }

        public IBreakpoint GetBreakpoint(int line)
        {
            return (IBreakpoint) bookmarkMargin.GetBookmarkFromLine(line);
        }
        


        #endregion

        #region " Markers "

        ITextMarkerService textMarkerService;
        
        void initializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(this.Document);
            this.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            this.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)this.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        bool isSelected(ITextMarker marker)
        {
            var textEditor = this;

            int selectionEndOffset = textEditor.SelectionStart + textEditor.SelectionLength;
            if (marker.StartOffset >= textEditor.SelectionStart && marker.StartOffset <= selectionEndOffset)
                return true;
            if (marker.EndOffset >= textEditor.SelectionStart && marker.EndOffset <= selectionEndOffset)
                return true;
            return false;
        }

        #region " Public marker methods "

        public void RemoveAllErrorMarks()
        {
            textMarkerService.RemoveAll((m) => (ResolverMarkerType)m.Tag == ResolverMarkerType.Error);
        }

        public void MarkError(int start, int length, string message)
        {

            var marker = textMarkerService.Create(start, length);
            marker.Tag = ResolverMarkerType.Error;
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
            marker.ToolTip = message;
        }

        public void RemoveAllDebugMarks()
        {
            textMarkerService.RemoveAll((m) => (ResolverMarkerType)m.Tag == ResolverMarkerType.DebugCurrentLine);
        }

        public void MarkDebugLine(int line, bool isError, string message = "")
        {
            RemoveAllDebugMarks();

            var document_line = this.Document.GetLineByNumber(line);
            var marker = textMarkerService.Create(document_line.Offset, document_line.Length);
            marker.Tag = ResolverMarkerType.DebugCurrentLine;
            marker.MarkerTypes = TextMarkerTypes.None;
            marker.BackgroundColor = Colors.Yellow;

            if (isError)
            {
                marker.MarkerColor = Colors.Red;
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            }
                        
            if (message!="")
            {
                marker.ToolTip = message;
            }

            this.ScrollToLine(line);
        }
        
        #endregion

        #region " Editor Tooltip "

        private ToolTip toolTip;

        private void ToolTipMouseHover(object sender, MouseEventArgs e)
        {
            var pos = this.TextArea.TextView.GetPositionFloor(e.GetPosition(this.TextArea.TextView) + this.TextArea.TextView.ScrollOffset);
            bool inDocument = pos.HasValue;
            if (inDocument)
            {
                TextLocation logicalPosition = pos.Value.Location;
                int offset = this.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (toolTip == null)
                    {
                        toolTip = new ToolTip();
                        toolTip.Closed += ToolTipClosed;
                        toolTip.PlacementTarget = this;
                        toolTip.Content = new TextBlock
                        {
                            Text = (string)markerWithToolTip.ToolTip,
                            TextWrapping = TextWrapping.Wrap
                        };
                        toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            toolTip = null;
        }

        void ToolTipMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        void ToolTipVisualLineChanged(object sender, EventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }

        #endregion
        
        #endregion
        
        #region Completion Window

        CompletionWindow completionWindow;


        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(this.TextArea);
                // provide AvalonEdit with the data:
                //IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                //data.Add(new MyCompletionData("Item1"));
                //data.Add(new MyCompletionData("Item2"));
                //data.Add(new MyCompletionData("Item3"));
                //data.Add(new MyCompletionData("Another item"));
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #endregion

        public void NavigateToLine(int line)
        {
            this.ScrollToLine(line);

            var document_line = this.Document.GetLineByNumber(line);
            this.Select(document_line.Offset, document_line.Length);
        }

    }


    public enum ResolverMarkerType
    {
        Error = 0,
        DebugCurrentLine = 4

    }

}