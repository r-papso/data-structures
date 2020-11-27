﻿using Structures.File;
using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Structures.Hashing
{
    internal class ExtendibleHashing<T> : IFileStructure<T>, ISerializable where T : ISerializable, new()
    {
        private static readonly int _maxDepth = 24;

        private static readonly string _headerFileName = "extendible_hashing_header.bin";
        private static readonly string _dataFileName = "primary_file_data.bin";
        private static readonly string _dataHeaderName = "primary_file_header.bin";
        private static readonly string _overflowFileName = "overflow_file_data.bin";
        private static readonly string _overflowHeaderName = "overflow_file_header.bin";

        private int _clusterSize;
        private int[] _directory;
        private BlockFile<T> _dataFile;
        private BlockFile<T> _overflowFile;

        private int Depth => Convert.ToInt32(Math.Log(_directory.Length, 2));

        private int BlockFactor => (_clusterSize - 2 * sizeof(int)) / new T().ByteSize;

        public int ByteSize => 4 * sizeof(int) + _directory.Length * sizeof(int);

        public ExtendibleHashing(string folder)
        {
            Restore(folder);

            _dataFile = new BlockFile<T>(Path.Combine(folder, _dataFileName), Path.Combine(folder, _dataHeaderName));
            _overflowFile = new BlockFile<T>(Path.Combine(folder, _overflowFileName), Path.Combine(folder, _overflowHeaderName));
        }

        public ExtendibleHashing(string folder, int clusterSize)
        {
            _clusterSize = clusterSize;
            _directory = new int[] { 0, clusterSize };

            _dataFile = new BlockFile<T>(Path.Combine(folder, _dataFileName), Path.Combine(folder, _dataHeaderName), clusterSize);
            _overflowFile = new BlockFile<T>(Path.Combine(folder, _overflowFileName), Path.Combine(folder, _overflowHeaderName), clusterSize * 2);
        }

        ~ExtendibleHashing() => Release();

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();
            var index = GetIndex(data.GetHashCode(), Depth);
            var block = _dataFile.GetBlock(_directory[index]);
            var resultData = block.Get(data, out bool found);

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
                    block = _dataFile.GetBlock(_directory[index]);
                else
                    block = splittedBlock1.Address == _directory[index] ? splittedBlock1 : splittedBlock2;

                if (block.ValidDataCount == BlockFactor)
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
                    block.Add(data);
                    _dataFile.AddBlock(block);
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

        public IEnumerator<T> GetEnumerator() => new ExtendibleHashingEnumerator(_dataFile, _directory);

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

        private void Restore(string folder)
        {
            using var headerStream = new FileStream(Path.Combine(folder, _headerFileName), FileMode.Open);
            var totalLength = new byte[sizeof(int)];
            headerStream.Read(totalLength, 0, sizeof(int));

            var byteArr = new byte[BitConverter.ToInt32(totalLength, 0)];
            headerStream.Seek(0, SeekOrigin.Begin);
            headerStream.Read(byteArr, 0, byteArr.Length);
            FromByteArray(byteArr, sizeof(int));
        }

        private void Release()
        {
            var dir = Path.Combine(Path.GetDirectoryName(_dataFile.FilePath), _headerFileName);
            using var headerStream = new FileStream(dir, FileMode.OpenOrCreate);
            headerStream.Write(ToByteArray(), 0, ByteSize);
            _dataFile.Dispose();
            _overflowFile.Dispose();
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
            var newBlock1 = new Block<T>(block.BlockDepth + 1);
            var newBlock2 = new Block<T>(block.BlockDepth + 1);

            foreach (var data in block)
            {
                var idx = GetIndex(data.GetHashCode(), block.BlockDepth + 1);

                if (idx % 2 == 0)
                    newBlock1.Add(data);
                else if (idx % 2 == 1)
                    newBlock2.Add(data);
            }

            newBlock1.Address = _directory[blockIdx];
            _dataFile.UpdateBlock(newBlock1);
            var newAddress = _dataFile.AddBlock(newBlock2);

            (int begin, int end) = GetBlockBounds(blockIdx);

            for (int i = (end - begin) / 2 + begin + 1; i <= end; i++)
                _directory[i] = newAddress;

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
            private Block<T> _currBlock;
            private readonly BlockFile<T> _file;
            private readonly int[] _directory;

            public ExtendibleHashingEnumerator(BlockFile<T> file, int[] directory)
            {
                _dirIndex = -1;
                _file = file;
                _directory = directory;
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

                        _currBlock = _file.GetBlock(_directory[_dirIndex]);
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
