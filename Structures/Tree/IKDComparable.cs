using System;

namespace Structures.Tree
{
    public interface IKDComparable
    {
        public int DimensionsCount { get; }

        public IComparable GetKey(int dimension);

        public bool Identical(IKDComparable other);
    }
}
