using Structures.Exception;
using Structures.Tree;
using System;

namespace StructuresTests
{
    public class TwoDimObject : IKDComparable
    {
        private static int DIM_COUNT = 2;

        public int PrimaryKey { get; }

        public double X { get; }

        public double Y { get; }

        public int DimensionsCount => DIM_COUNT;

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
                    throw new DimensionRangeException(dimension, DIM_COUNT);
            }
        }

        public bool Between(IKDComparable lower, IKDComparable upper)
        {
            var lowerObj = lower as TwoDimObject;
            var upperObj = upper as TwoDimObject;

            return lowerObj.X <= X && X <= upperObj.X && lowerObj.Y <= Y && Y <= upperObj.Y;
        }

        public bool GreaterThan(IKDComparable other)
        {
            var obj = other as TwoDimObject;
            return X > obj.X && Y > obj.Y;
        }

        public bool LessThan(IKDComparable other)
        {
            var obj = other as TwoDimObject;
            return X < obj.X && Y < obj.Y;
        }

        public bool Equal(IKDComparable other)
        {
            var obj = other as TwoDimObject;
            return X == obj.X && Y == obj.Y;
        }

        public bool Identical(IKDComparable other)
        {
            var obj = other as TwoDimObject;
            return X == obj.X && Y == obj.Y && PrimaryKey == obj.PrimaryKey;
        }
    }
}
