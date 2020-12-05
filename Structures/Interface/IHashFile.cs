using System;
using System.Collections.Generic;

namespace Structures.Interface
{
    /// <summary>
    /// Defines operations available in hash structure stored at the disk
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHashFile<T> : ITable<T>, IDisposable
    {
        /// <summary>
        /// Provides traversal over primary file of the structure
        /// </summary>
        public IEnumerable<IBlockState<T>> PrimaryFileState { get; }

        /// <summary>
        /// Provides traversal over primary file free addresses
        /// </summary>
        public IEnumerable<long> PrimaryFileFreeBlocks { get; }

        /// <summary>
        /// Provides traversal over overflow file of the structure
        /// </summary>
        public IEnumerable<IBlockState<T>> OverflowFileState { get; }

        /// <summary>
        /// Provides traversal over overflow file free addresses
        /// </summary>
        public IEnumerable<long> OverflowFileFreeBlocks { get; }
    }
}
