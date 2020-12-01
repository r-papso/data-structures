using Structures;
using Structures.Interface;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace StructuresTests
{
    public class HashingTests
    {
        private static bool _skipTests = false;

        private static int _clusterSize = 4096;
        private static string _extendibleHashingPath = "C:\\FRI\\ING\\1_rocnik\\AUS2\\ExtendibleHashing";

        private static string _header = Path.Combine(_extendibleHashingPath, "directory.csv");
        private static string _data = Path.Combine(_extendibleHashingPath, "primary_file_data.bin");
        private static string _dataHeader = Path.Combine(_extendibleHashingPath, "primary_file_header.bin");
        private static string _overflow = Path.Combine(_extendibleHashingPath, "overflow_file_data.bin");
        private static string _overflowHeader = Path.Combine(_extendibleHashingPath, "overflow_file_header.bin");

        private readonly ITestOutputHelper _output;

        public HashingTests(ITestOutputHelper output) => _output = output;

        #region HashSet

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void HashSetInsertion(int dataCount)
        {
            var hashSet = StructureFactory.Instance.GetHashSet<TwoDimObject>();
            InsertionTest(hashSet, dataCount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void HashSetIteration(int dataCount)
        {
            var hashSet = StructureFactory.Instance.GetHashSet<TwoDimObject>();
            IterationTest(hashSet, dataCount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void HashSetDeletion(int dataCount)
        {
            var hashSet = StructureFactory.Instance.GetHashSet<TwoDimObject>();
            DeletionTest(hashSet, dataCount);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        public void HashSetRandomInsertDelete(int dataCount)
        {
            var hashSet = StructureFactory.Instance.GetHashSet<TwoDimObject>();
            RandomInsertDeletTest(hashSet, dataCount);
        }

        #endregion

        #region ExtendibleHashing

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void ExtendibleHashingDeletion(int dataCount)
        {
            if (!_skipTests)
            {
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
                {
                    DeletionTest(hashing, dataCount);
                }

                RemoveFiles();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void ExtendibleHashingInsertion(int dataCount)
        {
            if (!_skipTests)
            {
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
                {
                    InsertionTest(hashing, dataCount);
                }

                RemoveFiles();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void ExtendibleHashingIteration(int dataCount)
        {
            if (!_skipTests)
            {
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
                {
                    IterationTest(hashing, dataCount);
                }

                RemoveFiles();
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        public void ExtendibleHashingRandomInsertDelete(int dataCount)
        {
            if (!_skipTests)
            {
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize))
                {
                    RandomInsertDeletTest(hashing, dataCount);
                }

                RemoveFiles();
            }
        }

        [Fact]
        public void CloseOpenTest()
        {
            if (!_skipTests)
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
        }

        #endregion

        #region Private methods

        private void InsertionTest(IStructure<TwoDimObject> structure, int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);

            foreach (var item in data)
            {
                structure.Insert(item);
            }

            foreach (var item in data)
            {
                var found = structure.Find(item);
                Assert.True(found.Count == 1 && found.First().Equals(item));
            }
        }

        private void DeletionTest(IStructure<TwoDimObject> structure, int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);

            foreach (var item in data)
            {
                structure.Insert(item);
            }

            foreach (var item in data)
            {
                structure.Delete(item);
                var found = structure.Find(item);
                Assert.Equal(0, found.Count);
            }
        }

        private void IterationTest(IStructure<TwoDimObject> structure, int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);

            foreach (var item in data)
            {
                structure.Insert(item);
            }

            int count = 0;
            foreach (var item in structure)
                count++;

            Assert.Equal(dataCount, count);
        }

        private void RandomInsertDeletTest(IStructure<TwoDimObject> structure, int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);
            var inserted = data.Take(data.Count / 2).ToList();
            var toInsert = data.Skip(data.Count / 2).ToList();
            var randOperation = new Random();
            var randIndex = new Random();

            foreach (var item in inserted)
            {
                structure.Insert(item);
            }

            while (true)
            {
                if (inserted.Count == 0)
                {
                    var item = toInsert[randIndex.Next(0, toInsert.Count)];
                    structure.Insert(item);
                    toInsert.Remove(item);

                    var found = structure.Find(item);
                    Assert.True(found.Count == 1 && found.First().Equals(item));
                }
                else if (toInsert.Count == 0)
                {
                    var item = inserted[randIndex.Next(0, inserted.Count)];
                    structure.Delete(item);
                    inserted.Remove(item);

                    var found = structure.Find(item);
                    Assert.True(found.Count == 0);
                }
                else
                {
                    if (randOperation.NextDouble() < 0.5)
                    {
                        var item = toInsert[randIndex.Next(0, toInsert.Count)];
                        structure.Insert(item);
                        toInsert.Remove(item);

                        var found = structure.Find(item);
                        Assert.True(found.Count == 1 && found.First().Equals(item));
                    }
                    else
                    {
                        var item = inserted[randIndex.Next(0, inserted.Count)];
                        structure.Delete(item);
                        inserted.Remove(item);

                        var found = structure.Find(item);
                        Assert.True(found.Count == 0);
                    }
                }

                if (inserted.Count == 0 && toInsert.Count == 0)
                    break;
            }
        }

        private void RemoveFiles()
        {
            File.Delete(_header);
            File.Delete(_data);
            File.Delete(_dataHeader);
            File.Delete(_overflow);
            File.Delete(_overflowHeader);
        }

        #endregion
    }
}
