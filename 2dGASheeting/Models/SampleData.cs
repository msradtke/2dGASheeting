using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public static class SampleData
    {
        static public Dictionary<Rect, int> GetSampleData1()
        {
            Dictionary<Rect, int> sample = new Dictionary<Rect, int>();

            //var width = new double[] { 27, 33, 12, 48, 54, 72, 38, 1 };
            //var height = new double[] { 27, 33, 12, 48, 40, 32, 38, 54 };
            //var demand = new int[] { 27, 33, 12, 48, 100, 150, 11, 10 };
            var width = new double[] { 27, 33, 12, 48, 54, 72, 38, 1};
            var height = new double[] { 27, 33, 12, 48, 40, 32, 38, 54};
            var demand = new int[] { 27, 33, 12, 48, 100, 150, 11,1};

            for (int i = 0; i < width.Length; ++i)
            {
                var Rect1 = new Rect();
                Rect1.Width = width[i];
                Rect1.Height = height[i];
                sample.Add(Rect1, demand[i]);
            }
            Master1 = new Rect();
            Master1.Width = 96;
            Master1.Height = 48;

            return sample;
        }
        static public Rect Master1;
    }
}
