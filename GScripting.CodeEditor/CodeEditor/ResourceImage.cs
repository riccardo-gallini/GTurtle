using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MResolver.UI
{
    public static class ResourceImage
    {
        public static ImageSource Get(string relative_uri)
        {
            return new BitmapImage(new Uri(relative_uri, UriKind.Relative));
        }

    }
}
