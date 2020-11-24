using Structures.Exception;
using Structures.Helper;
using Structures.Interface;
using SurveyApp.Interface;
using System;

namespace StructuresTests
{
    public class TwoDimObject : IKdComparable, ISaveable, ISerializable
    {
        private static int _dimCount = 2;
        private static int _maxDescLength = 20;

        private string _description;

        public int PrimaryKey { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public string Description
        {
            get => _description;
            private set
            {
                if (value.Length > _maxDescLength)
                    _description = value.Remove(_maxDescLength);
                else
                    _description = value;
            }
        }

        public int DimensionCount => _dimCount;

        public int ByteSize => sizeof(int) + 2 * sizeof(double) + sizeof(ushort) + _maxDescLength * sizeof(char);

        public TwoDimObject() { }

        public TwoDimObject(int primaryKey, double x, double y)
            : this(primaryKey, x, y, string.Empty)
        { }

        public TwoDimObject(int primaryKey, double x, double y, string description)
        {
            PrimaryKey = primaryKey;
            X = x;
            Y = y;
            Description = description;
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

        public byte[] ToByteArray()
        {
            var result = new byte[ByteSize];
            int offset = 0;

            var bArray = BitConverter.GetBytes(PrimaryKey);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(X);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(Y);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(Convert.ToUInt16(Description.Length));
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = Description.GetBytes();
            result.ReplaceRange(bArray, offset);

            return result;
        }

        public void FromByteArray(byte[] array, int offset = 0)
        {
            PrimaryKey = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            X = BitConverter.ToDouble(array, offset);
            offset += sizeof(double);
            Y = BitConverter.ToDouble(array, offset);
            offset += sizeof(double);

            var descLength = BitConverter.ToUInt16(array, offset);
            offset += sizeof(ushort);
            string desc = string.Empty;

            for (int i = 0; i < descLength; i++)
            {
                desc += BitConverter.ToChar(array, offset);
                offset += sizeof(char);
            }

            Description = desc;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TwoDimObject;

            if (other == null)
                return false;

            return PrimaryKey == other.PrimaryKey;
        }

        public override int GetHashCode() => PrimaryKey;
    }
}
