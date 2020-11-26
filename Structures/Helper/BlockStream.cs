using Structures.Hashing;
using Structures.Interface;
using System.IO;

namespace Structures.Helper
{
    internal class BlockStream
    {
        private FileStream _stream;

        public BlockStream(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public Block<T> ReadBlock<T>(int address, int clusterSize) where T : ISerializable, new()
        {
            var block = new Block<T>(1);
            var bytes = new byte[clusterSize];

            _stream.Seek(address, SeekOrigin.Begin);
            _stream.Read(bytes, 0, clusterSize);
            block.FromByteArray(bytes);
            block.Address = address;

            return block;
        }

        public void WriteBlock<T>(Block<T> block) where T : ISerializable, new()
        {
            _stream.Seek(block.Address, SeekOrigin.Begin);
            var byteArr = block.ToByteArray();
            _stream.Write(byteArr, 0, byteArr.Length);
        }

        public void Release()
        {
            _stream.Flush();
            _stream.Close();
        }

        public void Trunc(int address) => _stream.SetLength(address);
    }
}
