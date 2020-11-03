using System;
using System.Collections.Generic;
using System.IO;

namespace Structures.Hepler
{
    public static class Extensions
    {
        /// <summary>
        /// Yields all lines of string line by line
        /// </summary>
        /// <param name="s">String to be enumerated</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadLines(this string s)
        {
            string line;

            using var stringReader = new StringReader(s);
            while ((line = stringReader.ReadLine()) != null)
                yield return line;
        }

        /// <summary>
        /// Compares two <see cref="IEnumerable{T}"/> pair-wise with custom predicate function
        /// </summary>
        /// <typeparam name="T">Type of elements in <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="source">Source <see cref="IEnumerable{T}"/></param>
        /// <param name="target">Target <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">Custom predicate function used for element comparison</param>
        /// <returns></returns>
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
