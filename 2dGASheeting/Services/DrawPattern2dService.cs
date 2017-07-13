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
        List<PatternDemand2d> _demands;
        public DrawPattern2dService(List<PatternDemand2d> demands,Rect master,int scale)
        {
            _demands = demands;
            _master = master;
            _scale = scale;
        }

        public List<Canvas> GetCanvas()
        {
            var canvases = new List<Canvas>();
            foreach (var pattern in _demands.Select(x => x.Pattern))
            {
                var canvas = new Canvas();


                foreach (var blank in pattern.Blanks)
                {
                    var rect = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.Green
                    };
                    rect.Width = blank.Width * _scale;
                    rect.Height = blank.Height * _scale;

                    Canvas.SetLeft(rect, blank.X * _scale);
                    Canvas.SetBottom(rect, blank.Y * _scale);
                    canvas.Children.Add(rect);
                }

                foreach (var spaces in pattern.Spaces)
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
                canvases.Add(canvas);
            }
            return canvases;
        }

    }
}
