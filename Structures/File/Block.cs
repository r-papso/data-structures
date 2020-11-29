using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures.File
{
    internal class Block<T> : ISerializable, IEnumerable<T> where T : ISerializable, new()
    {
        private List<T> _dataList;

        public Block() => _dataList = new List<T>();

        public int ByteSize => sizeof(int) + new T().ByteSize * ValidDataCount;

        public int ValidDataCount => _dataList.Count;

        public void FromByteArray(byte[] array, int offset = 0)
        {
            var dataCount = BitConverter.ToInt32(array, offset);
            offset += sizeof(int);

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

            foreach (var item in _dataList)
            {
                bArray = item.ToByteArray();
                result.ReplaceRange(bArray, offset);
                offset += bArray.Length;
            }

            return result;
        }

        public T Get(int index) => _dataList[index];

        public void Add(T data) => _dataList.Add(data);

        public void Remove(T data)
        {
            var item = _dataList.Single(x => x.Equals(data));
            _dataList.Remove(item);
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
