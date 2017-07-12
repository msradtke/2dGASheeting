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
        public List<Rect> Blanks { get; set; }
        public List<Rect> Spaces { get; set; }
        public Rect Master { get; set; }
    }
}
