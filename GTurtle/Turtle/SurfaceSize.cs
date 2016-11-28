using System.Collections.Generic;

namespace GTurtle
{
    public class SurfaceSize
    {
        public double Height { get; }
        public double Width { get; }

        public SurfaceSize(int width, int height)
        {
            Height = height;
            Width = width;
        }

        public override string ToString()
        {
            return Width.ToString() + " x " + Height.ToString();
        }

        public static List<SurfaceSize> List()
        {
            var _list = new List<SurfaceSize>();

            _list.Add(new SurfaceSize(4000, 3000));
            _list.Add(new SurfaceSize(1800, 1600));
            _list.Add(new SurfaceSize(900, 700));
            


            return _list;
        }

    }
}
