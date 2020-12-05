using System.Collections.Generic;

namespace Structures.Interface
{
    /// <summary>
    /// Represent state of the block
    /// </summary>
    /// <typeparam name="T">Type of objects stored at block</typeparam>
    public interface IBlockState<T> : IEnumerable<T>
    {
        /// <summary>
        /// Count of valid data stored within block
        /// </summary>
        public int ValidDataCount { get; }

        /// <summary>
        /// Address of block in binary file
        /// </summary>
        public long Address { get; }

        /// <summary>
        /// Address of next block
        /// </summary>
        public long NextBlockAddress { get; }
    }
}
