using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Structures.Hashing
{
    internal class Block<T> : ISerializable, IEnumerable<T> where T : ISerializable, new()
    {
        private int _blockFactor;
        private List<T> _dataList;

        public Block(int blockFactor, int blockDepth)
        {
            ValidDataCount = 0;
            BlockDepth = blockDepth;
            _blockFactor = blockFactor;
            _dataList = new List<T>();
        }

        public int ByteSize => 2 * sizeof(int) + new T().ByteSize * _blockFactor;

        public int ValidDataCount { get; private set; }

        public int BlockDepth { get; set; }

        public int Address { get; set; }

        public void FromByteArray(byte[] array, int offset = 0)
        {
            ValidDataCount = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            BlockDepth = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            var itemByteSize = new T().ByteSize;
            for (int i = 0; i < ValidDataCount; i++)
            {
                var item = new T();
                item.FromByteArray(array, offset + i * itemByteSize);
                _dataList.Add(item);
            }
        }

        public byte[] ToByteArray()
        {
            var result = new byte[ByteSize];
            int offset = 0;

            var bArray = BitConverter.GetBytes(ValidDataCount);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            bArray = BitConverter.GetBytes(BlockDepth);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            for (int i = 0; i < ValidDataCount; i++)
            {
                bArray = _dataList[i].ToByteArray();
                result.ReplaceRange(bArray, offset);
                offset += bArray.Length;
            }

            return result;
        }

        public T Get(int index) => _dataList[index];

        public T GetData(T data, out bool found)
        {
            for (int i = 0; i < ValidDataCount; i++)
            {
                if (_dataList[i].Equals(data))
                {
                    found = true;
                    return _dataList[i];
                }
            }

            found = false;
            return default;
        }

        public void InsertData(T data)
        {
            _dataList.Add(data);
            ValidDataCount++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (ValidDataCount == 0)
                yield break;

            for (int i = 0; i < ValidDataCount; i++)
                yield return _dataList[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
