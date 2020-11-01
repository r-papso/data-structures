using System;

namespace Structures.Tree
{
    public interface IKdComparable
    {
        public int DimensionsCount { get; }

        public IComparable GetKey(int dimension);

        public bool Identical(IKdComparable other);
    }
}
