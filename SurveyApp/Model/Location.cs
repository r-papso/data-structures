using Structures.Helper;
using Structures.Interface;
using System;

namespace SurveyApp.Model
{
    /// <summary>
    /// Represents instance of a location
    /// </summary>
    public class Location : ISerializable
    {
        private static readonly int _maxDescLength = 20;

        private string _description;

        /// <summary>
        /// Unique ID of location
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Location's first longitude coordinate
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Location's first latitude coordinate
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Location's second longitude coordinate
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Location's second latitude coordinate
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// Number of location
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Location's description
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value.Length > _maxDescLength)
                    _description = value.Remove(_maxDescLength);
                else
                    _description = value;
            }
        }

        public int ByteSize => 2 * sizeof(int) + 4 * sizeof(float) + sizeof(ushort) + _maxDescLength * sizeof(char);

        /// <summary>
        /// Default constructor
        /// </summary>
        public Location() { }

        /// <summary>
        /// Constructor specifying all properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="description"></param>
        public Location(int id, int number, float x1, float y1, float x2, float y2, string description)
        {
            ID = id;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Number = number;
            Description = description;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">location to be copied from</param>
        public Location(Location other)
        {
            if (other != null)
            {
                ID = other.ID;
                Number = other.Number;
                X1 = other.X1;
                Y1 = other.Y1;
                X2 = other.X2;
                Y2 = other.Y2;
                Description = other.Description;
            }
        }

        public byte[] ToByteArray()
        {
            var result = new byte[ByteSize];
            int offset = 0;

            var bArray = BitConverter.GetBytes(ID);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(Number);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(X1);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(Y1);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(X2);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(Y2);
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
            ID = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            Number = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            X1 = BitConverter.ToSingle(array, offset);
            offset += sizeof(float);

            Y1 = BitConverter.ToSingle(array, offset);
            offset += sizeof(float);

            X2 = BitConverter.ToSingle(array, offset);
            offset += sizeof(float);

            Y2 = BitConverter.ToSingle(array, offset);
            offset += sizeof(float);

            var descLength = BitConverter.ToUInt16(array, offset);
            offset += sizeof(ushort);

            Description = array.ToString(offset, descLength);
        }

        public ISerializable Clone()
        {
            return new Location();
        }

        public override bool Equals(object obj)
        {
            var location = obj as Location;

            if (location == null)
                return false;

            return ID == location.ID;
        }

        public override int GetHashCode() => ID;
    }
}
