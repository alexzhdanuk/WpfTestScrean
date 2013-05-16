using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfTestScrean
{
    class C2PDrawingMath
    {
        public static int DistanceFromPoints(Point start, Point finish)
        {
            int distace = 0;
            distace = (int)Math.Abs(Math.Pow((finish.X - start.X) * (finish.X - start.X) + (finish.Y - start.Y) * (finish.Y - start.Y), 0.5));

            return distace;
        }
    }
}
