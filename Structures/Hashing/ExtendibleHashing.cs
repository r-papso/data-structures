using Structures.File;
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
    internal class ExtendibleHashing<T> : IHashFile<T> where T : ISerializable, new()
    {
        private static readonly int _maxDepth = 24;

        private int _clusterSize;
        private BlockMetaData[] _directory;
        private BlockFile<T> _dataFile;
        private BlockFile<T> _overflowFile;

        private int Depth => Convert.ToInt32(Math.Log(_directory.Length, 2));

        private int BlockFactor => (_clusterSize - sizeof(int) - sizeof(long)) / new T().ByteSize;

        private int OverflowBlockFactor => (_clusterSize * 2 - sizeof(int) - sizeof(long)) / new T().ByteSize;

        public IEnumerable<IBlockState<T>> PrimaryFileState
        {
            get
            {
                foreach (var block in _dataFile.FileState)
                    yield return block;
            }
        }

        public IEnumerable<IBlockState<T>> OverflowFileState
        {
            get
            {
                foreach (var block in _overflowFile.FileState)
                    yield return block;
            }
        }

        public IEnumerable<long> PrimaryFileFreeBlocks
        {
            get
            {
                foreach (var address in _dataFile.FreeAddresses)
                    yield return address;
            }
        }

        public IEnumerable<long> OverflowFileFreeBlocks
        {
            get
            {
                foreach (var address in _overflowFile.FreeAddresses)
                    yield return address;
            }
        }

        public ExtendibleHashing(string folder)
        {
            if (!CheckPaths(folder))
                throw new ArgumentException("Folder does not contain necessary files");

            _dataFile = new BlockFile<T>(Path.Combine(folder, StaticFields.DataFileName), Path.Combine(folder, StaticFields.DataHeaderName));
            _overflowFile = new BlockFile<T>(Path.Combine(folder, StaticFields.OverflowFileName), Path.Combine(folder, StaticFields.OverflowHeaderName));

            Restore(folder);
        }

        public ExtendibleHashing(string folder, int clusterSize)
        {
            _clusterSize = clusterSize;
            _dataFile = new BlockFile<T>(Path.Combine(folder, StaticFields.DataFileName), Path.Combine(folder, StaticFields.DataHeaderName), clusterSize);
            _overflowFile = new BlockFile<T>(Path.Combine(folder, StaticFields.OverflowFileName), Path.Combine(folder, StaticFields.OverflowHeaderName), clusterSize * 2);

            Initialize();
        }

        ~ExtendibleHashing() => Release();

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();
            var index = GetIndex(data.GetHashCode(), Depth);

            if (!_directory[index].IsValid || _directory[index].ValidDataCount == 0)
                return result;

            Block<T> block = _dataFile.GetBlock(_directory[index].Address);

            while (true)
            {
                if (block.Contains(data))
                {
                    result.AddLast(block.Get(data));
                    return result;
                }

                if (block.NextBlockAddress == -1)
                    break;

                if (block.NextBlockAddress == block.Address)
                    throw new InvalidOperationException();

                block = _overflowFile.GetBlock(block.NextBlockAddress);
            }

            return result;
        }

        public void Insert(T data)
        {
            BlockSplitResult<T> splitResult = null;

            while (true)
            {
                var index = GetIndex(data.GetHashCode(), Depth);
                var blockData = _directory[index];
                Block<T> block = null;

                if (splitResult == null)
                    block = _dataFile.GetBlock(blockData.Address);
                else
                    block = splitResult.Block1Data == blockData ? splitResult.Block1 : splitResult.Block2;

                if (block.Contains(data))
                    throw new ArgumentException("Cannot insert duplicate values");

                if (blockData.ValidDataCount == BlockFactor)
                {
                    if (blockData.Depth == Depth)
                    {
                        if (Depth == _maxDepth)
                        {
                            AddToOverflowFile(data, block);
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
            if (oldData.GetHashCode() == newData.GetHashCode())
            {
                var index = GetIndex(oldData.GetHashCode(), Depth);
                Block<T> block = _dataFile.GetBlock(_directory[index].Address);

                if (block.Contains(oldData))
                {
                    block.Remove(oldData);
                    block.Add(newData);
                    _dataFile.UpdateBlock(block, block.Address);
                }
                else
                {
                    while (true)
                    {
                        if (block.NextBlockAddress == -1)
                            throw new ArgumentException("Data not found");

                        block = _overflowFile.GetBlock(block.NextBlockAddress);

                        if (block.Contains(oldData))
                        {
                            block.Remove(oldData);
                            block.Add(newData);
                            _overflowFile.UpdateBlock(block, block.Address);

                            break;
                        }
                    }
                }
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

            if (_directory[index].ValidDataCounts.Count == 0)
            {
                Block<T> block = null;
                var blockData = _directory[index];

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
                    var neighbour = _directory[neighbourIdx];

                    if (Depth == 1)
                        break;

                    if (_directory[index] == _directory[neighbourIdx] || _directory[neighbourIdx].ValidDataCounts.Count > 0)
                        break;

                    if (blockData.Depth != neighbour.Depth || blockData.ValidDataCount + neighbour.ValidDataCount > BlockFactor)
                        break;

                    var blockResult = MergeBlock(block, index, neighbourIdx);
                    block = blockResult.Block;
                    blockData = blockResult.BlockData;
                    index = GetIndex(data.GetHashCode(), Depth);
                }

                _dataFile.UpdateBlock(block, blockData.Address);

                if (block.ValidDataCount == 0)
                {
                    _dataFile.RemoveBlock(block.Address);
                    blockData.IsValid = false;
                }

                if (Depth == 1 && _directory[0].ValidDataCount == 0 && _directory[1].ValidDataCount == 0)
                    Initialize();
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
                    var sign = _directory[begin].IsValid ? 1 : 0;
                    string line = $"{begin};{i};{sign};{_directory[begin].Address};{_directory[begin].ValidDataCount};{_directory[begin].Depth}";

                    foreach (var item in _directory[begin].ValidDataCounts)
                        line += $";{item}";

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
            _directory = new BlockMetaData[Convert.ToInt32(head[1])];

            foreach (var line in csv.ReadLines().Skip(1))
            {
                var props = line.Split(';');
                int begin = Convert.ToInt32(props[0]);
                int end = Convert.ToInt32(props[1]);
                var metaData = new BlockMetaData()
                {
                    IsValid = props[2] == "1",
                    Address = Convert.ToInt64(props[3]),
                    ValidDataCount = Convert.ToInt32(props[4]),
                    Depth = Convert.ToInt32(props[5])
                };

                for (int i = 6; i < props.Length; i++)
                    metaData.ValidDataCounts.Add(Convert.ToInt32(props[i]));

                for (int i = begin; i <= end; i++)
                    _directory[i] = metaData;
            }
        }

        private void Initialize()
        {
            _directory = new BlockMetaData[]
            {
                new BlockMetaData()
                {
                   IsValid = true,
                    Address = 0,
                    ValidDataCount = 0,
                    Depth = 1
                },
                new BlockMetaData()
                {
                    IsValid = true,
                    Address = _clusterSize,
                    ValidDataCount = 0,
                    Depth = 1
                }
            };

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
            if (_dataFile != null && _overflowFile != null)
            {
                var path = Path.Combine(Path.GetDirectoryName(_dataFile.FilePath), StaticFields.DirectoryFileName);
                System.IO.File.WriteAllText(path, ToCsv());

                _dataFile.Dispose();
                _overflowFile.Dispose();
            }
        }

        private bool CheckPaths(string folder)
        {
            return System.IO.File.Exists(Path.Combine(folder, StaticFields.DataFileName)) && System.IO.File.Exists(Path.Combine(folder, StaticFields.DataHeaderName)) &&
                   System.IO.File.Exists(Path.Combine(folder, StaticFields.OverflowFileName)) && System.IO.File.Exists(Path.Combine(folder, StaticFields.OverflowHeaderName)) &&
                   System.IO.File.Exists(Path.Combine(folder, StaticFields.DirectoryFileName));
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
            var blockData = _directory[blockIdx];
            var beginArray = new BitArray(new int[] { blockIdx });
            var endArray = new BitArray(new int[] { blockIdx });
            var dividingBit = Depth - blockData.Depth;

            for (int i = dividingBit - 1; i >= 0; i--)
            {
                beginArray.Set(i, false);
                endArray.Set(i, true);
            }

            return (beginArray.ToInt(), endArray.ToInt());
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
            result.Block2Data = newBlockMetaData;

            for (int i = (end - begin) / 2 + begin + 1; i <= end; i++)
                _directory[i] = newBlockMetaData;

            return result;
        }

        private BlockMergeResult<T> MergeBlock(Block<T> block, int blockIdx, int neighbourIdx)
        {
            var result = new BlockMergeResult<T>();
            var blockData = _directory[blockIdx];
            var neighbourData = _directory[neighbourIdx];

            if (neighbourData.IsValid)
            {
                var neighbourBlock = _dataFile.GetBlock(neighbourData.Address);

                if (neighbourBlock.Address < block.Address)
                {
                    var tempData = blockData;
                    var tempIdx = blockIdx;
                    var tempBlock = block;

                    blockData = neighbourData;
                    neighbourData = tempData;
                    blockIdx = neighbourIdx;
                    neighbourIdx = tempIdx;
                    block = neighbourBlock;
                    neighbourBlock = tempBlock;
                }

                foreach (var item in neighbourBlock)
                    block.Add(item);

                _dataFile.RemoveBlock(neighbourData.Address);
            }

            blockData.Depth--;
            blockData.ValidDataCount = block.ValidDataCount;
            (int begin, int end) = GetBlockBounds(neighbourIdx);

            for (int i = begin; i <= end; i++)
                _directory[i] = _directory[blockIdx];

            CompressDirectory();

            result.Block = block;
            result.BlockData = blockData;

            return result;
        }

        private void AddToOverflowFile(T data, Block<T> block)
        {
            var overflowBlock = block;
            Block<T> blockToAdd = null;
            int i = 0;
            int position = 0;

            if (block.NextBlockAddress != -1)
            {
                overflowBlock = _overflowFile.GetBlock(overflowBlock.NextBlockAddress);

                while (true)
                {
                    if (overflowBlock.Contains(data))
                        throw new ArgumentException("Cannot insert duplicate values");

                    if (blockToAdd == null && overflowBlock.ValidDataCount < OverflowBlockFactor)
                    {
                        position = i;
                        blockToAdd = overflowBlock;
                    }

                    if (overflowBlock.NextBlockAddress == -1)
                        break;

                    i++;
                    overflowBlock = _overflowFile.GetBlock(overflowBlock.NextBlockAddress);
                }
            }

            if (blockToAdd == null)
            {
                var index = GetIndex(data.GetHashCode(), Depth);
                _directory[index].ValidDataCounts.Add(1);

                var newBlock = new Block<T>();
                newBlock.Add(data);
                overflowBlock.NextBlockAddress = _overflowFile.AddBlock(newBlock);

                if (overflowBlock != block)
                    _overflowFile.UpdateBlock(overflowBlock, overflowBlock.Address);
                else
                    _dataFile.UpdateBlock(overflowBlock, overflowBlock.Address);
            }
            else
            {
                var index = GetIndex(data.GetHashCode(), Depth);
                _directory[index].ValidDataCounts[position]++;

                blockToAdd.Add(data);
                _overflowFile.UpdateBlock(blockToAdd, blockToAdd.Address);
            }
        }

        private void RemoveFromOverflowFile(T data)
        {
            int index = GetIndex(data.GetHashCode(), Depth);

            if (_directory[index].ValidDataCount + _directory[index].ValidDataCounts.Sum(x => x) - 1 <= BlockFactor + OverflowBlockFactor * (_directory[index].ValidDataCounts.Count - 1))
            {
                var blocks = new LinkedList<Block<T>>();
                var blocksHistory = new LinkedList<ITable<T>>();
                var actualBlock = _dataFile.GetBlock(_directory[index].Address);
                blocks.AddLast(actualBlock);
                blocksHistory.AddLast(StructureFactory.Instance.GetHashSet(actualBlock));

                while (actualBlock.NextBlockAddress != -1)
                {
                    var nextBlock = _overflowFile.GetBlock(actualBlock.NextBlockAddress);
                    blocks.AddLast(nextBlock);
                    blocksHistory.AddLast(StructureFactory.Instance.GetHashSet(nextBlock));
                    actualBlock = nextBlock;
                }

                bool found = false;

                foreach (var block in blocks)
                {
                    if (block.Contains(data))
                    {
                        block.Remove(data);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    throw new ArgumentException("Data not found");

                var actualBlockNode = blocks.First;
                var nextBlockNode = actualBlockNode.Next;

                while (nextBlockNode != null)
                {
                    var capacity = actualBlockNode == blocks.First ? BlockFactor : OverflowBlockFactor;

                    if (actualBlockNode.Value.ValidDataCount < capacity)
                    {
                        actualBlockNode.Value.Add(nextBlockNode.Value.RemoveAt(nextBlockNode.Value.ValidDataCount - 1));
                    }
                    else
                    {
                        actualBlockNode = actualBlockNode.Next;
                        nextBlockNode = actualBlockNode.Next;
                    }
                }

                _directory[index].ValidDataCount = blocks.First.Value.ValidDataCount;
                var idx = 0;

                foreach (var block in blocks.Skip(1))
                {
                    _directory[index].ValidDataCounts[idx++] = block.ValidDataCount;
                }

                actualBlockNode = blocks.First;
                var actualBlockHist = blocksHistory.First;

                while (actualBlockNode.Next != null)
                {
                    if (actualBlockNode.Next.Next == null)
                        actualBlockNode.Value.NextBlockAddress = -1;

                    if (actualBlockNode.Value.Any(x => actualBlockHist.Value.Find(x).Count == 0) || actualBlockNode.Value.NextBlockAddress == -1)
                    {
                        var blockFile = actualBlockNode == blocks.First ? _dataFile : _overflowFile;
                        blockFile.UpdateBlock(actualBlockNode.Value, actualBlockNode.Value.Address);
                    }

                    actualBlockNode = actualBlockNode.Next;
                    actualBlockHist = actualBlockHist.Next;
                }

                _overflowFile.RemoveBlock(blocks.Last.Value.Address);
                _directory[index].ValidDataCounts.RemoveAt(_directory[index].ValidDataCounts.Count - 1);
            }
            else
            {
                int i = -1;
                bool found = false;

                if (!_directory[index].IsValid || _directory[index].ValidDataCount == 0)
                    throw new ArgumentException("Data not found");

                Block<T> block = _dataFile.GetBlock(_directory[index].Address);

                while (true)
                {
                    if (block.Contains(data))
                    {
                        block.Remove(data);
                        found = true;

                        if (i == -1)
                        {
                            _dataFile.UpdateBlock(block, block.Address);
                            _directory[index].ValidDataCount--;
                        }
                        else
                        {
                            _overflowFile.UpdateBlock(block, block.Address);
                            _directory[index].ValidDataCounts[i]--;
                        }

                        break;
                    }

                    if (block.NextBlockAddress == -1)
                        break;

                    i++;
                    block = _overflowFile.GetBlock(block.NextBlockAddress);
                }

                if (!found)
                    throw new ArgumentException("Data not found");
            }
        }

        private void ExtendDirectory()
        {
            var newDir = new BlockMetaData[_directory.Length * 2];

            for (int i = 0; i < newDir.Length; i++)
                newDir[i] = _directory[i / 2];

            _directory = newDir;
        }

        private void CompressDirectory()
        {
            if (CanCompressDirectory())
            {
                var newDir = new BlockMetaData[_directory.Length / 2];

                for (int i = 0; i < _directory.Length; i += 2)
                    newDir[i / 2] = _directory[i];

                _directory = newDir;
            }
        }

        private bool CanCompressDirectory()
        {
            if (Depth == 1)
                return false;

            var idx = 0;

            while (idx < _directory.Length)
            {
                if (_directory[idx].Depth == Depth)
                    return false;

                (_, int end) = GetBlockBounds(idx);
                idx = end + 1;
            }

            return true;
        }

        private class ExtendibleHashingEnumerator : IEnumerator<T>
        {
            private int _dirIndex;
            private int _blockIndex;
            private Block<T> _currBlock;
            private readonly BlockFile<T> _dataFile;
            private readonly BlockFile<T> _overflowFile;
            private readonly BlockMetaData[] _directory;

            public ExtendibleHashingEnumerator(BlockFile<T> dataFile, BlockFile<T> overflowFile, BlockMetaData[] directory)
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
                    else if (_currBlock != null && _currBlock.NextBlockAddress != -1)
                    {
                        _currBlock = _overflowFile.GetBlock(_currBlock.NextBlockAddress);
                        _blockIndex = 0;
                    }
                    else
                    {
                        var oldData = _dirIndex == -1 ? null : _directory[_dirIndex];

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

                                if (_directory[_dirIndex - 1] != _directory[_dirIndex] && _directory[_dirIndex].IsValid)
                                    break;
                            }
                        }

                        if (oldData == _directory[_dirIndex])
                            return false;

                        _currBlock = _dataFile.GetBlock(_directory[_dirIndex].Address);
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
