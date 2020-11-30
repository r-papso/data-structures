using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures.File
{
    internal class Block<T> : IBlockState<T>, ISerializable where T : ISerializable, new()
    {
        private List<T> _dataList;

        public Block()
        {
            NextBlockAddress = -1;
            _dataList = new List<T>();
        }

        public int ByteSize => sizeof(int) + sizeof(long) + new T().ByteSize * ValidDataCount;

        public int ValidDataCount => _dataList.Count;

        public long Address { get; set; }

        public long NextBlockAddress { get; set; }

        public void FromByteArray(byte[] array, int offset = 0)
        {
            var dataCount = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

            NextBlockAddress = BitConverter.ToInt64(array, offset);
            offset += sizeof(long);

            var itemByteSize = new T().ByteSize;
            for (int i = 0; i < dataCount; i++)
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

            bArray = BitConverter.GetBytes(NextBlockAddress);
            result.ReplaceRange(bArray, offset);
            offset += bArray.Length;

            foreach (var item in _dataList)
            {
                bArray = item.ToByteArray();
                result.ReplaceRange(bArray, offset);
                offset += bArray.Length;
            }

            return result;
        }

        public bool Contains(T data) => _dataList.Any(x => x.Equals(data));

        public T Get(int index) => _dataList[index];

        public T Get(T data) => _dataList.Single(x => x.Equals(data));

        public void Add(T data) => _dataList.Add(data);

        public T Remove(T data)
        {
            var item = _dataList.Single(x => x.Equals(data));
            _dataList.Remove(item);
            return item;
        }

        public T RemoveAt(int index)
        {
            var removed = _dataList[index];
            _dataList.RemoveAt(index);
            return removed;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (ValidDataCount == 0)
                yield break;

            foreach (var item in _dataList)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
