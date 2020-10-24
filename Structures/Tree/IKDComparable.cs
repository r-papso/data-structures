using System;

namespace Structures.Tree
{
    public interface IKDComparable
    {
        public int DimensionsCount { get; }

        public IComparable GetKey(int dimension);

        public bool Between(IKDComparable lower, IKDComparable upper);

        public bool LessThan(IKDComparable other);

        public bool Equal(IKDComparable other);

        public bool GreaterThan(IKDComparable other);

        public bool Identical(IKDComparable other);
    }
}
