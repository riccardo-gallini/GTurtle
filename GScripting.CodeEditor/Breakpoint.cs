using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using GScripting.CodeEditor.Internal;

namespace GScripting.CodeEditor
{
    public class Breakpoint : BookmarkBase, GScripting.IBreakpoint
    {

        internal Breakpoint(int _line, IBookmarkMargin _manager) : base(new TextLocation(_line, 1))
        {
            Manager = _manager;
        }
        

        public override ImageSource Image
        {
            get
            {
                return ResourceImage.Get("CodeEditor/Icons/Bookmarks/Breakpoint.png");
            }
        }

        internal IBookmarkMargin Manager { get; }
           

       
    }
}
