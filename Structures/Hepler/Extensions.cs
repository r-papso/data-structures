using System;
using System.Collections.Generic;
using System.IO;

namespace Structures.Hepler
{
    public static class Extensions
    {
        public static IEnumerable<string> ReadLines(this string s)
        {
            string line;

            using var stringReader = new StringReader(s);
            while ((line = stringReader.ReadLine()) != null)
                yield return line;
        }

        public static bool ComparePairWise<T>(this IEnumerable<T> source, IEnumerable<T> target, Func<T, T, bool> predicate)
        {
            var sourceEnumerator = source.GetEnumerator();
            var targetEnumerator = target.GetEnumerator();

            while (sourceEnumerator.MoveNext())
            {
                if (!targetEnumerator.MoveNext())
                    return false;

                if (!predicate(sourceEnumerator.Current, targetEnumerator.Current))
                    return false;
            }

            if (targetEnumerator.MoveNext())
                return false;

            return true;
        }
    }
}
