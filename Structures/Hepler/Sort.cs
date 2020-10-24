using Structures.Tree;
using System;
using System.Collections.Generic;

namespace Structures.Hepler
{
    public static class Sort
    {
        //private static int _maxSampleSize = 100;
        //private static Random _rand = new Random();

        /*public static int GetKthIndex<T>(IList<T> data, int dimension, int k) where T : IKDComparable
        {
            int sqrtCount = Convert.ToInt32(Math.Round(Math.Sqrt(data.Count)));
            int sampleSize = (sqrtCount < 1) ? 1 : (sqrtCount > _maxSampleSize) ? _maxSampleSize : sqrtCount;
            var sampleArray = new DataIndex[sampleSize];

            for (int i = 0; i < sampleSize; i++)
            {
                int randIndex = _rand.Next(0, data.Count);
                sampleArray[randIndex] = new DataIndex(randIndex, data[randIndex].GetKey(dimension));
            }

            return QuickSelect(sampleArray, k, 0, sampleSize).Index;
        }*/

        public static void QuickSort<T>(IList<T> data, int dimension, int min, int max) where T : IKDComparable
        {
            var pivot = data[Convert.ToInt32((min + max) / 2)].GetKey(dimension);
            int left = min;
            int right = max;

            do
            {
                while (pivot.CompareTo(data[left].GetKey(dimension)) > 0)
                {
                    left++;
                }
                while (pivot.CompareTo(data[right].GetKey(dimension)) < 0)
                {
                    right--;
                }
                if (left <= right)
                {
                    T temp = data[left];
                    data[left] = data[right];
                    data[right] = temp;
                    left++;
                    right--;
                }
            }
            while (left <= right);

            if (min < right)
            {
                QuickSort(data, dimension, min, right);
            }
            if (left < max)
            {
                QuickSort(data, dimension, left, max);
            }
        }

        /*public static T QuickSelect<T>(T[] data, int k, int min, int max) where T : IComparable
        {
            while (true)
            {
                if (min == max)
                {
                    return data[min];
                }

                int pivotIdx = _rand.Next(min, max + 1);
                pivotIdx = Partition(data, min, max, pivotIdx);

                if (k == pivotIdx)
                {
                    return data[k];
                }
                else if (k < pivotIdx)
                {
                    max = pivotIdx - 1;
                }
                else
                {
                    min = pivotIdx + 1;
                }
            }
        }*/

        /*public static void Swap<T>(ref T source, ref T target)
        {
            T temp = source;
            source = target;
            target = temp;
        }*/

        /*private static int Partition<T>(T[] data, int left, int right, int pivotIdx) where T : IComparable
        {
            var pivot = data[pivotIdx];
            Swap(ref data[pivotIdx], ref data[right]);
            int storeIdx = left;

            for (int i = left; i < right; i++)
            {
                if (data[i].CompareTo(pivot) <= 0)
                {
                    Swap(ref data[storeIdx++], ref data[i]);
                }
            }
            Swap(ref data[storeIdx], ref data[right]);

            return storeIdx;
        }*/

        /*private class DataIndex : IComparable<DataIndex>, IComparable
        {
            public DataIndex(int index, IComparable data) => (Index, Data) = (index, data);

            public int Index { get; }

            public IComparable Data { get; }

            public int CompareTo(DataIndex other) => Data.CompareTo(other.Data);

            public int CompareTo(object obj) => CompareTo((DataIndex)obj);
        }*/
    }
}
