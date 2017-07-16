using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.Models
{
    public class BestFitScrap
    {
        BottomLeftBestFitHeuristic _algservice;
        Dictionary<Pattern2d, bool> _patternComplete;

        List<PatternDemand2d> _patternDemands;


        Pattern2d _lastPatternAdded;
        Rect _lastBlankAdded;


        public BestFitScrap()
        {
            _algservice = new BottomLeftBestFitHeuristic();

        }

        public List<PatternDemand2d> Process(Dictionary<Rect, int> demand, Func<List<Rect>, List<Rect>> sort, Rect master, Predicate<int> rotate)
        {
            _patternComplete = new Dictionary<Pattern2d, bool>();
            _patternDemands = new List<PatternDemand2d>(); //return this
            var addedBlanks = new List<Rect>();
            var residualDemand = new Dictionary<Rect, int>(demand);
            bool allDemandComplete = false;
            Rect newBlank = null;
            while (!allDemandComplete)
            {
                if (residualDemand.Sum(x => x.Value) == 0)
                    break;
                //var orderedSizes = residualDemand.Where(x => x.Value > 0).Select(x => x.Key).OrderBy(x => x.Height * x.Width).ToList(); //ordered by area
                var orderedSizes = sort(residualDemand.Where(x => x.Value > 0).Select(x => x.Key).ToList()).ToList(); //sort blanks, remove demand = 0

                var stack = new Stack<Rect>(orderedSizes);

                //choose pattern to fill based on largest space

                if(_patternDemands.Count(x=>x.Pattern.Blanks.Count ==16) > 0)
                {

                }
                foreach(var pat in _patternDemands)
                {
                    if(!_patternComplete.ContainsKey(pat.Pattern))
                    {

                    }
                }

                var patsBySumOfUsedArea = _patternDemands.OrderBy(x => x.Pattern.Blanks.Sum(z => z.Height * z.Width));
                var parentPatterns = patsBySumOfUsedArea
                    .Where(x => !_patternComplete[x.Pattern])
                    .Select(x => x.Pattern).ToList();//order by largest area of space that's not complete

                var parentPattern = parentPatterns.FirstOrDefault();
                var isNewPattern = false;
                if (parentPattern == null)
                {
                    parentPattern = new Pattern2d();
                    //_patternDemands.Add(new PatternDemand2d { Pattern = parentPattern, Demand = int.MaxValue });
                    //_patternComplete.Add(parentPattern, false);
                    parentPattern.Master = master;
                    parentPattern.SetMasterSpace();
                    isNewPattern = true;

                    parentPatterns.Add(parentPattern);
                }




                //_patternMaxCount.Add(pattern, int.MaxValue);


                var newPattern = parentPattern.GetCopy();
                addedBlanks = new List<Rect>();

                bool masterIsComplete = false;


                bool blankFits = false;
                var tryCount = 0; //used in rotate, rotate first, rotate first 2 etc


                if (stack.Count == 0)
                    masterIsComplete = true;
                else
                {

                    foreach (var p in parentPatterns)
                        foreach (var size in orderedSizes)
                        {
                            newBlank = size;
                            parentPattern = p;
                            newPattern = parentPattern.GetCopy();

                            blankFits = false;
                            if (stack.Count == 0)
                            {

                            }

                            if (newBlank.Width == 48)
                            {

                            }
                            var tempDemand = residualDemand[newBlank];
                            if (tempDemand == 0)
                                continue; //pop next blank


                            var blankCopy = new Rect(size);
                            if (rotate(tryCount))
                                blankCopy.Rotate();

                            //if (pattern.Blanks.Count > 0)
                            //_algservice.SetSpaces(pattern, master);

                            var longSide = blankCopy.Width >= blankCopy.Width ? blankCopy.Width : blankCopy.Y;
                            var shortSide = longSide == blankCopy.Width ? blankCopy.Height : blankCopy.Width;


                            var orderedSpaces = parentPattern.Spaces.OrderBy(x => x.Y).ThenBy(x => x.X);
                            foreach (var space in orderedSpaces)
                            {

                                if (_algservice.FitsWidthToWidth(blankCopy, space)) //
                                    blankFits = true;
                                else if (_algservice.FitsWidthToHeight(blankCopy, space))
                                {
                                    blankCopy.Rotate();
                                    blankFits = true;
                                }

                                if (blankFits == true)
                                {
                                    blankCopy.X = space.X;
                                    blankCopy.Y = space.Y;

                                    newPattern.Blanks.Add(blankCopy);

                                    //if (!addedBlanks.Contains(newBlank))
                                    addedBlanks.Add(newBlank);
                                    tempDemand--;
                                    break;
                                }
                            }
                            if (blankFits)
                                break;
                        }
                    if (!blankFits && !isNewPattern)
                    {

                        newPattern = new Pattern2d();
                        //_patternDemands.Add(new PatternDemand2d { Pattern = parentPattern, Demand = int.MaxValue });
                        //_patternComplete.Add(parentPattern, false);
                        newPattern.Master = master;
                        newPattern.SetMasterSpace();
                        isNewPattern = true;
                        _patternComplete.Add(newPattern, false);
                        AddPatternDemand(newPattern, 1);
                        continue;
                    }
                    if (newPattern.Blanks.Count(x => x.Height == 48 && x.Width == 48) == 1)
                    {

                    }
                    var samePattern = _patternDemands.FirstOrDefault(x => x.Pattern.Equals(newPattern));
                    var parentPatDemand = _patternDemands.FirstOrDefault(x => x.Pattern.Equals(parentPattern));

                    if (samePattern != null)
                    {
                        samePattern.Demand++;
                        residualDemand[newBlank]--;
                        if (!isNewPattern)
                            parentPatDemand.Demand--;
                    }
                    else
                    {
                        if (!isNewPattern)
                        {
                            parentPatDemand.Demand--;

                            AddPatternDemand(newPattern, 1);
                            residualDemand[newBlank]--;
                        }
                        else //new pattern without any same already added
                        {
                            //RemovePatternFromDemands(parentPattern);
                            //_patternComplete.Remove(parentPattern);
                            AddPatternDemand(newPattern, 1);
                            residualDemand[newBlank]--;
                        }

                        _patternComplete.Add(newPattern, false);
                    }
                    if (!isNewPattern && parentPatDemand.Demand == 0)
                    {
                        RemovePatternFromDemands(parentPattern);
                        _patternComplete.Remove(parentPattern);
                    }


                    //SubtractDemand(parentPattern, newPattern, newBlank, residualDemand);


                    _algservice.SetSpaces(parentPattern, master);
                    _algservice.SetSpaces(newPattern, master);

                    var completedPat = new List<Pattern2d>();
                    foreach (var pd in _patternComplete.Where(x => x.Value == false))
                    {
                        if (!DoAnyBlanksFit(pd.Key, residualDemand.Where(x => x.Value > 0).Select(x => x.Key).ToList()))
                            completedPat.Add(pd.Key);
                    }
                    completedPat.ForEach(x => _patternComplete[x] = true);



                }

                _algservice.SetSpaces(parentPattern, master);
                _algservice.SetSpaces(newPattern, master);

                //if (blankFits == true)
                {


                    /*
                    if (masterIsComplete)
                    {
                        if (pattern.Blanks.Count > 0)
                            _algservice.SetSpaces(pattern, master);
                        var max = _algservice.MaxPatterns(pattern, residualDemand, addedBlanks);
                        foreach (var i in addedBlanks)
                        {
                            var sub = max * pattern.Blanks.Count(x => x.Equals(i));
                            residualDemand[i] -= sub;

                        }
                        _patternDemands.Add(new PatternDemand2d { Pattern = pattern, Demand = max });
                        if (residualDemand.Count(x => x.Value > 0) == 0)
                            allDemandComplete = true;
                    }*/

                }


            }

            return _patternDemands;
        }
        bool DoAnyBlanksFit(Pattern2d pattern, List<Rect> blanks)
        {
            foreach (var space in pattern.Spaces)
            {
                foreach (var blank in blanks)
                {
                    if (_algservice.FitsWidthToHeight(blank, space))
                        return true;
                    if (_algservice.FitsWidthToWidth(blank, space))
                        return true;
                }
            }
            return false;
        }/*
        void SubtractDemand(Pattern2d oldPattern, Pattern2d newPattern, Rect newBlank, Dictionary<Rect, int> residualDemand)
        {
            var maxNew = residualDemand[newBlank];
            var maxOld = GetDemand(oldPattern);
            var needKeepOldPattern = maxNew < maxOld ? true : false;

            if (oldPattern.Blanks.Count == 0) //if new pattern
            {
                _patternComplete.Remove(oldPattern);
                RemovePatternFromDemands(oldPattern);
                AddPatternDemand(newPattern, int.MaxValue);
                residualDemand[newBlank]--;
                _patternComplete.Add(newPattern, false);
                _lastPatternAdded = newPattern;
                _lastBlankAdded = newBlank;
                return;
            }
            if (maxOld == int.MaxValue)
            {
                if (_lastPatternAdded == oldPattern && _lastBlankAdded.Equals(newBlank))
                {
                    _patternComplete.Remove(oldPattern);
                    RemovePatternFromDemands(oldPattern);
                    AddPatternDemand(newPattern, maxOld);
                }
                else //maxvalue and not the same
                {

                }
            }

            if (needKeepOldPattern) //using up all of new blank
            {
                _patternDemands.Add(new PatternDemand2d { Pattern = newPattern, Demand = maxNew });
                _patternComplete.Add(newPattern, false);

                _lastPatternAdded = newPattern;
                _lastBlankAdded = newBlank;
            }
            else //max of new >= to the old count, never if the pattern was empty
            {
                _patternComplete.Remove(oldPattern);
                RemovePatternFromDemands(oldPattern);
                AddPatternDemand(newPattern, maxOld);
            }
            residualDemand[newBlank] -= maxNew;            
        }
        */
        int GetDemand(Pattern2d pattern)
        {
            return _patternDemands.FirstOrDefault(x => x.Pattern == pattern).Demand;
        }
        void AddPatternDemand(Pattern2d pattern, int demand)
        {

            _patternDemands.Add(new PatternDemand2d { Pattern = pattern, Demand = demand });
        }
        void SetDemand(Pattern2d pattern, int demand)
        {
            _patternDemands.FirstOrDefault(x => x.Pattern == pattern).Demand = demand;
        }
        void RemovePatternFromDemands(Pattern2d pattern)
        {
            _patternDemands.RemoveAll(x => x.Pattern == pattern);
        }

    }
}
