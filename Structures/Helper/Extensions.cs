using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Structures.Helper
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

        /// <summary>
        /// Returns index of first element of <paramref name="source"/> satisfying <paramref name="predicate"/> conditon, or -1 if no such element exists in <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="source">Source <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">Predicate, when satisfied, index of element is returned</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int index = 0;

            foreach (var obj in source)
            {
                if (predicate(obj))
                    return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Shuffles the <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in <paramref name="source"/></typeparam>
        /// <param name="source"><see cref="IEnumerable{T}"/> to be shuffled</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => Shuffle(source, new Random());

        /// <summary>
        /// Shuffles the <paramref name="source"/> with specific random number generator
        /// </summary>
        /// <typeparam name="T">Type of elements in <paramref name="source"/></typeparam>
        /// <param name="source"><see cref="IEnumerable{T}"/> to be shuffled</param>
        /// <param name="rand">Random number generator used within shuffling</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rand)
        {
            var list = source.ToList();
            var target = new List<T>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                int j = rand.Next(i, list.Count);
                target.Add(list[j]);
                list[j] = list[i];
            }

            return target;
        }

        /// <summary>
        /// Converts <paramref name="bitArray"/> to integer
        /// </summary>
        /// <param name="bitArray"><see cref="BitArray"/> to be converted</param>
        /// <returns>Integer representation of <paramref name="bitArray"/></returns>
        public static int ToInt(this BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException("BitArray length longer than 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        /// <summary>
        /// Replaces specific <paramref name="subset"/> in <paramref name="source"/> array
        /// </summary>
        /// <typeparam name="T">Type of elements in array</typeparam>
        /// <param name="source">Source array, its length must be greater than or equal to subsets' array length</param>
        /// <param name="subset">Replacing subset</param>
        /// <param name="offset">Start index at <paramref name="source"/> array where replacing will begin</param>
        public static void ReplaceRange<T>(this T[] source, T[] subset, int offset)
        {
            if (source.Length < subset.Length)
                throw new ArgumentException("Subset length greater than source length");

            if (offset < 0 || subset.Length + offset > source.Length)
                throw new ArgumentException("Offset out of range");

            for (int i = 0; i < subset.Length; i++)
                source[i + offset] = subset[i];
        }

        /// <summary>
        /// Converts string to byte array
        /// </summary>
        /// <param name="str">String to be converted</param>
        /// <returns>Byte array representation of string</returns>
        public static byte[] GetBytes(this string str)
        {
            var result = new byte[sizeof(char) * str.Length];
            var offset = 0;

            for (int i = 0; i < str.Length; i++)
            {
                var bytes = BitConverter.GetBytes(str[i]);
                result.ReplaceRange(bytes, offset);
                offset += bytes.Length;
            }

            return result;
        }

        /// <summary>
        /// Converts byte array to string
        /// </summary>
        /// <param name="arr">Byte array to be converted</param>
        /// <param name="offset">Start index in array where conversion will begin</param>
        /// <param name="length">Total bytes to be converted</param>
        /// <returns></returns>
        public static string ToString(this byte[] arr, int offset, int length)
        {
            var result = string.Empty;

            for (int i = 0; i < length; i++)
            {
                result += BitConverter.ToChar(arr, offset);
                offset += sizeof(char);
            }

            return result;
        }
    }
}
