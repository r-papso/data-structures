namespace Structures.Interface
{
    public interface ISerializable
    {
        public int ByteSize { get; }

        public byte[] ToByteArray();

        public void FromByteArray(byte[] array);
    }
}
