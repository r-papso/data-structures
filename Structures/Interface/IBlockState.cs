using System.Collections.Generic;

namespace Structures.Interface
{
    public interface IBlockState<T> : IEnumerable<T>
    {
        public long Address { get; }

        public long NextBlockAddress { get; }
    }
}
