using Structures.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Structures.Hashing
{
    internal abstract class AbstractFile
    {
        private int _blockFactor;
        private BlockStream _stream;

        public AbstractFile(int clusterSize) => Create(clusterSize);

        private void Create(int clusterSize)
        {
            _stream = new BlockStream(StaticFields.ExtendibleHashingData);

            if (File.Exists(StaticFields.ExtendibleHashingHeader) && new FileInfo(StaticFields.ExtendibleHashingHeader).Length > 0)
                Restore();
            else
                Initialize(clusterSize);
        }
    }
}
