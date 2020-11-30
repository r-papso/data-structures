using Structures.Helper;
using Structures.Interface;
using System;
using System.IO;

namespace Structures.File
{
    internal class BlockFile<T> : IDisposable, ISerializable where T : ISerializable, new()
    {
        private int _clusterSize;
        private long _maxAddress;
        private string _headerFilePath;
        private ITree<long> _freeAddresses;
        private BlockStream _stream;

        public int ByteSize => 3 * sizeof(int) + sizeof(long) + _freeAddresses.Count * sizeof(long);

        public string FilePath => _stream.StreamPath;

        public BlockFile(string dataFilePath, string headerFilePath)
        {
            _headerFilePath = headerFilePath;
            _stream = new BlockStream(dataFilePath);
            Restore(headerFilePath);
        }

        public BlockFile(string dataFilePath, string headerFilePath, int clusterSize)
        {
            _maxAddress = -1;
            _clusterSize = clusterSize;
            _headerFilePath = headerFilePath;
            _freeAddresses = StructureFactory.Instance.GetAvlTree<long>();
            _stream = new BlockStream(dataFilePath);
        }

        ~BlockFile() => Release();

        public Block<T> GetBlock(long address) => _stream.ReadBlock<T>(address, _clusterSize);

        public long AddBlock(Block<T> block)
        {
            if (_maxAddress == -1)
            {
                _maxAddress = 0;
                _stream.WriteBlock(block, _maxAddress);
                return _maxAddress;
            }
            else if (_freeAddresses.Count == 0)
            {
                _maxAddress += _clusterSize;
                _stream.WriteBlock(block, _maxAddress);
                return _maxAddress;
            }
            else
            {
                var minAddress = _freeAddresses.Min;
                _freeAddresses.Delete(minAddress);
                _stream.WriteBlock(block, minAddress);
                return minAddress;
            }
        }

        public void UpdateBlock(Block<T> block, long address) => _stream.WriteBlock(block, address);

        public void RemoveBlock(long address)
        {
            if (address == _maxAddress)
            {
                TrimFile();
                if (_maxAddress == 0)
                    _maxAddress = -1;
            }
            else
            {
                _freeAddresses.Insert(address);
            }
        }

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

            bArray = BitConverter.GetBytes(_maxAddress);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_clusterSize);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(_freeAddresses.Count);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            foreach (var address in _freeAddresses.LevelOrderTraversal)
            {
                bArray = BitConverter.GetBytes(address);
                result.ReplaceRange(bArray, offset);
                offset += bArray.Length;
            }

            return result;
        }

        public void FromByteArray(byte[] array, int offset = 0)
        {
            _maxAddress = BitConverter.ToInt64(array, offset);
            offset += sizeof(long);
            _clusterSize = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            var length = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _freeAddresses = StructureFactory.Instance.GetAvlTree<long>();

            for (int i = 0; i < length; i++)
            {
                _freeAddresses.Insert(BitConverter.ToInt32(array, offset));
                offset += sizeof(long);
            }
        }

        private void TrimFile()
        {
            var currAddress = _maxAddress;

            while (true)
            {
                if (_freeAddresses.Count == 0)
                    break;

                var address = _freeAddresses.Max;

                if (currAddress - address != _clusterSize)
                    break;

                currAddress = address;
                _freeAddresses.Delete(address);
            }

            _stream.Trim(currAddress);
            _maxAddress = currAddress - _clusterSize;
        }

        private void Restore(string headerFile)
        {
            using var headerStream = new FileStream(headerFile, FileMode.Open);
            var totalLength = new byte[sizeof(int)];
            headerStream.Read(totalLength, 0, sizeof(int));

            var byteArr = new byte[BitConverter.ToInt32(totalLength, 0)];
            headerStream.Seek(0, SeekOrigin.Begin);
            headerStream.Read(byteArr, 0, byteArr.Length);
            FromByteArray(byteArr, sizeof(int));
        }

        private void Release()
        {
            var path = Path.Combine(Path.GetDirectoryName(FilePath), _headerFilePath);
            using var headerStream = new FileStream(path, FileMode.OpenOrCreate);
            headerStream.Write(ToByteArray(), 0, ByteSize);
            _stream.Release();
        }
    }
}
