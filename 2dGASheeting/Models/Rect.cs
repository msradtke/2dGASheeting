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
        public void Rotate()
        {
            var swapH = Height;
            Height = Width;
            Width = swapH;
        }
        public bool Equals(Rect other)
        {

            if (Height == other.Height && Width == other.Width) return true;
            if (Height == other.Width && Width == other.Height) return true;
            return false;
        }

        public bool IsExactlyEqual(Rect other)
        {
            if (X != other.X || Y != other.Y) return false;

            if (Height == other.Height && Width == other.Width) return true;
            if (Height == other.Width && Width == other.Height) return true;
            return false;
        }

        public bool Equals(Rect x, Rect y)
        {
            if (x.Height == y.Height && x.Width == y.Width) return true;
            if (x.Height == y.Width && x.Width == y.Height) return true;
            return false;
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
            if (x.Height == x.Height && x.Width == x.Width) return true;
            if (x.Height == x.Width && x.Width == x.Height) return true;
            return false;
        }

        public int GetHashCode(Rect obj)
        {
            return (int)(obj.Height * obj.Width * 23);
        }
    }

    public class BlankEquality : IEquatable<Rect>
    {
        public bool Equals(Rect other)
        {
            throw new NotImplementedException();
        }
    }

}
