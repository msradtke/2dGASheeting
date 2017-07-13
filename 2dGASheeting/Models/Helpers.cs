using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public static class Helpers
    {
        public static T[] Shuffle<T>(this IEnumerable<T> items)
        {
            var result = items.ToArray();
            var r = new Random();
            for (int i = items.Count(); i > 1; i--)
            {
                int j = r.Next(i);
                var t = result[j];
                result[j] = result[i - 1];
                result[i - 1] = t;
            }

            return result;
        }

    }
}
