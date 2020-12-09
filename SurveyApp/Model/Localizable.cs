using Structures.Interface;

namespace SurveyApp.Model
{
    public abstract class Localizable : ISerializable
    {
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

        public abstract int ByteSize { get; }

        public Localizable() { }

        protected Localizable(int id, float x1, float y1, float x2, float y2)
        {
            ID = id;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override bool Equals(object obj)
        {
            var location = obj as Localizable;

            if (location == null)
                return false;

            return ID == location.ID;
        }

        public override int GetHashCode() => ID;

        public abstract byte[] ToByteArray();

        public abstract void FromByteArray(byte[] array, int offset = 0);
    }
}
