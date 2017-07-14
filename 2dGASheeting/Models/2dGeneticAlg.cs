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
        int _population = 50;
        Rect _master;
        Dictionary<Rect, int> _demand;
        List<PatternDemand2d> _patternDemands;
        List<Rect> _items;
        double _patternWeight = .1;
        double _masterWeight = .9;
        int _shuffleSolutionCount = 2;
        double additionalPatternSelection = .10;
        double _chanceToShuffle = .1;
        double chanceToReorder = .1;
        int _mutateCount = 2000;
        int _crossoverCount = 5000;
        public _2dGeneticAlg()
        {
            _solutions = new List<Solution>();
        }
        public void UseSampleData()
        {
            _demand = SampleData.GetSampleData1();
            _items = _demand.Keys.ToList();
            _population = 10;
            _master = SampleData.Master1;
        }
        public List<Solution> Process()
        {
            CreateInitialSolutions();
            for (int i = 0; i < _mutateCount; ++i)
            {
                var children = Crossover();
                foreach (var solu in children)
                {
                    RepairSolution(solu);
                    _solutions.Add(solu);
                    SetFitness();
                    var min = _solutions.Where(x=> _solutions.Min(y=>y.Fitness) == x.Fitness).First();
                    if (solu.Fitness > min.Fitness)
                    {                        
                        _solutions.Remove(min);
                    }
                    else
                        _solutions.Remove(solu);
                }
                SetFitness();
                Mutate();
            }
            return _solutions.OrderBy(x => x.MasterCount).ToList();
        }
        void Mutate()
        {
            var bf = new BottomLeftBestFitHeuristic(_demand, _master, null);
            var getParent = GetSelectParentFn();
            var parent = getParent();
            Random rn = new Random();
            var length = parent.PatternDemands.Count;
            var indexToShuffle = rn.Next(0, length);
            var patterns = parent.Patterns;
            var pd = parent.PatternDemands[indexToShuffle];
            var pattern = patterns[indexToShuffle];
            var shuffleRan = rn.NextDouble();

            if (shuffleRan <= _chanceToShuffle)//shuffle
            {
                
                pd.Pattern= bf.Shuffle(pd.Pattern);
                RepairSolution(parent);
            }
            else//rotate
            {
                shuffleRan = rn.NextDouble();
                indexToShuffle = rn.Next(0, length);
                patterns = parent.Patterns;
                pd = parent.PatternDemands[indexToShuffle];
                pattern = patterns[indexToShuffle];

                if ((shuffleRan <= _chanceToShuffle))
                {
                    parent.PatternDemands.Shuffle();
                    RepairSolution(parent);
                }
            }
            

            SetFitness();
        }
        void RepairSolution(Solution solution)
        {
            var newDemand = new Dictionary<Rect, int>(_demand);
            int zeroPats = 0;
            var remove = new List<PatternDemand2d>();
            foreach (var pd in solution.PatternDemands)
            {
                int max = MaxPatterns(pd.Pattern, newDemand);

                if (max <= 0)
                {
                    remove.Add(pd);
                }
                else
                {
                    var distinct = pd.Pattern.Blanks.Distinct(new RectComparer());
                    foreach (var i in distinct)
                    {
                        var demRef = newDemand.Where(x => x.Key.Equals(i)).FirstOrDefault().Key;
                        var sub = max * pd.Pattern.Blanks.Count(x => x.Width == i.Width && x.Height == i.Height);
                        newDemand[demRef] -= sub;
                    }
                }
            }
            foreach (var r in remove)
                solution.PatternDemands.Remove(r);

            if (newDemand.Sum(x => x.Value) > 0)
            {
                Func<List<Rect>, List<Rect>> sort;
                sort = (t) =>
                {
                    return t.OrderBy(x => x.Height * x.Width).ToList();
                };
                var bestFit = new BottomLeftBestFitHeuristic(newDemand, _master, sort);

                var newPD = bestFit.Process();

                foreach (var pd in newPD)
                    solution.PatternDemands.Add(pd);
            }
        }
        int MaxPatterns(Pattern2d pattern, Dictionary<Rect, int> residualDemand)
        {
            int max = int.MaxValue;
            foreach (var item in pattern.Blanks)
            {
                var demRef = residualDemand.Where(x => x.Key.Equals(item)).FirstOrDefault().Key;
                var count = pattern.Blanks.Count(x => x.Width == item.Width && x.Height == item.Height);
                var demand = residualDemand[demRef];
                var thisMax = demand / count;
                if (thisMax < max)
                    max = thisMax;
            }
            return max;
        }
        public void CreateInitialSolutions()
        {
            List<PatternDemand2d> allSolutions = new List<PatternDemand2d>();

            var sol = new Solution();
            Func<List<Rect>, List<Rect>> _sort;
            _sort = (t) =>
            {
                return t.OrderBy(x => x.Height * x.Width).ToList();
            };

            var BLBF = new BottomLeftBestFitHeuristic(_demand, _master, _sort);
            sol.PatternDemands = BLBF.Process();
            _solutions.Add(sol);

            sol = new Solution();
            _sort = (t) =>
            {
                return t.OrderBy(x => x.Height).ToList();
            };

            BLBF = new BottomLeftBestFitHeuristic(_demand, _master, _sort);
            sol.PatternDemands = BLBF.Process();
            _solutions.Add(sol);

            sol = new Solution();
            _sort = (t) =>
            {
                return t.OrderBy(x => x.Width).ToList();
            };

            BLBF = new BottomLeftBestFitHeuristic(_demand, _master, _sort);
            sol.PatternDemands = BLBF.Process();
            _solutions.Add(sol);

            SetFitness();
            while (_solutions.Count < _population)
            {
                var children = Crossover();
                foreach (var solu in children)
                {
                    RepairSolution(solu);
                    _solutions.Add(solu);
                }
                SetFitness();
            }

            var pds = _solutions[0].PatternDemands;
            var bf = new BottomLeftBestFitHeuristic(_demand, _master, null);

            var shufMe = pds.First();
            var shuffled = bf.Shuffle(shufMe.Pattern);
            shufMe.Pattern = shuffled;
            _solutions.Reverse();
            var shuffleSolutions = _solutions.Take(_shuffleSolutionCount);


            foreach (var s in shuffleSolutions)
            {
                foreach (var pd in s.PatternDemands)
                {
                    pd.Pattern = bf.Shuffle(pd.Pattern);
                }
                RepairSolution(s);
            }
            SetFitness();
            //patterns.Remove(shufMe);
            //patterns.Insert(0, shuffled);
            //return _solutions.OrderBy(x=>x.MasterCount).ToList();
        }
        void SetFitness()
        {
            SetMasterCountFitness();
            SetPatternCountFitness();
        }
        void SetMasterCountFitness()
        {
            var totalMaster = _solutions.Sum(x => x.MasterCount);
            var denom = _solutions.Sum(x => totalMaster - x.MasterCount);
            foreach (var sol in _solutions)
            {
                double num = totalMaster - sol.MasterCount;
                double fit = num / denom;
                sol.Fitness += fit * _masterWeight;
            }
        }

        void SetPatternCountFitness()
        {
            var totalMaster = _solutions.Sum(x => x.PatternCount);
            var denom = _solutions.Sum(x => totalMaster - x.PatternCount);
            foreach (var sol in _solutions)
            {
                double num = totalMaster - sol.PatternCount;
                double fit = num / denom;
                sol.Fitness += fit * _patternWeight;
            }
        }

        List<Solution> Crossover()
        {
            var getParent = GetSelectParentFn();
            var parent1 = getParent();
            var pd1 = parent1.PatternDemands;


            var parent2 = getParent();
            while (parent2 == parent1)
                parent2 = getParent();

            var pd2 = parent2.PatternDemands;

            var newSolution1 = new Solution();
            var newSolution2 = new Solution();

            var rn = new Random();
            var num = rn.Next(1, 5); // between 1 and 5 swaps;

            var lesser = Math.Min(parent1.PatternCount, parent2.PatternCount);

            var allNums = new List<int>();
            for (int i = 0; i < lesser; ++i)
            {
                allNums.Add(i);
            }

            var rnSwapPositions = allNums.Shuffle().Take(num);

            int x = 0;
            foreach (var pd in parent1.PatternDemands)
            {
                var newPd = new PatternDemand2d();
                if (rnSwapPositions.Contains(x))
                {
                    newPd.Demand = pd2[x].Demand;
                    newPd.Pattern = pd2[x].Pattern;
                }
                else
                {
                    newPd.Demand = pd.Demand;
                    newPd.Pattern = pd.Pattern;
                }
                newSolution1.PatternDemands.Add(newPd);
                ++x;
            }
            x = 0;
            foreach (var pd in parent2.PatternDemands)
            {
                var newPd = new PatternDemand2d();
                if (rnSwapPositions.Contains(x))
                {
                    newPd.Demand = pd1[x].Demand;
                    newPd.Pattern = pd1[x].Pattern;
                }
                else
                {
                    newPd.Demand = pd.Demand;
                    newPd.Pattern = pd.Pattern;
                }
                newSolution2.PatternDemands.Add(newPd);
                ++x;
            }
            var newSols = new List<Solution>();

            newSols.Add(newSolution1);
            newSols.Add(newSolution2);
            return newSols;

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
