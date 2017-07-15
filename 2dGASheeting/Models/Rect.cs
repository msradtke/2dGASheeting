using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public class Rect : IEquatable<Rect>, IEqualityComparer<Rect>
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

        public bool Equals(Rect other)
        {
            if (Height != other.Height) return false;
            if (Width != other.Width) return false;
            return true;
        }

        public bool Equals(Rect x, Rect y)
        {
            if (x.Height != y.Height) return false;
            if (x.Width != y.Width) return false;
            return true;
        }

        public int GetHashCode(Rect obj)
        {
            return obj.GetHashCode();
        }
        public Rect Rotated()
        {
            var r = new Rect();
            r.Width = this.Height;
            r.Height = this.Width;
            return r;
        }
    }
    public class RectComparer : IEqualityComparer<Rect>
    {
        public bool Equals(Rect x, Rect y)
        {
            if (x.Height != y.Height) return false;
            if (x.Width != y.Width) return false;
            return true;
        }

        public int GetHashCode(Rect obj)
        {
            return (int)(obj.Height * obj.Width * 23);
        }
    }
}
