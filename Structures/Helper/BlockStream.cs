using Structures.Hashing;
using Structures.Interface;
using System.IO;

namespace Structures.Helper
{
    internal static class BlockStream
    {
        public static Block<T> ReadBlock<T>(FileStream stream, int address, int blockFactor) where T : ISerializable, new()
        {
            var block = new Block<T>(blockFactor, 1);
            var bytes = new byte[block.ByteSize];

            stream.Seek(address, SeekOrigin.Begin);
            stream.Read(bytes, 0, block.ByteSize);
            block.FromByteArray(bytes);
            block.Address = address;

            return block;
        }

        public static void WriteBlock<T>(FileStream stream, Block<T> block) where T : ISerializable, new()
        {
            stream.Seek(block.Address, SeekOrigin.Begin);
            var byteArr = block.ToByteArray();
            stream.Write(byteArr, 0, byteArr.Length);
        }
    }
}
