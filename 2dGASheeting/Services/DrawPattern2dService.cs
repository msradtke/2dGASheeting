using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using _2dGASheeting.Models;
namespace _2dGASheeting.Services
{
    class DrawPattern2dService
    {
        List<Rect> _blanks;
        List<Rect> _spaces;
        Rect _master;
        double _scale;
        int _width;
        int _height;
        public DrawPattern2dService(List<Rect> blanks, List<Rect> spaces, Rect master,int scale)
        {
            _blanks = blanks;
            _spaces = spaces;
            _master = master;
            _scale = scale;
        }

        public Canvas GetCanvas()
        {
            var canvas = new Canvas();

     
            foreach(var blank in _blanks)
            {
                var rect = new Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.Green
                };
                rect.Width = blank.Width*_scale;
                rect.Height = blank.Height * _scale;

                Canvas.SetLeft(rect, blank.X * _scale);
                Canvas.SetBottom(rect, blank.Y * _scale);
                canvas.Children.Add(rect);
            }

            foreach (var spaces in _spaces)
            {
                var rect = new Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.LightBlue,
                    Opacity = .7
                };

                rect.Width = spaces.Width * _scale;
                rect.Height = spaces.Height * _scale;

                Canvas.SetLeft(rect, spaces.X * _scale);
                Canvas.SetBottom(rect, spaces.Y * _scale);
                canvas.Children.Add(rect);

            }
            return canvas;
        }

    }
}
