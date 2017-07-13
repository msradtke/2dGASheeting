using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public class Solution
    {
        public Solution()
        {
            PatternDemands = new List<PatternDemand2d>();
        }
        public List<PatternDemand2d> PatternDemands { get; set; }
        public List<Pattern2d> Patterns { get {return PatternDemands.Select(x => x.Pattern).ToList(); } }
        public double Fitness { get; set; }
        public int MasterCount { get { return PatternDemands.Sum(x => x.Demand); } }
        public int PatternCount { get { return Patterns.Count(); } }
    }
}
