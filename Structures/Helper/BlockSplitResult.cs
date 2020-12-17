using Structures.File;
using Structures.Interface;

namespace Structures.Helper
{
    internal class BlockSplitResult<T> where T : ISerializable
    {
        public BlockMetaData Block1Data { get; set; }

        public BlockMetaData Block2Data { get; set; }

        public Block<T> Block1 { get; set; }

        public Block<T> Block2 { get; set; }
    }
}
