using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Structures.Hashing
{
    internal class ExtendibleHashing<T> : IStructure<T> where T : ISerializable, new()
    {
        private int _depth;
        private List<int> _directory;
        private FileStream _stream;

        public ExtendibleHashing()
        {
            _directory = new List<int>();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures", "Data", "data.bin");
            _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();
            var bitsUsed = Convert.ToInt32(Math.Pow(2, _depth));
            var bitArray = new BitArray(new int[] { data.GetHashCode() });
            var reversed = new BitArray(sizeof(int) * 8, false);

            for (int i = 0; i < bitsUsed; i++)
            {
                reversed.Set(i, bitArray.Get(bitArray.Length - 1 - i));
            }

            var dirIndex = BitArrayToInt(reversed);
            var block = new Block<T>();
            var bytes = new byte[block.ByteSize];

            _stream.Seek(_directory[dirIndex], SeekOrigin.Begin);
            _stream.Read(bytes, 0, block.ByteSize);
            block.FromByteArray(bytes);
            var resultData = block.GetData(data, out bool found);

            if (found)
                result.AddLast(resultData);

            return result;
        }

        public void Insert(T data)
        {
            throw new NotImplementedException();
        }

        public void Update(T oldData, T newData)
        {
            throw new NotImplementedException();
        }

        public void Delete(T data)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private int BitArrayToInt(BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException("BitArray length longer than 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }
    }
}
