using ICSharpCode.SharpDevelop.Bookmarks;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace MResolver.UI
{
    public class Breakpoint : BookmarkBase, GScripting.IBreakpoint
    {

        public Breakpoint(int _line, IBookmarkMargin _manager) : base(new TextLocation(_line, 1))
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

        public IBookmarkMargin Manager { get; }
           

       
    }
}
