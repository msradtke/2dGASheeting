using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace _2dGASheeting.Models
{
    public class Pattern2d
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
    }
}
