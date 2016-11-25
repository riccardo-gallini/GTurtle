using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GTurtle
{
    public class Turtle
    {
        private Brush currentBrush;
        private double currentThickness;

        private Path currentPath;
        private PathFigure currentPathFigure;
        private PathGeometry currentPathGeometry;
        private double _currentPathStartX;
        private double _currentPathStartY;

        private Image turtleImage;
        private bool hasImage;

        private double _origin_X;
        private double _origin_Y;

        private double _pos_X = 0;
        private double _pos_Y = 0;
        private double _orientation = 0;
        private bool _is_pen_down = true;
        
        private Canvas canvas;
               
        
        public Turtle(Canvas _canvas, double CanvasHeight, double CanvasWidth)
        {
            canvas = _canvas;

            canvas.Dispatcher.Invoke(
                () =>
                {
                    _origin_X = CanvasWidth / 2;
                    _origin_Y = CanvasHeight / 2;
                    currentBrush = new SolidColorBrush(Colors.Black);
                    currentThickness = 4.0;

                    turtleImage = new Image();
                    canvas.Children.Add(turtleImage);
                }
            );

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

            if (hasImage)
            {
                canvas.Dispatcher.Invoke(
                    () =>
                    {
                        Canvas.SetTop(turtleImage, _transform_Y(_pos_Y) - turtleImage.Height / 2);
                        Canvas.SetLeft(turtleImage, _transform_X(_pos_X) - turtleImage.Width / 2);
                    }
                );
            }
        }

        public void go_to(double to_x, double to_y)
        {
            if (_is_pen_down)
            {
                path_line(to_x, to_y);
            }
            jump(to_x, to_y);
        }

        private void create_path()
        {
            _currentPathStartX = _pos_X;
            _currentPathStartY = _pos_Y;

            currentPathFigure = new PathFigure();
            currentPathFigure.StartPoint = new Point(_transform_X(_pos_X), _transform_Y(_pos_Y));
            currentPathFigure.IsFilled = false;

            currentPathGeometry = new PathGeometry();
            currentPathGeometry.Figures.Add(currentPathFigure);

            currentPath = new Path();
            currentPath.Stroke = currentBrush;
            currentPath.StrokeThickness = currentThickness;
            currentPath.Data = currentPathGeometry;

            canvas.Children.Add(currentPath);
        }

        private void path_line(double to_x, double to_y)
        {
            canvas.Dispatcher.Invoke(
                () =>
                {
                    if (currentPath == null)
                    {
                        create_path();
                    }

                    var segment = new LineSegment();
                    segment.Point = new Point(_transform_X(to_x), _transform_Y(to_y));
                    segment.IsStroked = true;

                    currentPathFigure.Segments.Add(segment);
                }
            );
            
        }

        public void closed()
        {
            canvas.Dispatcher.Invoke(
                () =>
                {
                    if (currentPath == null)
                    {
                        create_path();
                    }
                    currentPathFigure.IsClosed = true;
                }
            );
        }

        public void filled(string col_name)
        {
            canvas.Dispatcher.Invoke(
                () =>
                {
                    if (currentPath == null)
                    {
                        create_path();
                    }
                    currentPathFigure.IsFilled = true;
                    var c = (Color)ColorConverter.ConvertFromString(col_name);
                    currentPath.Fill = new SolidColorBrush(c);
                }
            );
        }


        public void aim(double angle)
        {
            _orientation = angle;
            _orientation = Math.IEEERemainder(_orientation, 360);

            if (hasImage)
            {

                canvas.Dispatcher.Invoke(
                    () =>
                        {
                            turtleImage.RenderTransform =
                                    new RotateTransform(angle,
                                                        turtleImage.Width / 2,
                                                        turtleImage.Height / 2);
                        }
                );

            }

        }

        public void turn(double angle)
        {
            aim(_orientation + angle);
        }

        public void pen_up()
        {
            _is_pen_down = false;
            currentPath = null;
        }

        public void pen_down()
        {
            _is_pen_down = true;
        }

        public void color(string col_name)
        {
            canvas.Dispatcher.Invoke(
                () =>
                {
                    var c = (Color)ColorConverter.ConvertFromString(col_name);
                    currentBrush = new SolidColorBrush(c);
                }
            );
            currentPath = null;
        }

        public void width(int w)
        {
            currentThickness = w;
            currentPath = null;
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
            hasImage = true;

            canvas.Dispatcher.Invoke(
                () =>
                {
                    var source = new BitmapImage(new Uri(url));
                    
                    turtleImage.Source = source;
                    turtleImage.Height = 35;
                    turtleImage.Width = 35;
                    
                }
            );

            //reposition image (if size is changed)
            jump(_pos_X, _pos_Y);
            
        }

        public void hide()
        {
            canvas.Dispatcher.Invoke(
                () => turtleImage.Visibility = Visibility.Hidden);
        }

        public void show()
        {
            canvas.Dispatcher.Invoke(
                () => turtleImage.Visibility = Visibility.Visible);
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
