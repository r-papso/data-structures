using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Structures.Hashing
{
    internal class ExtendibleHashing<T> : IFileStructure<T>, ISerializable where T : ISerializable, new()
    {
        private static int _maxDepth = 24;

        private int _blockFactor;
        private int _maxAddress;
        private int _clusterSize;
        private int[] _directory;
        private FileStream _stream;

        private int Depth => Convert.ToInt32(Math.Log(_directory.Length, 2));

        public int ByteSize => 5 * sizeof(int) + _directory.Length * sizeof(int);

        public ExtendibleHashing(int clusterSize)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StaticFields.ExtendibleHashingData));
            _stream = new FileStream(StaticFields.ExtendibleHashingData, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            if (File.Exists(StaticFields.ExtendibleHashingHeader) && new FileInfo(StaticFields.ExtendibleHashingHeader).Length > 0)
                Restore();
            else
                Initialize(clusterSize);
        }

        ~ExtendibleHashing() => Release();

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();
            var index = GetIndex(data.GetHashCode(), Depth);
            var block = BlockStream.ReadBlock<T>(_stream, _directory[index], _blockFactor);
            var resultData = block.GetData(data, out bool found);

            if (found)
                result.AddLast(resultData);

            return result;
        }

        public void Insert(T data)
        {
            Block<T> splittedBlock1 = null;
            Block<T> splittedBlock2 = null;

            while (true)
            {
                var index = GetIndex(data.GetHashCode(), Depth);
                Block<T> block = null;

                if (splittedBlock1 == null || splittedBlock2 == null)
                    block = BlockStream.ReadBlock<T>(_stream, _directory[index], _blockFactor);
                else
                    block = splittedBlock1.Address == _directory[index] ? splittedBlock1 : splittedBlock2;

                if (block.ValidDataCount == _blockFactor)
                {
                    if (block.BlockDepth == Depth)
                    {
                        if (Depth == _maxDepth)
                        {
                            //Overflow
                            throw new NotImplementedException();
                        }
                        ExtendDirectory();
                        index = GetIndex(data.GetHashCode(), Depth);
                    }
                    (splittedBlock1, splittedBlock2) = SplitBlock(block, index);
                }
                else
                {
                    block.InsertData(data);
                    BlockStream.WriteBlock(_stream, block);
                    break;
                }
            }
        }

        public void Update(T oldData, T newData)
        {
            throw new NotImplementedException();
        }

        public void Delete(T data)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() => new ExtendibleHashingEnumerator(_blockFactor, _stream, _directory);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        public byte[] ToByteArray()
        {
            var result = new byte[ByteSize];
            var offset = 0;

            var bArray = BitConverter.GetBytes(ByteSize);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_blockFactor);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_maxAddress);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_clusterSize);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_directory.Length);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            for (int i = 0; i < _directory.Length; i++)
            {
                bArray = BitConverter.GetBytes(_directory[i]);
                result.ReplaceRange(bArray, offset);
                offset += bArray.Length;
            }

            return result;
        }

        public void FromByteArray(byte[] array, int offset = 0)
        {
            _blockFactor = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _maxAddress = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _clusterSize = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            var length = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _directory = new int[length];

            for (int i = 0; i < length; i++)
            {
                _directory[i] = BitConverter.ToInt32(array, offset);
                offset += sizeof(int);
            }
        }

        private void Initialize(int clusterSize)
        {
            _maxAddress = clusterSize;
            _clusterSize = clusterSize;
            _blockFactor = (clusterSize - 2 * sizeof(int)) / (new T().ByteSize);
            _directory = new int[] { 0, clusterSize };

            var block = new Block<T>(_blockFactor, 1);
            block.Address = _directory[0];
            BlockStream.WriteBlock(_stream, block);
            block.Address = _directory[1];
            BlockStream.WriteBlock(_stream, block);
        }

        private void Restore()
        {
            using var headerStream = new FileStream(StaticFields.ExtendibleHashingHeader, FileMode.Open);
            var totalLength = new byte[sizeof(int)];
            headerStream.Read(totalLength, 0, sizeof(int));

            var byteArr = new byte[BitConverter.ToInt32(totalLength, 0)];
            headerStream.Seek(0, SeekOrigin.Begin);
            headerStream.Read(byteArr, 0, byteArr.Length);
            FromByteArray(byteArr, sizeof(int));
        }

        private void Release()
        {
            using var headerStream = new FileStream(StaticFields.ExtendibleHashingHeader, FileMode.OpenOrCreate);
            headerStream.Write(ToByteArray(), 0, ByteSize);
            _stream.Flush();
            _stream.Close();
        }

        private int GetIndex(int hashCode, int bitsUsed)
        {
            var bitArray = new BitArray(new int[] { hashCode });
            var reversed = new BitArray(_maxDepth, false);

            int i = _maxDepth - 1;
            int j = bitsUsed - 1;
            while (j >= 0)
            {
                reversed.Set(j--, bitArray.Get(i--));
            }

            return reversed.ToInt();
        }

        private (int begin, int end) GetBlockBounds(int blockIdx)
        {
            int begin = blockIdx;
            int end = blockIdx;
            int address = _directory[blockIdx];

            while (begin > 0 && _directory[begin - 1] == address)
                begin--;

            while (end < _directory.Length - 1 && _directory[end + 1] == address)
                end++;

            return (begin, end);
        }

        private (Block<T> block1, Block<T> block2) SplitBlock(Block<T> block, int blockIdx)
        {
            var newBlock1 = new Block<T>(_blockFactor, block.BlockDepth + 1);
            var newBlock2 = new Block<T>(_blockFactor, block.BlockDepth + 1);

            var idxs = block.Select(x => GetIndex(x.GetHashCode(), block.BlockDepth + 1));

            foreach (var data in block)
            {
                var idx = GetIndex(data.GetHashCode(), block.BlockDepth + 1);

                if (idx % 2 == 0)
                    newBlock1.InsertData(data);
                else if (idx % 2 == 1)
                    newBlock2.InsertData(data);
            }

            _maxAddress += _clusterSize;
            newBlock1.Address = _directory[blockIdx];
            newBlock2.Address = _maxAddress;
            BlockStream.WriteBlock(_stream, newBlock1);
            BlockStream.WriteBlock(_stream, newBlock2);

            (int begin, int end) = GetBlockBounds(blockIdx);

            for (int i = (end - begin) / 2 + begin + 1; i <= end; i++)
                _directory[i] = _maxAddress;

            return (newBlock1, newBlock2);
        }

        private void ExtendDirectory()
        {
            var newDir = new int[_directory.Length * 2];
            for (int i = 0; i < newDir.Length; i++)
            {
                newDir[i] = _directory[i / 2];
            }
            _directory = newDir;
        }

        private class ExtendibleHashingEnumerator : IEnumerator<T>
        {
            private int _dirIndex;
            private int _blockIndex;
            private int _blockFactor;
            private Block<T> _currBlock;
            private readonly FileStream _stream;
            private readonly int[] _directory;

            public ExtendibleHashingEnumerator(int blockFactor, FileStream stream, int[] directory)
            {
                _dirIndex = -1;
                _stream = stream;
                _directory = directory;
                _blockFactor = blockFactor;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_currBlock != null && _blockIndex < _currBlock.ValidDataCount)
                    {
                        Current = _currBlock.Get(_blockIndex++);
                        return true;
                    }
                    else
                    {
                        var oldIndex = _dirIndex;

                        if (_dirIndex < 0)
                        {
                            _dirIndex++;
                        }
                        else
                        {
                            while (true)
                            {
                                if (_dirIndex < _directory.Length - 1)
                                    _dirIndex++;
                                else
                                    break;

                                if (_directory[_dirIndex - 1] != _directory[_dirIndex])
                                    break;
                            }
                        }

                        if (oldIndex == _dirIndex)
                            return false;

                        _currBlock = BlockStream.ReadBlock<T>(_stream, _directory[_dirIndex], _blockFactor);
                        _blockIndex = 0;
                    }
                }
            }

            public void Reset()
            {
                _currBlock = null;
                _dirIndex = -1;
            }
        }
    }
}
