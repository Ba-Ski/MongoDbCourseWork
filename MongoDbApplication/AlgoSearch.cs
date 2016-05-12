using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbApplication
{
    public static class AlgoSearch
    {
        public static int LevenshteinDist(string string1, string string2)
        {
            if (string1 == null) throw new ArgumentNullException("string1");
            if (string2 == null) throw new ArgumentNullException("string2");
            int diff;
            int[,] m = new int[string1.Length + 1, string2.Length + 1];
            for (int i = 0; i <= string1.Length; i++) m[i, 0] = i;
            for (int j = 0; j <= string2.Length; j++) m[0, j] = j;
            for (int i = 1; i <= string1.Length; i++)
            {
                for (int j = 1; j <= string2.Length; j++)
                {
                    diff = (string1[i - 1] == string2[j - 1]) ? 0 : 1;
                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1,
                    m[i, j - 1] + 1),
                    m[i - 1, j - 1] + diff);
                }
            }
            return m[string1.Length, string2.Length];
        }
        private static readonly double mWeightThreshold = 0.7;
        private static readonly int mNumChars = 4;
        public static double JWDist(string string1, string string2)
        {
            return proximity(string1, string2);
        }
        private static double proximity(string aString1, string aString2)
        {
            int lLen1 = aString1.Length, lLen2 = aString2.Length;
            if (lLen1 == 0) return lLen2 == 0 ? 1.0 : 0.0;
            int lSearchRange = Math.Max(0, Math.Max(lLen1, lLen2) / 2 - 1);
            bool[] lMatched2 = new bool[lLen2];
            for (int i = 0; i < lMatched2.Length; i++)
            {
                lMatched2[i] = false;
            }
            bool[] lMatched1 = new bool[lLen1];
            for (int i = 0; i < lMatched1.Length; i++)
            {
                lMatched1[i] = false;
            }
            int Common = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                int lStart = Math.Max(0, i - lSearchRange);
                int lEnd = Math.Min(i + lSearchRange + 1, lLen2);
                for (int j = lStart; j < lEnd; ++j)
                {
                    if (lMatched2[j]) continue;
                    if (aString1[i] != aString2[j])
                        continue;
                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++Common;
                    break;
                }
            }
            if (Common == 0) return 0.0;
            int HalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                if (!lMatched1[i]) continue;
                while (!lMatched2[k]) ++k;
                if (aString1[i] != aString2[k])
                    ++HalfTransposed;
                ++k;
            }
            int Transposed = HalfTransposed / 2;
            double CommonD = Common;
            double lWeight = (CommonD / lLen1 + CommonD / lLen2 + (Common - Transposed) / CommonD) / 3.0;
            if (lWeight <= mWeightThreshold) return lWeight;
            int lMax = Math.Min(mNumChars, Math.Min(aString1.Length, aString2.Length));
            int lPos = 0;
            while (lPos < lMax && aString1[lPos] == aString2[lPos]) ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);
        }
        public static int HamDist(string source, string target)
        {
            if (source.Length != target.Length)
            {
                throw new Exception("Strings must be equal length");
            }
            int distance = source.ToCharArray().Zip(target.ToCharArray(), (c1, c2) => new { c1, c2 }).Count(m => m.c1 != m.c2);
            return distance;
        }
    }
}


