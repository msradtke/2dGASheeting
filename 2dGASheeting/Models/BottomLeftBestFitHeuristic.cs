using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace _2dGASheeting.Models
{
    public class BottomLeftBestFitHeuristic
    {
        List<Rectangle> Spaces;
        Dictionary<Rect, int> _demand;
        List<PatternDemand2d> _patternDemands;
        Rect _master;
        public BottomLeftBestFitHeuristic(Dictionary<Rect, int> demand, Rect  master)
        {
            _demand = demand;
            _master = master;
        }
        public void ProcessOld()
        {
            _master = new Rect();
            _master.Width = 96;
            _master.Height = 48;
            Pattern2d pattern = new Pattern2d();
            var rect1 = new Rect();
            rect1.X = 0;
            rect1.Y = 0;
            rect1.Height = 10;
            rect1.Width = 20;
            var rect2 = new Rect();
            rect2.X = 20;
            rect2.Y = 0;
            rect2.Height = 10;
            rect2.Width = 20;
            var rect3 = new Rect();
            rect3.X = 20;
            rect3.Y = 20;
            rect3.Height = 27;
            rect3.Width = 55;
            var rect4 = new Rect();
            rect4.X = 77;
            rect4.Y = 20;
            rect4.Height = 10;
            rect4.Width = 10;
            pattern.Blanks = new List<Rect>();
            pattern.Blanks.Add(rect1);
            pattern.Blanks.Add(rect2);
            pattern.Blanks.Add(rect3);
            pattern.Blanks.Add(rect4);
            SetSpaces(pattern);
        }
        public List<PatternDemand2d> Process()
        {
            _patternDemands = new List<PatternDemand2d>();

            var residualDemand = new Dictionary<Rect, int>(_demand);
            bool allDemandComplete = false;
            Rect newBlank = null;
            while (!allDemandComplete)
            {
                var orderedSizes = residualDemand.Where(x => x.Value > 0).Select(x => x.Key).OrderBy(x => x.Height * x.Width).ToList();
                var stack = new Stack<Rect>(orderedSizes);
                var pattern = new Pattern2d();
                pattern.Master = _master;
                pattern.SetMasterSpace();
                bool masterIsComplete = false;
                while (!masterIsComplete)
                {
                    if (stack.Count == 0) 
                        masterIsComplete = true;
                    else
                    {
                        newBlank = stack.Pop();
                        var blank = new Rect(newBlank);
                        if (residualDemand[newBlank] == 0)
                            continue;
                        bool blankFits = true;

                        while (blankFits)
                        {
                            if(pattern.Blanks.Count >0)
                                SetSpaces(pattern);
                            
                            blankFits = false;
                            var longSide = blank.Width >= blank.Width ? blank.Width : blank.Y;
                            var shortSide = longSide == blank.Width ? blank.Height : blank.Width;

                            var orderedSpaces = pattern.Spaces.OrderBy(x => x.Y).ThenBy(x => x.X);
                            foreach (var space in orderedSpaces)
                            {
                                if (FitsLongToHeight(blank, space, longSide, shortSide))
                                    blankFits = true;
                                else if (FitsLongToHeight(blank, space, longSide, shortSide))
                                    blankFits = true;

                                if (blankFits == true)
                                {
                                    blank.X = space.X;
                                    blank.Y = space.Y;
                                    pattern.Blanks.Add(blank);
                                    //residualDemand[blank]--;
                                    break;
                                }
                            }

                        }
                        
                    }
                    if (masterIsComplete)
                    {
                        var max = MaxPatterns(pattern, residualDemand,newBlank);
                        var distinct = pattern.GetDistinct();
                        foreach (var i in distinct)
                        {
                            
                            residualDemand[newBlank] -= max*pattern.Blanks.Count(x=>x.X == i.X && x.Y == i.Y);
                            _patternDemands.Add(new PatternDemand2d { Pattern = pattern, Demand = max });
                        }
                        if (residualDemand.Count(x => x.Value > 0) == 0)
                            allDemandComplete = true;
                    }
                }

            }
            return _patternDemands;
        }
        int MaxPatterns(Pattern2d pattern, Dictionary<Rect, int> residualDemand, Rect demandReference)
        {
            int max = int.MaxValue;
            List<Rect> used = new List<Rect>();
            foreach(var item in pattern.Blanks)
            {
                int useCnt = 0;
                useCnt = used.Count(x => x.Width == item.Width && x.Height == item.Height);
                if (useCnt == 0)
                {
                    used.Add(item);
                    var count = pattern.Blanks.Count(x => x.Width == item.Width && x.Height == item.Height);

                    
                    var demand = residualDemand[demandReference];
                    var thisMax = demand / count;
                    if (thisMax < max)
                        max = thisMax;
                }                
            }

            return max;
        }
        bool FitsLongToHeight(Rect blank, Rect space, double longSide, double shortSide)
        {
            if (longSide <= space.Height && shortSide <= space.Width)
                return true;
            return false;
        }
        bool FitsShortToHeight(Rect blank, Rect space, double longSide, double shortSide)
        {
            if (longSide <= space.Width && shortSide <= space.Height)
                return true;
            return false;
        }

        void SetSpaces(Pattern2d pattern)
        {
            var spaces = pattern.Spaces = new List<Rect>();
            var blanks = pattern.Blanks;
            var sortedBlanks = blanks.OrderBy(blank => blank.X).ThenBy(blank => blank.Y);

            foreach (var blank in sortedBlanks)
            {
                var space = new Rect();
                space.X = blank.X;
                space.Y = blank.Y + blank.Height;
                var firstBlankAbove = GetFirstBlockGoingUp(blank, blanks);
                if (firstBlankAbove == null)
                    space.Height = _master.Height - space.Y;
                else
                {
                    space.Height = firstBlankAbove.Y - space.Y;
                }

                var firstBlankRight = GetFirstBlockGoingRight(space, blanks);
                if (firstBlankRight == null)
                    space.Width = _master.Width - space.X;
                else
                {
                    space.Width = firstBlankRight.X - space.X;
                }
                if (space.Height > 0 && space.Width > 0)
                    spaces.Add(space);

                space = new Rect();
                space.X = blank.X + blank.Width;
                space.Y = blank.Y;
                firstBlankRight = GetFirstBlockGoingRight(blank, blanks);
                if (firstBlankRight == null)
                    space.Width = _master.Width - space.X;
                else
                {
                    space.Width = firstBlankRight.X - space.X;
                }
                if (space.Height > 0 && space.Width > 0)
                    spaces.Add(space);

                firstBlankAbove = GetFirstBlockGoingUp(space, blanks);
                if (firstBlankAbove == null)
                    space.Height = _master.Height - space.Y;
                else
                {
                    space.Height = firstBlankAbove.Y - space.Y;
                }
                if (space.Height > 0 && space.Width > 0)
                    spaces.Add(space);
            }

        }
        Rect GetFirstBlockGoingUp(Rect blank, List<Rect> blanks)
        {
            var blocking = new List<Rect>();

            var blanksAbove = blanks.Where(x => x.Y >= blank.Y && blank != x);
            foreach (var blocker in blanksAbove)
            {
                if (Between(blank.X, blocker.X, blocker.X + blocker.Width, false))
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (Between(blocker.X, blank.X, blank.X + blank.Width, false))
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (blank.X >= blocker.X && blocker.X + blocker.Width > blank.X)
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (blank.X == blocker.X && blocker.Width > 0)
                {
                    blocking.Add(blocker);
                    continue;
                }
            }

            var firstblocker = blocking.OrderBy(x => x.X).FirstOrDefault();
            return firstblocker;
        }
        Rect GetFirstBlockGoingRight(Rect blank, List<Rect> blanks)
        {
            var blocking = new List<Rect>();
            var blanksRight = blanks.Where(x => x.X >= blank.X && blank != x);
            foreach (var blocker in blanksRight)
            {
                if (Between(blank.Y, blocker.Y, blocker.Y + blocker.Height, false))
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (Between(blocker.Y, blank.Y, blank.Y + blank.Height, false))
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (blank.Y >= blocker.Y && blocker.Y + blocker.Height > blank.Y)
                {
                    blocking.Add(blocker);
                    continue;
                }
                if (blank.Y == blocker.Y && blocker.Height > 0)
                {
                    blocking.Add(blocker);
                    continue;
                }
            }

            var firstblocker = blocking.OrderBy(x => x.Y).FirstOrDefault();
            return firstblocker;
        }

        public bool Between(double num, double lower, double upper, bool inclusive = false)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }


    }

    public class RectComparer : Comparer<Rect>
    {
        public override int Compare(Rect x, Rect y)
        {
            throw new NotImplementedException();
        }

    }
}
