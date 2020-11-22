using Structures.Interface;
using System;
using System.Collections.Generic;

namespace Structures.Hashing
{
    internal class Block<T> : ISerializable where T : ISerializable, new()
    {
        private int _validDataCount;
        private List<T> _dataList;

        public Block()
        {
            _validDataCount = 0;
            _dataList = new List<T>();
        }

        public int ByteSize => throw new NotImplementedException();

        public void FromByteArray(byte[] array)
        {
            throw new NotImplementedException();
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public T GetData(T data, out bool found)
        {
            for (int i = 0; i < _validDataCount; i++)
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
    }
}
