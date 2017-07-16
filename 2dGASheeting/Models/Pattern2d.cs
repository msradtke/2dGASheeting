using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace _2dGASheeting.Models
{
    public class Pattern2d : IEquatable<Pattern2d>
    {
        public Pattern2d()
        {
            Blanks = new List<Rect>();
            Spaces = new List<Rect>();
            
        }
        public List<Rect> Blanks { get; set; }
        public List<Rect> Spaces { get; set; }
        public Rect Master { get; set; }
        public void SetMasterSpace()
        {
            Spaces.Add(new Rect { Width = Master.Width, Height = Master.Height, X = 0, Y = 0 });
        }
        public List<Rect> GetDistinct()
        {
            List<Rect> distinct = new List<Rect>();
            List<Rect> used = new List<Rect>();
            foreach (var item in Blanks)
            {
                int useCnt = 0;
                useCnt = used.Count(x => x.Width == item.Width && x.Height == item.Height);
                if (useCnt == 0)
                {
                    used.Add(item);
                    distinct.Add(item);
                }
            }
            return distinct;
        }

        public Pattern2d GetCopy()
        {
            var newPattern = new Pattern2d();
            foreach (var blank in Blanks)
            {
                var copyBlank = new Rect(blank);
                newPattern.Blanks.Add(copyBlank);

            }
            foreach (var space in Spaces)
            {
                var copySpace = new Rect(space);
                newPattern.Spaces.Add(copySpace);
            }
            newPattern.Master = Master;
            return newPattern;
        }

        public bool Equals(Pattern2d other)
        {
            if (Blanks.Count != other.Blanks.Count)
                return false;
            foreach(var blank in Blanks)
            {
                if (other.Blanks.Count(x => x.Equals(blank) && x.X == blank.X && x.Y == blank.Y) != 1)
                    return false;                
            }
            return true;
        }
    }
}
