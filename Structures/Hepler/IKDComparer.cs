using Structures.Tree;
using System.Collections.Generic;

namespace Structures.Hepler
{
    public class IKDComparer<T> : IComparer<T> where T : IKDComparable
    {
        public int Dimension { get; set; }

        public IKDComparer(int dimension) => Dimension = dimension;

        public int Compare(T x, T y)
        {
            return x.GetKey(Dimension).CompareTo(y.GetKey(Dimension));
        }
    }
}
