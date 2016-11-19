using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GTurtle
{
    public class Turtle
    {
        private Brush currentBrush;
        private double currentThickness;

        private double _origin_X;
        private double _origin_Y;

        private double _pos_X = 0;
        private double _pos_Y = 0;
        private double _orientation = 0;
        private bool _is_pen_down = true;

        private Canvas canvas;
               
        
        public Turtle(Canvas _canvas, SurfaceSize sz)
        {
            canvas = _canvas;
            _origin_X = sz.Width / 2;
            _origin_Y = sz.Height / 2;
            currentBrush = new SolidColorBrush(Colors.Black);
            currentThickness = 4.0;
        }


        public Dictionary<string, Object> GetCommands()
        {
            var cmd = new Dictionary<string, object>();
            cmd.Add("go", new Action<double>(this.go));
            cmd.Add("turn", new Action<double>(this.turn));
            cmd.Add("aim", new Action<double>(this.aim));
            cmd.Add("clear", new Action(this.clear));
            cmd.Add("color", new Action<string>(this.color));
            cmd.Add("go_to", new Action<double,double>(this.go_to));
            cmd.Add("image", new Action<string>(this.image));
            cmd.Add("pen_down", new Action(this.pen_down));
            cmd.Add("pen_up", new Action(this.pen_up));
            cmd.Add("width", new Action<int>(this.width));
            cmd.Add("jump", new Action<double, double>(this.jump));
            cmd.Add("rectangle", new Action<double, double, double,double>(this.rectangle));
            cmd.Add("line", new Action<double, double, double, double>(this.line));
            cmd.Add("circle", new Action<double, double, double>(this.circle));
            cmd.Add("bezier", new Action<double, double, double, double, double, double, double, double>(this.bezier));
            cmd.Add("x", new Func<double>(this.x));
            cmd.Add("y", new Func<double>(this.y));
            cmd.Add("sleep", new Action<int>(this.sleep));
            cmd.Add("nop", new Action(this.nop));

            return cmd;
        }

        #region "TURTLE COMMANDS IMPLEMENTATION"

        public void go(double distance)
        {
            double _new_X = _pos_X + distance * Math.Sin(_orientation / 180 * Math.PI);
            double _new_Y = _pos_Y + distance * Math.Cos(_orientation / 180 * Math.PI);

            go_to(_new_X, _new_Y);
        }
        
        public void jump(double x, double y)
        {
            _pos_X = x;
            _pos_Y = y;
        }

        public void go_to(double to_x, double to_y)
        {
            if (_is_pen_down)
            {
                line(_pos_X, _pos_Y, to_x, to_y);
            }
            jump(to_x, to_y);
        }

        public void aim(double angle)
        {
            _orientation = angle;
            _orientation = Math.IEEERemainder(_orientation, 360);
        }

        public void turn(double angle)
        {
            _orientation += angle;
            _orientation = Math.IEEERemainder(_orientation, 360);
        }

        public void pen_up()
        {
            _is_pen_down = false;
        }

        public void pen_down()
        {
            _is_pen_down = true;
        }

        public void color(string col_name)
        {
            var c = (Color)ColorConverter.ConvertFromString(col_name);
            currentBrush = new SolidColorBrush(c);
        }

        public void width(int w)
        {
            currentThickness = w;
        }

        public double x()
        {
            return _pos_X;
        }

        public double y()
        {
            return _pos_Y;
        }

        public void image(string url)
        {

        }

        public void nop()
        {
            
        }

        public void sleep(int t)
        {
            System.Threading.Thread.Sleep(t);
            //TODO: visual update?
        }

        public void rectangle(double x, double y, double width, double height)
        {
            double _x = _transform_X(x);
            double _y = _transform_Y(y);

            canvas.Dispatcher.Invoke(
                () =>
                {
                    var rect = new Rectangle();
                    rect.Height = height;
                    rect.Width = width;
                    rect.Stroke = currentBrush;
                    rect.StrokeThickness = currentThickness;
                    Canvas.SetTop(rect, _y);
                    Canvas.SetLeft(rect, _x);
                    canvas.Children.Add(rect);
                }
            );

            
        }

        public void line(double x1, double y1, double x2, double y2)
        {
            canvas.Dispatcher.Invoke(
                () => { 
                        var line = new Line();
                        line.X1 = _transform_X(x1);
                        line.Y1 = _transform_Y(y1);
                        line.X2 = _transform_X(x2);
                        line.Y2 = _transform_Y(y2);
                        line.Stroke = currentBrush;
                        line.StrokeThickness = currentThickness;
                        canvas.Children.Add(line);
                }
            );
        }

        public void bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
        }

        public void circle(double x, double y, double radius)
        {
            double _x = _transform_X(x - radius);
            double _y = _transform_Y(y + radius);
            double _diameter = 2 * (float)radius;

            canvas.Dispatcher.Invoke(
                () =>
                {

                    var ellipse = new Ellipse();
                    ellipse.Stroke = currentBrush;
                    ellipse.StrokeThickness = currentThickness;

                    ellipse.Height = _diameter;
                    ellipse.Width = _diameter;
                    Canvas.SetTop(ellipse, _y);
                    Canvas.SetLeft(ellipse, _x);
                    canvas.Children.Add(ellipse);
                }
            );
            
        }

        public void clear()
        {
            canvas.Dispatcher.Invoke(() => canvas.Children.Clear());
        }

        public void reset()
        {
            clear();
            jump(0, 0);
        }

        #endregion


        private double _transform_X(double x)
        {
            return _origin_X + x;
        }

        private double _transform_Y(double y)
        {
            return _origin_Y - y;
        }

    }
}
