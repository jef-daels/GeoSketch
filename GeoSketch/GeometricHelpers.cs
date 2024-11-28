using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSketch
{
    /// <summary>
    /// used to sort Vector2 instances
    /// </summary>
    public class Vector2ComparerHighToLow : IComparer<Vector2>
    {
        /// <summary>
        /// compare 2 vectors, higher means smaller
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Vector2 x, Vector2 y)
        {
            //lower y values means higher point => smaller (before) in the array
            if (x.Y < y.Y)
                return -1;
            else if (x.Y == y.Y)
                return 0;
            else
                return 1;
        }
    }
    internal class Vector2ComparerLeftToRight : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            //lower y values means higher point => smaller (before) in the array
            if (x.X < y.X)
                return -1;
            else if (x.X == y.X)
                return 0;
            else
                return 1;
        }
    }

    internal class GeometricHelpers
    {
        /// <summary>
        /// returns the first highest point in an array
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static void SortVector2HighToLow(Vector2[] vectors)
        {
            SortVector2(vectors, new Vector2ComparerHighToLow());
        }
        public static void SortVector2LeftToRight(Vector2[] vectors)
        {
            SortVector2(vectors, new Vector2ComparerLeftToRight());
        }
        public static (Vector2 left, Vector2 right) SortVector2LeftToRight(Vector2 v1, Vector2 v2)
        {
            Vector2[] vectors = new Vector2[] { v1, v2 };
            SortVector2(vectors, new Vector2ComparerLeftToRight());
            return (vectors[0], vectors[1]);
        }
        public static (Vector2 high, Vector2 low) SortVector2HighToLow(Vector2 v1, Vector2 v2)
        {
            Vector2[] vectors = new Vector2[] { v1, v2 };
            SortVector2(vectors, new Vector2ComparerHighToLow());
            return (vectors[0], vectors[1]);
        }


        private static void SortVector2(Vector2[] points, IComparer<Vector2> pointSorter)
        {
            Array.Sort<Vector2>(points, pointSorter);
        }
    }
}
