using Structures.File;
using Structures.Interface;

namespace Structures.Helper
{
    internal class BlockMergeResult<T> where T : ISerializable, new()
    {
        public BlockMetaData BlockData { get; set; }

        public Block<T> Block { get; set; }
    }
}
