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
        Rect _master;
        public BottomLeftBestFitHeuristic()
        {

        }
        public Pattern2d Process()
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
            return pattern;

        }
        void SetSpaces(Pattern2d pattern)
        {
            var spaces = pattern.Spaces = new List<Rect>();

            var y = 0d;


            var allSpacesFound = false;
            while (!allSpacesFound)
            {
                var x = 0d;
                var allBlanks = new List<Rect>(pattern.Blanks);
                allBlanks.AddRange(spaces);
                var blanks = pattern.Blanks;
                allBlanks = blanks;
                var blanksAtY = allBlanks.Where(blank => blank.Y == y);
                blanksAtY.OrderBy(order => order.X);

                //starting at y=0 find any and all spaces
                //a space is start at 0 or location of end of last space
                //get all intercept, order by x ascending

                bool foundAllSpaces = false;
                List<Rect> allXIntercept = null;

                while (foundAllSpaces == false)
                {
                    if (y == 0)
                        allXIntercept = allBlanks.Where(blank => Between(y, blank.Y, blank.Y + blank.Height, true)).ToList();
                    else
                        allXIntercept = allBlanks.Where(blank => Between(y, blank.Y, blank.Y + blank.Height, false)).ToList();

                    if (x >= _master.Width)
                        foundAllSpaces = true;
                    else
                    {
                        var space = new Rect();
                        space.X = x;
                        space.Y = y;
                        var first = allBlanks.Where(blank => blank.X >= x && Between(y, blank.Y, blank.Y + blank.Height, true)).FirstOrDefault();

                        
                        if (first != null)
                            if (first.X == x && first.Y >= y) //no space there
                            {
                                x += first.Width;
                                continue;
                            }

                        var getYGreater = blanks.Where(blank => blank.Y > y).OrderBy(blank => blank.Y);
                        var intercept = getYGreater.Where(blank => Between(x, blank.X, blank.X + blank.Width, true)).OrderBy(blank => blank.Y).FirstOrDefault();
                        if (intercept == null)
                            space.Height = _master.Height - y;
                        else
                            space.Height = intercept.Y - y;

                        //if (first == null)
                        {
                            //find width, first x intercepting if any
                            var greaterX = blanks.Where(blank => blank.X > x).OrderBy(blank=>blank.X);
                            var widthStop = greaterX.Where(blank => Between(y, blank.Y, blank.Y + blank.Height, false) || Between(blank.Y, space.Y, space.Y + space.Height, false) || blank.Y == y);

                            var nextX = widthStop.OrderBy(blank => blank.X).FirstOrDefault();
                            if (nextX == null)
                            {
                                space.Width = _master.Width - x;
                                foundAllSpaces = true;
                            }
                            else
                            {
                                space.Width = nextX.X - space.X;
                                x += space.Width;
                            }

                        }
                        //height


                        spaces.Add(space);
                    }

                }
                /*
                foreach (var b in blanksAtY)
                {
                    //no blanks at it's x value, above it 
                    var spaceLeft = _master.Height - b.Y - b.Height;
                    if (spaceLeft == 0)
                        continue;

                    var blanksAbove = pattern.Blanks.Where(blank => blank.X == b.X && blank.Y > blank.Y);
                    if (blanksAbove.Count() == 0)
                    {
                        var space = new Rect();
                        space.X = b.X;
                        space.Y = b.Y+b.Height;
                        space.Height = spaceLeft;

                        var blanksAtYOrAbove = pattern.Blanks.Where(blank => blank.X > b.X && Between(blank.Y + blank.Height, b.Y, b.Y + b.Height,true));
                        if (blanksAtYOrAbove.Count() > 0)
                        {
                            var mostLeftInTheWay = blanksAtYOrAbove.OrderBy(blank => blank.X).First();
                            space.Width = mostLeftInTheWay.X - space.X;
                        }
                        else
                            space.Width = _master.Width - x;
                        spaces.Add(space);
                    }
                }*/

                //find next y level greater than current
                var greaterYs = pattern.Blanks.Where(blank => blank.Y + blank.Height > y || blank.Y >= y).OrderBy(blank2 => blank2.Y);
                if (greaterYs.Count() == 0)
                {
                    allSpacesFound = true;
                }
                else
                {
                    var f = greaterYs.OrderBy(order => order.Y).OrderBy(order=>order.Height).First();
                    if (f.Y > y)
                        y = f.Y;
                    else
                        y = f.Y + f.Height;

                }

            }
            //var patternsByFurthestRight = pattern.Blanks.OrderByDescending(x => x.X + x.Width).FirstOrDefault();

        }

        public bool Between(double num, double lower, double upper, bool inclusive = false)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
    }
}
