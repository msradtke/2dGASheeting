using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public class Rect
    {
        public Rect()
        {

        }
        public Rect(Rect copy)
        {
            Height = copy.Height;
            Width = copy.Width;
            X = copy.X;
            Y = copy.Y;
        }
        public double Height { get; set; }
        public double Width { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
