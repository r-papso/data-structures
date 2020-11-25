using Structures.Helper;
using Structures.Interface;

namespace Structures.Hashing
{
    internal class OverflowFile<T> where T : ISerializable, new()
    {
        private int _clusterSize;
        private BlockStream _stream;

        private int BlockFactor => (_clusterSize - 2 * sizeof(int)) / new T().ByteSize * 2;

        public OverflowFile(int clusterSize)
        {
            _clusterSize = clusterSize;
            _stream = new BlockStream(StaticFields.OverflowFile);
        }

        public Block<T> GetBlock(int address) => _stream.ReadBlock<T>(address);

        public void AddBlock(Block<T> block)
        {

        }
    }
}
