﻿using System;
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
        Func<List<Rect>, List<Rect>> _sort;
        public BottomLeftBestFitHeuristic(Dictionary<Rect, int> demand, Rect master, Func<List<Rect>, List<Rect>> sort)
        {
            _demand = demand;
            _master = master;
            _sort = sort;
        }
        public BottomLeftBestFitHeuristic()
        {

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
            SetSpaces(pattern, _master);
        }
        public List<PatternDemand2d> Process()
        {
            _patternDemands = new List<PatternDemand2d>();
            var addedBlanks = new List<Rect>();
            var residualDemand = new Dictionary<Rect, int>(_demand);
            bool allDemandComplete = false;
            Rect newBlank = null;
            while (!allDemandComplete)
            {
                //var orderedSizes = residualDemand.Where(x => x.Value > 0).Select(x => x.Key).OrderBy(x => x.Height * x.Width).ToList(); //ordered by area
                var orderedSizes = _sort(residualDemand.Where(x => x.Value > 0).Select(x => x.Key).ToList()).ToList();

                var stack = new Stack<Rect>(orderedSizes);
                stack.Reverse();

                var pattern = new Pattern2d();
                addedBlanks = new List<Rect>();
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
                        var tempDemand = residualDemand[newBlank];
                        if (tempDemand == 0)
                            continue;
                        bool blankFits = true;


                        while (blankFits)
                        {
                            var blankCopy = new Rect(newBlank);
                            if (pattern.Blanks.Count > 0)
                                SetSpaces(pattern, _master);

                            blankFits = false;
                            var longSide = blankCopy.Width >= blankCopy.Width ? blankCopy.Width : blankCopy.Y;
                            var shortSide = longSide == blankCopy.Width ? blankCopy.Height : blankCopy.Width;

                            var orderedSpaces = pattern.Spaces.OrderBy(x => x.Y).ThenBy(x => x.X);
                            foreach (var space in orderedSpaces)
                            {
                                if (FitsLongToHeight(blankCopy, space, longSide, shortSide)) //
                                    blankFits = true;
                                else if (FitsShortToHeight(blankCopy, space, longSide, shortSide))
                                    blankFits = true;

                                if (blankFits == true)
                                {
                                    blankCopy.X = space.X;
                                    blankCopy.Y = space.Y;
                                    pattern.Blanks.Add(blankCopy);
                                    if (!addedBlanks.Contains(newBlank))
                                        addedBlanks.Add(newBlank);
                                    tempDemand--;
                                    break;
                                }
                            }
                            if (tempDemand == 0)
                                break;

                        }

                    }
                    if (masterIsComplete)
                    {
                        if (pattern.Blanks.Count > 0)
                            SetSpaces(pattern, _master);
                        var max = MaxPatterns(pattern, residualDemand, addedBlanks);
                        foreach (var i in addedBlanks)
                        {
                            var sub = max * pattern.Blanks.Count(x => x.Equals(i));
                            residualDemand[i] -= sub;

                        }
                        _patternDemands.Add(new PatternDemand2d { Pattern = pattern, Demand = max });
                        if (residualDemand.Count(x => x.Value > 0) == 0)
                            allDemandComplete = true;
                    }
                }

            }
            return _patternDemands;
        }

        public Pattern2d Shuffle(Pattern2d pattern)
        {
            //demand should be set to the removed pattern 
            Pattern2d shuffled = new Pattern2d();
            shuffled.Master = _master;

            List<Rect> randBlanks = new List<Rect>(pattern.Blanks);
            List<Func<Rect, Rect, double, double, bool>> fitFns = new List<Func<Rect, Rect, double, double, bool>>();
            fitFns.Add(FitsLongToHeight);
            fitFns.Add(FitsShortToHeight);

            Random ran = new Random();
            var ranBlanks = randBlanks.Shuffle();
            bool added = true;
            for (int i = 0; i < ranBlanks.Length; ++i)
            {
                List<int> fitFunHelper = new List<int> { 0, 1 };
                var blank = ranBlanks[i];
                var longSide = blank.Width >= blank.Width ? blank.Width : blank.Y;
                var shortSide = longSide == blank.Width ? blank.Height : blank.Width;

                if (added == true)
                    SetSpaces(shuffled, _master);
                added = false;
                var shuSpaces = shuffled.Spaces.Shuffle();
                if (shuSpaces.Length == 0)
                    break;
                var spaceIndex = ran.Next(0, shuSpaces.Length);
                var space = shuSpaces[spaceIndex];
                var fitnum = ran.Next(0, 2);
                if (fitFns[fitnum](blank, space, longSide, shortSide))
                {
                    var newBlank = new Rect(blank);
                    newBlank.X = space.X;
                    newBlank.Y = space.Y;
                    shuffled.Blanks.Add(newBlank);
                    added = true;
                }
                else
                {
                    fitFunHelper.Remove(fitnum);
                    if ((fitFns[fitFunHelper.First()](blank, space, longSide, shortSide)))
                    {
                        var newBlank = new Rect(blank);
                        newBlank.X = space.X;
                        newBlank.Y = space.Y;
                        shuffled.Blanks.Add(newBlank);
                        added = true;
                    }
                }

            }
            SetSpaces(shuffled, _master);
            return shuffled;
        }
        public int MaxPatterns(Pattern2d pattern, Dictionary<Rect, int> residualDemand, List<Rect> demandReference)
        {
            int max = int.MaxValue;
            foreach (var item in demandReference)
            {

                var count = pattern.Blanks.Count(x => x.Equals(item));


                var demand = residualDemand[item];
                var thisMax = demand / count;
                if (thisMax < max)
                    max = thisMax;

            }

            return max;
        }
        public bool FitsLongToHeight(Rect blank, Rect space, double longSide, double shortSide)
        {
            if (longSide <= space.Height && shortSide <= space.Width)
                return true;
            return false;
        }
        public bool FitsShortToHeight(Rect blank, Rect space, double longSide, double shortSide)
        {
            if (longSide <= space.Width && shortSide <= space.Height)
                return true;
            return false;
        }
        public bool FitsWidthToWidth(Rect blank, Rect space)
        {
            if (blank.Width <= space.Width && blank.Height <= space.Height)
                return true;
            return false;
        }
        public bool FitsWidthToHeight(Rect blank, Rect space)
        {
            if (blank.Width <= space.Height && blank.Height <= space.Width)
                return true;
            return false;
        }
        public void SetSpaces(Pattern2d pattern, Rect master)
        {

            var spaces = pattern.Spaces = new List<Rect>();
            var blanks = pattern.Blanks;
            var sortedBlanks = blanks.OrderBy(blank => blank.X).ThenBy(blank => blank.Y);


            if(blanks.Exists(x=>x.Height == 1))
            {

            }

            if (blanks.Count == 0)
            {
                var space = new Rect();
                space.X = 0;
                space.Y = 0;
                space.Width = master.Width;
                space.Height = master.Height;
                spaces.Add(space);
            }
            var rotate = false;
            foreach (var blank in sortedBlanks)
            {
                if (blank.Height > blank.Width)
                {
                    blank.Rotate();
                    rotate = true;
                }
                var space = new Rect();
                space.X = blank.X;
                space.Y = blank.Y + blank.Height;
                var firstBlankAbove = GetFirstBlockGoingUp(blank, blanks);
                if (firstBlankAbove == null)
                    space.Height = master.Height - space.Y;
                else
                {
                    space.Height = firstBlankAbove.Y - space.Y;
                }
                var firstBlankRight = GetFirstBlockGoingRight(space, blanks);
                if (firstBlankRight == null)
                    space.Width = master.Width - space.X;
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
                    space.Width = master.Width - space.X;
                else
                {
                    space.Width = firstBlankRight.X - space.X;
                }
                if (space.Height > 0 && space.Width > 0)
                    spaces.Add(space);

                firstBlankAbove = GetFirstBlockGoingUp(space, blanks);
                if (firstBlankAbove == null)
                    space.Height = master.Height - space.Y;
                else
                {
                    space.Height = firstBlankAbove.Y - space.Y;
                }
                if (space.Height > 0 && space.Width > 0)
                    spaces.Add(space);

                if (rotate)
                {
                    blank.Rotate();
                }
            }

            foreach (var blank in pattern.Blanks)
            {
                if (pattern.Spaces.Count(x => x.X == blank.X && x.Y == blank.Y) > 0)
                {

                }
            }
        }
        Rect GetFirstBlockGoingUp(Rect blank, List<Rect> blanks)
        {
            var blocking = new List<Rect>();

            var blanksAbove = blanks.Where(x => x.Y >= blank.Y + blank.Height && !blank.IsExactlyEqual(x));
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
            var firstblocker = blocking.OrderBy(x => x.Y).ThenBy(x => x.X).FirstOrDefault();
            return firstblocker;
        }
        Rect GetFirstBlockGoingRight(Rect blank, List<Rect> blanks)
        {
            if (blanks.Count > 13)
            {

            }
            var blocking = new List<Rect>();
            var blanksRight = blanks.Where(x => x.X >= blank.X && !blank.IsExactlyEqual(x));
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

            var firstblocker = blocking.OrderBy(x => x.X).ThenBy(x => x.Y).FirstOrDefault();
            return firstblocker;
        }

        public bool Between(double num, double lower, double upper, bool inclusive = false)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }


    }

}
