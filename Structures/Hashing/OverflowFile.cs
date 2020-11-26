using Structures.Helper;
using Structures.Interface;
using System;
using System.IO;

namespace Structures.Hashing
{
    internal class OverflowFile<T> : IDisposable, ISerializable where T : ISerializable, new()
    {
        private int _clusterSize;
        private int _maxAddress;
        private ISortedTree<int> _freeAddresses;
        private BlockStream _stream;

        private int BlockFactor => (_clusterSize - 2 * sizeof(int)) / new T().ByteSize;

        public int ByteSize => 2 * sizeof(int) + _freeAddresses.Count * sizeof(int);

        public OverflowFile(int clusterSize)
        {
            _stream = new BlockStream(StaticFields.OverflowFileData);

            if (File.Exists(StaticFields.OverflowFileHeader) && new FileInfo(StaticFields.OverflowFileHeader).Length > 0)
                Restore();
            else
                Initialize(clusterSize);
        }

        ~OverflowFile() => Release();

        public Block<T> GetBlock(int address) => _stream.ReadBlock<T>(address, _clusterSize);

        public int AddBlock(Block<T> block)
        {
            if (_freeAddresses.Count == 0)
            {
                _maxAddress += _clusterSize;
                block.Address = _maxAddress;
                _stream.WriteBlock(block);
            }
            else
            {
                block.Address = _freeAddresses.Min;
                _freeAddresses.Delete(block.Address);
                _stream.WriteBlock(block);
            }

            return block.Address;
        }

        public void RemoveBlock(int address)
        {
            if (address == _maxAddress)
                CascadeRemove();
            else
                _freeAddresses.Insert(address);
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
            _maxAddress = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _clusterSize = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            var length = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);
            _freeAddresses = StructureFactory.Instance.GetAvlTree<int>();

            for (int i = 0; i < length; i++)
            {
                _freeAddresses.Insert(BitConverter.ToInt32(array, offset));
                offset += sizeof(int);
            }
        }

        private void CascadeRemove()
        {
            var currAddress = _maxAddress;

            while (true)
            {
                var address = _freeAddresses.Max;

                if (currAddress - address != _clusterSize)
                    break;

                currAddress = address;
                _freeAddresses.Delete(address);
            }

            _stream.Trunc(currAddress);
            _maxAddress = currAddress - _clusterSize;
        }

        private void Restore()
        {

        }

        private void Initialize(int clusterSize)
        {

        }

        private void Release()
        {
            using var headerStream = new FileStream(StaticFields.OverflowFileHeader, FileMode.OpenOrCreate);
            headerStream.Write(ToByteArray(), 0, ByteSize);
            _stream.Release();
        }
    }
}
