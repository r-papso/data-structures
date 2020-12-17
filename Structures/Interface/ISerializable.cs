namespace Structures.Interface
{
    /// <summary>
    /// Defines operations used in serialization and deserialization of object
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Size of object in bytes
        /// </summary>
        public int ByteSize { get; }

        /// <summary>
        /// Serializes object to byte array
        /// </summary>
        /// <returns>Byte array representation of object</returns>
        public byte[] ToByteArray();

        /// <summary>
        /// Deserializes object from byte array
        /// </summary>
        /// <param name="array">Array representation of object</param>
        /// <param name="offset">Start index in array where deserialization will begin</param>
        public void FromByteArray(byte[] array, int offset = 0);

        /// <summary>
        /// Creates new instance with same type as instance implementing this interface
        /// </summary>
        /// <returns>New instance with same type</returns>
        public ISerializable Clone();
    }
}
