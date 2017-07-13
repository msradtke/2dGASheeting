using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace _2dGASheeting.Models
{
    public class _2dGeneticAlg
    {
        List<Solution> _solutions;
        int _population;
        Rect _master;
        Dictionary<Rect, int> _demand;
        List<PatternDemand2d> _patternDemands;
        List<Rect> _items;
        double additionalPatternSelection = .10;


        public _2dGeneticAlg()
        {
                
        }
        public void UseSampleData()
        {
            _demand = SampleData.GetSampleData1();
            _items = _demand.Keys.ToList();
            _population = 10;
            _master = SampleData.Master1;
        }
        public void Process()
        {
            CreateInitialSolutions();
        }
        public List<PatternDemand2d> CreateInitialSolutions()
        {
            var BLBF = new BottomLeftBestFitHeuristic(_demand,_master);
            return BLBF.Process();
        }
        Func<Solution> GetSelectParentFn()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //get total weight
            var totalFitness = _solutions.Sum(x => x.Fitness);
            Dictionary<Solution, double> weights = new Dictionary<Solution, double>();
            foreach (var s in _solutions)
            {
                var propWeight = s.Fitness / totalFitness;
                weights.Add(s, propWeight);
            }
            //Dictionary<int, Solution> segments = new Dictionary<int, Solution>();
            Dictionary<double, Solution> segments = new Dictionary<double, Solution>();

            double segmentCount = 0;
            foreach (var kvp in weights)
            {
                segmentCount += kvp.Value;
                segments.Add(segmentCount, kvp.Key);
            }
            var sortedSegs = segments.Keys.OrderBy(x => x);
            Func<Solution> ReturnParent = () =>
            {
                Random r = new Random();
                Solution Parent = null;
                foreach (var seg in sortedSegs)
                {
                    if (r.NextDouble() < seg)
                    {
                        Parent = segments[seg];
                        break;
                    }
                }
                return Parent;
            };

            watch.Stop();
            var elapsedSeconds = watch.ElapsedMilliseconds / 1000;
            return ReturnParent;
        }
        
    }
}
