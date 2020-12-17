using Structures.Interface;
using System.IO;

namespace Structures.File
{
    internal class BlockStream
    {
        private FileStream _stream;

        public string StreamPath => _stream.Name;

        public BlockStream(string path, FileMode fileMode)
        {
            _stream = new FileStream(path, fileMode, FileAccess.ReadWrite, FileShare.None);
        }

        public Block<T> ReadBlock<T>(long address, int clusterSize) where T : ISerializable
        {
            var block = new Block<T>();
            var bytes = new byte[clusterSize];

            _stream.Seek(address, SeekOrigin.Begin);
            _stream.Read(bytes, 0, clusterSize);
            block.FromByteArray(bytes);
            block.Address = address;

            return block;
        }

        public void WriteBlock<T>(Block<T> block, long address) where T : ISerializable
        {
            _stream.Seek(address, SeekOrigin.Begin);
            var byteArr = block.ToByteArray();
            _stream.Write(byteArr, 0, byteArr.Length);
        }

        public void Release()
        {
            _stream.Flush();
            _stream.Close();
        }

        public void Trim(long address) => _stream.SetLength(address);
    }
}
