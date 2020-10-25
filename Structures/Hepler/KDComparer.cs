using Structures.Tree;
using System.Collections.Generic;

namespace Structures.Hepler
{
    public class KdComparer<T> : IComparer<T> where T : IKdComparable
    {
        public int Dimension { get; set; }

        public KdComparer() { }

        public KdComparer(int dimension) => Dimension = dimension;

        public int Compare(T left, T right)
        {
            return left.GetKey(Dimension).CompareTo(right.GetKey(Dimension));
        }

        public int Compare(T left, T right, int dimension)
        {
            return left.GetKey(dimension).CompareTo(right.GetKey(dimension));
        }

        public bool Between(T data, T lower, T upper)
        {
            for (int i = 0; i < data.DimensionsCount; i++)
            {
                if (lower.GetKey(i).CompareTo(data.GetKey(i)) > 0 || upper.GetKey(i).CompareTo(data.GetKey(i)) < 0)
                    return false;
            }
            return true;
        }

        public bool LessThan(T left, T right)
        {
            for (int i = 0; i < left.DimensionsCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) >= 0)
                    return false;
            }
            return true;
        }

        public bool Equal(T left, T right)
        {
            for (int i = 0; i < left.DimensionsCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) != 0)
                    return false;
            }
            return true;
        }

        public bool GreaterThan(T left, T right)
        {
            for (int i = 0; i < left.DimensionsCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) <= 0)
                    return false;
            }
            return true;
        }
    }
}
