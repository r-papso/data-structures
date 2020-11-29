﻿using Structures.File;
using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Structures.Hashing
{
    internal class ExtendibleHashing<T> : IFileStructure<T> where T : ISerializable, new()
    {
        private static readonly int _maxDepth = 24;

        private int _clusterSize;
        private LinkedList<BlockMetaData>[] _directory;
        private BlockFile<T> _dataFile;
        private BlockFile<T> _overflowFile;

        private int Depth => Convert.ToInt32(Math.Log(_directory.Length, 2));

        private int BlockFactor => (_clusterSize - sizeof(int)) / new T().ByteSize;

        private int OverflowBlockFactor => (_clusterSize * 2 - sizeof(int)) / new T().ByteSize;

        public ExtendibleHashing(string folder)
        {
            _dataFile = new BlockFile<T>(Path.Combine(folder, StaticFields.DataFileName), Path.Combine(folder, StaticFields.DataHeaderName));
            _overflowFile = new BlockFile<T>(Path.Combine(folder, StaticFields.OverflowFileName), Path.Combine(folder, StaticFields.OverflowHeaderName));

            Restore(folder);
        }

        public ExtendibleHashing(string folder, int clusterSize)
        {
            _dataFile = new BlockFile<T>(Path.Combine(folder, StaticFields.DataFileName), Path.Combine(folder, StaticFields.DataHeaderName), clusterSize);
            _overflowFile = new BlockFile<T>(Path.Combine(folder, StaticFields.OverflowFileName), Path.Combine(folder, StaticFields.OverflowHeaderName), clusterSize * 2);

            Initialize(clusterSize);
        }

        ~ExtendibleHashing() => Release();

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();
            var index = GetIndex(data.GetHashCode(), Depth);

            foreach (var blockData in _directory[index])
            {
                Block<T> block = null;

                if (blockData == _directory[index].First.Value)
                    block = _dataFile.GetBlock(blockData.Address);
                else
                    block = _overflowFile.GetBlock(blockData.Address);

                if (block.Contains(data))
                {
                    result.AddLast(block.Get(data));
                    return result;
                }
            }

            return result;
        }

        public void Insert(T data)
        {
            BlockSplitResult<T> splitResult = null;

            while (true)
            {
                var index = GetIndex(data.GetHashCode(), Depth);
                var blockData = _directory[index].First.Value;
                Block<T> block = null;

                if (splitResult == null)
                    block = _dataFile.GetBlock(blockData.Address);
                else
                    block = splitResult.Block1Data == blockData ? splitResult.Block1 : splitResult.Block2;

                if (blockData.ValidDataCount == BlockFactor)
                {
                    if (blockData.Depth == Depth)
                    {
                        if (Depth == _maxDepth)
                        {
                            AddToOverflowFile(data, index);
                            break;
                        }

                        ExtendDirectory();
                        index = GetIndex(data.GetHashCode(), Depth);
                    }

                    splitResult = SplitBlock(blockData, block, index);
                }
                else
                {
                    if (splitResult != null && splitResult.Block1 != splitResult.Block2)
                    {
                        block.Add(data);
                        blockData.ValidDataCount = block.ValidDataCount;
                        _dataFile.UpdateBlock(splitResult.Block1, splitResult.Block1Data.Address);
                        splitResult.Block2Data.Address = _dataFile.AddBlock(splitResult.Block2);
                    }
                    else
                    {
                        if (blockData.IsValid)
                        {
                            block.Add(data);
                            _dataFile.UpdateBlock(block, blockData.Address);
                            blockData.ValidDataCount = block.ValidDataCount;
                        }
                        else
                        {
                            block = new Block<T>();
                            block.Add(data);
                            var newAddress = _dataFile.AddBlock(block);
                            blockData.Address = newAddress;
                            blockData.ValidDataCount = block.ValidDataCount;
                            blockData.IsValid = true;
                        }
                    }

                    break;
                }
            }
        }

        public void Update(T oldData, T newData)
        {
            if (oldData.Equals(newData))
            {
                var index = GetIndex(oldData.GetHashCode(), Depth);

                foreach (var blockData in _directory[index])
                {
                    Block<T> block = null;

                    if (blockData == _directory[index].First.Value)
                        block = _dataFile.GetBlock(blockData.Address);
                    else
                        block = _overflowFile.GetBlock(blockData.Address);

                    if (block.Contains(oldData))
                    {
                        block.Remove(oldData);
                        block.Add(newData);

                        if (blockData == _directory[index].First.Value)
                            _dataFile.UpdateBlock(block, blockData.Address);
                        else
                            _overflowFile.UpdateBlock(block, blockData.Address);

                        return;
                    }
                }

                throw new ArgumentException("Data not found");
            }
            else
            {
                Delete(oldData);
                Insert(newData);
            }
        }

        public void Delete(T data)
        {
            var index = GetIndex(data.GetHashCode(), Depth);

            if (_directory[index].Count == 1)
            {
                Block<T> block = null;
                var blockData = _directory[index].First.Value;

                while (true)
                {
                    if (block == null)
                    {
                        block = _dataFile.GetBlock(blockData.Address);

                        if (!block.Contains(data))
                            throw new ArgumentException("Data not found");

                        block.Remove(data);
                        blockData.ValidDataCount = block.ValidDataCount;
                    }

                    var neighbourIdx = GetNeighbourIndex(blockData, index);
                    var neighbour = _directory[neighbourIdx].First.Value;

                    if (Depth == 1)
                        break;

                    if (_directory[index] == _directory[neighbourIdx] || _directory[neighbourIdx].Count > 1)
                        break;

                    if (blockData.Depth != neighbour.Depth || blockData.ValidDataCount + neighbour.ValidDataCount > BlockFactor)
                        break;

                    MergeBlock(block, index, neighbourIdx);
                    index = GetIndex(data.GetHashCode(), Depth);
                }

                _dataFile.UpdateBlock(block, blockData.Address);
            }
            else
            {
                RemoveFromOverflowFile(data);
            }
        }

        public IEnumerator<T> GetEnumerator() => new ExtendibleHashingEnumerator(_dataFile, _overflowFile, _directory);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        private string ToCsv()
        {
            var csv = new StringBuilder();
            csv.AppendLine($"{_clusterSize};{_directory.Length}");
            int begin = 0;

            for (int i = 0; i < _directory.Length; i++)
            {
                if (i == _directory.Length - 1 || _directory[i + 1] != _directory[begin])
                {
                    string line = $"{begin};{i}";
                    foreach (var item in _directory[begin])
                    {
                        var sign = item.IsValid ? 1 : 0;
                        line += $";{sign};{item.Address};{item.ValidDataCount};{item.Depth}";
                    }
                    csv.AppendLine(line);
                    begin = i + 1;
                }
            }

            return csv.ToString();
        }

        private void FromCsv(string csv)
        {
            var head = csv.ReadLines().First().Split(';');
            _clusterSize = Convert.ToInt32(head[0]);
            _directory = new LinkedList<BlockMetaData>[Convert.ToInt32(head[1])];

            foreach (var line in csv.ReadLines().Skip(1))
            {
                var props = line.Split(';');
                int begin = Convert.ToInt32(props[0]);
                int end = Convert.ToInt32(props[1]);
                var list = new LinkedList<BlockMetaData>();

                for (int i = 2; i < props.Length; i += 4)
                {
                    list.AddLast(new BlockMetaData()
                    {
                        IsValid = props[i] == "1",
                        Address = Convert.ToInt64(props[i + 1]),
                        ValidDataCount = Convert.ToInt32(props[i + 2]),
                        Depth = Convert.ToInt32(props[i + 3])
                    });
                }

                for (int i = begin; i <= end; i++)
                {
                    _directory[i] = list;
                }
            }
        }

        private void Initialize(int clusterSize)
        {
            _clusterSize = clusterSize;
            _directory = new LinkedList<BlockMetaData>[]
            {
                new LinkedList<BlockMetaData>(),
                new LinkedList<BlockMetaData>()
            };

            _directory[0].AddLast(new BlockMetaData()
            {
                IsValid = true,
                Address = 0,
                ValidDataCount = 0,
                Depth = 1
            });
            _directory[1].AddLast(new BlockMetaData()
            {
                IsValid = true,
                Address = clusterSize,
                ValidDataCount = 0,
                Depth = 1
            });

            _dataFile.AddBlock(new Block<T>());
            _dataFile.AddBlock(new Block<T>());
        }

        private void Restore(string folder)
        {
            var path = Path.Combine(folder, StaticFields.DirectoryFileName);
            FromCsv(System.IO.File.ReadAllText(path));
        }

        private void Release()
        {
            var path = Path.Combine(Path.GetDirectoryName(_dataFile.FilePath), StaticFields.DirectoryFileName);
            System.IO.File.WriteAllText(path, ToCsv());

            _dataFile.Dispose();
            _overflowFile.Dispose();
        }

        private int GetIndex(int hashCode, int bitsUsed)
        {
            var bitArray = new BitArray(new int[] { hashCode });
            var index = new BitArray(sizeof(int) * 8, false);

            int i = _maxDepth - 1;
            int j = bitsUsed - 1;
            while (j >= 0)
            {
                index.Set(j--, bitArray.Get(i--));
            }

            return index.ToInt();
        }

        private int GetNeighbourIndex(BlockMetaData blockData, int blockIdx)
        {
            var bitArray = new BitArray(new int[] { blockIdx });
            var neighbourIdxArr = new BitArray(new int[] { blockIdx });
            var dividingBit = Depth - blockData.Depth;

            neighbourIdxArr.Set(dividingBit, !bitArray.Get(dividingBit));

            return neighbourIdxArr.ToInt();
        }

        private (int begin, int end) GetBlockBounds(int blockIdx)
        {
            int begin = blockIdx;
            int end = blockIdx;

            while (begin > 0 && _directory[begin] == _directory[begin - 1])
                begin--;

            while (end < _directory.Length - 1 && _directory[end] == _directory[end + 1])
                end++;

            return (begin, end);
        }

        private BlockSplitResult<T> SplitBlock(BlockMetaData blockData, Block<T> block, int blockIdx)
        {
            BlockSplitResult<T> result = new BlockSplitResult<T>()
            {
                Block1 = block,
                Block1Data = blockData,
                Block2 = block,
                Block2Data = blockData
            };

            if (blockData.Depth == Depth)
                throw new InvalidOperationException();

            var newBlock1Data = new List<T>();
            var newBlock2Data = new List<T>();

            foreach (var item in block)
            {
                var idx = GetIndex(item.GetHashCode(), blockData.Depth + 1);

                if (idx % 2 == 0)
                    newBlock1Data.Add(item);
                else
                    newBlock2Data.Add(item);
            }

            (int begin, int end) = GetBlockBounds(blockIdx);
            var block1Address = blockData.Address;
            var block2Address = blockData.Address;

            if (newBlock1Data.Count > 0 && newBlock2Data.Count > 0)
            {
                result.Block1 = new Block<T>();
                result.Block2 = new Block<T>();

                foreach (var item in newBlock1Data)
                    result.Block1.Add(item);

                foreach (var item in newBlock2Data)
                    result.Block2.Add(item);
            }

            blockData.Depth++;
            blockData.IsValid = newBlock1Data.Count > 0;
            blockData.ValidDataCount = newBlock1Data.Count;
            blockData.Address = block1Address;

            var newBlockMetaData = new BlockMetaData()
            {
                Address = block2Address,
                Depth = blockData.Depth,
                IsValid = newBlock2Data.Count > 0,
                ValidDataCount = newBlock2Data.Count
            };
            var newLinkedList = new LinkedList<BlockMetaData>();
            newLinkedList.AddLast(newBlockMetaData);
            result.Block2Data = newBlockMetaData;

            for (int i = (end - begin) / 2 + begin + 1; i <= end; i++)
            {
                _directory[i] = newLinkedList;
            }

            return result;
        }

        private void MergeBlock(Block<T> block, int blockIdx, int neighbourIdx)
        {
            var blockData = _directory[blockIdx].First.Value;
            var neighbourData = _directory[neighbourIdx].First.Value;

            if (neighbourData.IsValid)
            {
                var neighbourBlock = _dataFile.GetBlock(neighbourData.Address);
                foreach (var item in neighbourBlock)
                {
                    block.Add(item);
                }
                _dataFile.RemoveBlock(neighbourData.Address);
            }

            blockData.Depth--;
            blockData.ValidDataCount = block.ValidDataCount;
            (int begin, int end) = GetBlockBounds(neighbourIdx);

            for (int i = begin; i <= end; i++)
                _directory[i] = _directory[blockIdx];

            CompressDirectory();
        }

        private void AddToOverflowFile(T data, int blockIdx)
        {
            foreach (var item in _directory[blockIdx].Skip(1))
            {
                if (item.ValidDataCount < OverflowBlockFactor)
                {
                    var block = _overflowFile.GetBlock(item.Address);
                    block.Add(data);
                    _overflowFile.UpdateBlock(block, item.Address);
                    item.ValidDataCount = block.ValidDataCount;

                    return;
                }
            }

            var newBlock = new Block<T>();
            var newBlockData = new BlockMetaData();
            newBlock.Add(data);

            var newAddress = _overflowFile.AddBlock(newBlock);
            newBlockData.Address = newAddress;
            newBlockData.ValidDataCount = newBlock.ValidDataCount;
            _directory[blockIdx].AddLast(newBlockData);
        }

        private void RemoveFromOverflowFile(T data)
        {
            int index = GetIndex(data.GetHashCode(), Depth);

            if (_directory[index].Sum(x => x.ValidDataCount) - 1 <= BlockFactor + OverflowBlockFactor * (_directory[index].Count - 2))
            {
                var firstBlock = _dataFile.GetBlock(_directory[index].First.Value.Address);
                var overflowBlocks = new LinkedList<Block<T>>();

                foreach (var blockData in _directory[index].Skip(1))
                    overflowBlocks.AddLast(_overflowFile.GetBlock(blockData.Address));

                if (firstBlock.Contains(data))
                {
                    firstBlock.Remove(data);
                }
                else
                {
                    foreach (var block in overflowBlocks)
                    {
                        if (block.Contains(data))
                        {
                            block.Remove(data);
                            break;
                        }
                    }
                }

                while (firstBlock.ValidDataCount < BlockFactor)
                    firstBlock.Add(overflowBlocks.First.Value.RemoveAt(0));

                _directory[index].First.Value.ValidDataCount = firstBlock.ValidDataCount;

                var actualBlock = overflowBlocks.First;
                var actualBlockData = _directory[index].First.Next;

                while (true)
                {
                    if (actualBlock == overflowBlocks.Last)
                    {
                        actualBlockData.Value.ValidDataCount = actualBlock.Value.ValidDataCount;
                        break;
                    }

                    while (actualBlock.Value.ValidDataCount < OverflowBlockFactor)
                        actualBlock.Value.Add(actualBlock.Next.Value.RemoveAt(0));

                    actualBlockData.Value.ValidDataCount = actualBlock.Value.ValidDataCount;
                    actualBlock = actualBlock.Next;
                    actualBlockData = actualBlockData.Next;
                }

                _dataFile.UpdateBlock(firstBlock, _directory[index].First.Value.Address);

                actualBlock = overflowBlocks.First;
                actualBlockData = _directory[index].First.Next;

                while (true)
                {
                    if (actualBlock == null)
                        break;

                    _overflowFile.UpdateBlock(actualBlock.Value, actualBlockData.Value.Address);

                    actualBlock = actualBlock.Next;
                    actualBlockData = actualBlockData.Next;
                }

                _overflowFile.RemoveBlock(_directory[index].Last.Value.Address);
                _directory[index].RemoveLast();
            }
            else
            {
                foreach (var blockData in _directory[index])
                {
                    Block<T> block = null;

                    if (blockData == _directory[index].First.Value)
                        block = _dataFile.GetBlock(blockData.Address);
                    else
                        block = _overflowFile.GetBlock(blockData.Address);

                    if (block.Contains(data))
                    {
                        block.Remove(data);
                        blockData.ValidDataCount = block.ValidDataCount;

                        if (blockData == _directory[index].First.Value)
                            _dataFile.UpdateBlock(block, blockData.Address);
                        else
                            _overflowFile.UpdateBlock(block, blockData.Address);

                        break;
                    }
                }
            }
        }

        private void ExtendDirectory()
        {
            var newDir = new LinkedList<BlockMetaData>[_directory.Length * 2];
            for (int i = 0; i < newDir.Length; i++)
            {
                newDir[i] = _directory[i / 2];
            }
            _directory = newDir;
        }

        private void CompressDirectory()
        {
            if (CanCompressDirectory())
            {
                var newDir = new LinkedList<BlockMetaData>[_directory.Length / 2];
                for (int i = 0; i < _directory.Length; i += 2)
                {
                    newDir[i / 2] = _directory[i];
                }
                _directory = newDir;
            }
        }

        private bool CanCompressDirectory()
        {
            if (Depth == 1)
                return false;

            for (int i = 0; i < _directory.Length; i++)
            {
                if (_directory[i].First.Value.Depth == Depth)
                    return false;
            }

            return true;
        }

        private class ExtendibleHashingEnumerator : IEnumerator<T>
        {
            private int _dirIndex;
            private int _blockIndex;
            private Block<T> _currBlock;
            private LinkedListNode<BlockMetaData> _currBlockData;
            private readonly BlockFile<T> _dataFile;
            private readonly BlockFile<T> _overflowFile;
            private readonly LinkedList<BlockMetaData>[] _directory;

            public ExtendibleHashingEnumerator(BlockFile<T> dataFile, BlockFile<T> overflowFile, LinkedList<BlockMetaData>[] directory)
            {
                _dirIndex = -1;
                _dataFile = dataFile;
                _overflowFile = overflowFile;
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
                    else if (_currBlockData != null && _currBlockData.Next != null)
                    {
                        _currBlockData = _currBlockData.Next;
                        _currBlock = _overflowFile.GetBlock(_currBlockData.Value.Address);
                        _blockIndex = 0;
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

                                if (_directory[_dirIndex - 1] != _directory[_dirIndex] && _directory[_dirIndex].First.Value.IsValid)
                                    break;
                            }
                        }

                        if (oldIndex == _dirIndex)
                            return false;

                        _currBlockData = _directory[_dirIndex].First;
                        _currBlock = _dataFile.GetBlock(_currBlockData.Value.Address);
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
