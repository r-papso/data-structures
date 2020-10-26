using Structures.Exception;
using Structures.Tree;
using System;

namespace StructuresTests
{
    public class TwoDimObject : IKdComparable
    {
        private static int _dimCount = 2;

        public int PrimaryKey { get; }

        public double X { get; }

        public double Y { get; }

        public int DimensionsCount => _dimCount;

        public TwoDimObject(int primaryKey, double x, double y)
        {
            PrimaryKey = primaryKey;
            X = x;
            Y = y;
        }

        public IComparable GetKey(int dimension)
        {
            switch (dimension)
            {
                case 0:
                    return X;
                case 1:
                    return Y;
                default:
                    throw new DimensionRangeException(dimension, _dimCount);
            }
        }

        public bool Identical(IKdComparable other)
        {
            var obj = other as TwoDimObject;

            if (obj == null)
                return false;

            return X == obj.X && Y == obj.Y && PrimaryKey == obj.PrimaryKey;
        }
    }
}
