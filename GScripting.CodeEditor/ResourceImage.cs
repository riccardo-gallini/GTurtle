using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GScripting.CodeEditor
{
    internal static class ResourceImage
    {
        public static ImageSource Get(string relative_uri)
        {
            return new BitmapImage(new Uri(relative_uri, UriKind.Relative));
        }

    }
}
