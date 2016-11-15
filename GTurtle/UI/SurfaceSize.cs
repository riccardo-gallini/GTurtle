using System.Collections.Generic;

namespace GTurtle
{
    class SurfaceSize
    {
        public System.Drawing.Size ActualSize;

        public SurfaceSize(int width, int height)
        {
            ActualSize = new System.Drawing.Size(width, height);
        }

        public override string ToString()
        {
            return ActualSize.Width.ToString() + " x " + ActualSize.Height.ToString();
        }

        public static List<SurfaceSize> List()
        {
            var _list = new List<SurfaceSize>();

            _list.Add(new SurfaceSize(800, 600));
            _list.Add(new SurfaceSize(1800, 1600));
            _list.Add(new SurfaceSize(4000, 3000));

            return _list;
        }

    }
}
