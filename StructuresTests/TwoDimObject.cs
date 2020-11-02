using Structures.Exception;
using Structures.Interface;
using System;

namespace StructuresTests
{
    public class TwoDimObject : IKdComparable, ISaveable
    {
        private static int _dimCount = 2;

        public int PrimaryKey { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public int DimensionsCount => _dimCount;

        public TwoDimObject() { }

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

        public string ToCsv(string delimiter = ",")
        {
            return $"{PrimaryKey}{delimiter}{X}{delimiter}{Y}";
        }

        public void FromCsv(string csv, string delimiter = ",")
        {
            var props = csv.Split(new string[] { delimiter }, StringSplitOptions.None);
            PrimaryKey = Int32.Parse(props[0]);
            X = Double.Parse(props[1]);
            Y = Double.Parse(props[2]);
        }
    }
}
