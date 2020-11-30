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
        private static int _clusterSize = 256;
        private static string _extendibleHashingPath = "C:\\FRI\\ING\\1_rocnik\\AUS2\\ExtendibleHashing";

        private static string _header = Path.Combine(_extendibleHashingPath, "directory.csv");
        private static string _data = Path.Combine(_extendibleHashingPath, "primary_file_data.bin");
        private static string _dataHeader = Path.Combine(_extendibleHashingPath, "primary_file_header.bin");
        private static string _overflow = Path.Combine(_extendibleHashingPath, "overflow_file_data.bin");
        private static string _overflowHeader = Path.Combine(_extendibleHashingPath, "overflow_file_header.bin");

        private readonly ITestOutputHelper _output;

        public HashingTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void DeletionTest(int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount, 1);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
            {
                foreach (var item in data)
                {
                    hashing.Insert(item);
                }

                int i = 0;

                foreach (var item in data)
                {
                    hashing.Delete(item);
                    var found = hashing.Find(item);
                    Assert.Equal(0, found.Count);
                    i++;
                }
            }

            //RemoveFiles();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void InsertionTest(int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount, 1);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
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

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void IterationTest(int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount, 1);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
            {
                foreach (var item in data)
                {
                    hashing.Insert(item);
                }

                int count = 0;
                foreach (var item in hashing)
                    count++;

                Assert.Equal(dataCount, count);
            }

            RemoveFiles();
        }

        [Fact]
        public void RandomInsertDeleteTest()
        {
            var data = Generator.GenerateRandomData(1000, 1);
            var inserted = data.Take(data.Count / 2).ToList();
            var toInsert = data.Skip(data.Count / 2).ToList();
            var randOperation = new Random(1);
            var randIndex = new Random(1);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
            {
                foreach (var item in inserted)
                {
                    hashing.Insert(item);
                }

                while (true)
                {
                    if (inserted.Count == 0)
                    {
                        var item = toInsert[randIndex.Next(0, toInsert.Count)];
                        hashing.Insert(item);
                        toInsert.Remove(item);

                        var found = hashing.Find(item);
                        Assert.True(found.Count == 1 && found.First().Equals(item));
                    }
                    else if (toInsert.Count == 0)
                    {
                        var item = inserted[randIndex.Next(0, inserted.Count)];
                        hashing.Delete(item);
                        inserted.Remove(item);

                        var found = hashing.Find(item);
                        Assert.True(found.Count == 0);
                    }
                    else
                    {
                        if (randOperation.NextDouble() < 0.5)
                        {
                            var item = toInsert[randIndex.Next(0, toInsert.Count)];
                            hashing.Insert(item);
                            toInsert.Remove(item);

                            var found = hashing.Find(item);
                            Assert.True(found.Count == 1 && found.First().Equals(item));
                        }
                        else
                        {
                            var item = inserted[randIndex.Next(0, inserted.Count)];
                            hashing.Delete(item);
                            inserted.Remove(item);

                            var found = hashing.Find(item);
                            Assert.True(found.Count == 0);
                        }
                    }

                    if (inserted.Count == 0 && toInsert.Count == 0)
                        break;
                }
            }

            //RemoveFiles();
        }

        [Fact]
        public void CloseOpenTest()
        {
            var data = Generator.GenerateRandomData(10000, 1);

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
            {
                foreach (var item in data)
                {
                    hashing.Insert(item);
                }
            }

            using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath))
            {
                foreach (var item in data)
                {
                    var found = hashing.Find(item);
                    Assert.True(found.Count == 1 && found.First().Equals(item));
                }
            }
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
            File.Delete(_header);
            File.Delete(_data);
            File.Delete(_dataHeader);
            File.Delete(_overflow);
            File.Delete(_overflowHeader);
        }
    }
}
