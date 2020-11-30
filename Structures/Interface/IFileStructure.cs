using System;
using System.Collections.Generic;

namespace Structures.Interface
{
    public interface IFileStructure<T> : IStructure<T>, IDisposable
    {
        public IEnumerable<IBlockState<T>> PrimaryFileState { get; }

        public IEnumerable<IBlockState<T>> OverflowFileState { get; }
    }
}
