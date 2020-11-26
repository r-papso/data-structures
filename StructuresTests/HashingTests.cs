using Structures;
using Structures.Helper;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace StructuresTests
{
    public class HashingTests
    {
        private static int _clusterSize = 4096;
        private static string _dataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures", "Data", "extendible_hashing_data.bin");
        private static string _headerFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures", "Data", "extendible_hashing_header.bin");

        private readonly ITestOutputHelper _output;

        public HashingTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void InsertionTest(int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_clusterSize))
            {
                foreach (var item in data)
                {
                    hashing.Insert(item);
                }

                foreach (var item in data)
                {
                    var found = hashing.Find(item);
                    Assert.True(found.Count == 1 && found.First().Equals(item));
                }
            }

            RemoveFiles();
        }

        [Fact]
        public void RightShiftTest()
        {
            var rand = new Random(2);
            int randInt = rand.Next(int.MinValue, int.MaxValue);
            _output.WriteLine($"RandInt: {randInt}");

            var bitsUsed = 8;
            var bitArray = new BitArray(new int[] { randInt });
            var reversed = new BitArray(sizeof(int) * 8, false);
            _output.WriteLine($"BitArray: {BitArrayToString(bitArray)}");
            _output.WriteLine($"Reversed before: {BitArrayToString(reversed)}");

            int i = bitArray.Length - 1;
            int j = bitsUsed - 1;
            while (j >= 0)
            {
                reversed.Set(j--, bitArray.Get(i--));
            }

            _output.WriteLine($"Reversed after: {BitArrayToString(reversed)}");
            _output.WriteLine($"Reversed int: {reversed.ToInt()}");
        }

        private string BitArrayToString(BitArray bitArray)
        {
            string result = "";
            for (int i = bitArray.Length - 1; i >= 0; i--)
            {
                result += bitArray.Get(i) ? '1' : '0';
            }
            return result;
        }

        private void RemoveFiles()
        {
            File.Delete(_dataFile);
            File.Delete(_headerFile);
        }
    }
}
