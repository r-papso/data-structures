using Structures;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace StructuresTests
{
    public class HashingTests
    {
        private static bool _skipTests = false;

        private static int _clusterSize = 256;
        private static string _extendibleHashingPath = Directory.GetCurrentDirectory();

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
            ITableTests.InsertionTest(hashSet, dataCount, (found, wanted) => found.Count == 1 && found.First().Equals(wanted));
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
            ITableTests.IterationTest(hashSet, dataCount);
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
            ITableTests.DeletionTest(hashSet, dataCount);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        public void HashSetRandomInsertDelete(int dataCount)
        {
            var hashSet = StructureFactory.Instance.GetHashSet<TwoDimObject>();
            ITableTests.RandomInsertDeletTest(hashSet, dataCount, (found, wanted) => found.Count == 1 && found.First().Equals(wanted));
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
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize, new TwoDimObject()))
                {
                    ITableTests.DeletionTest(hashing, dataCount);
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
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize, new TwoDimObject()))
                {
                    ITableTests.InsertionTest(hashing, dataCount, (found, wanted) => found.Count == 1 && found.First().Equals(wanted));
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
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize, new TwoDimObject()))
                {
                    ITableTests.IterationTest(hashing, dataCount);
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
                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize, new TwoDimObject()))
                {
                    ITableTests.RandomInsertDeletTest(hashing, dataCount, (found, wanted) => found.Count == 1 && found.First().Equals(wanted));
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

                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, _clusterSize, new TwoDimObject()))
                {
                    foreach (var item in data)
                    {
                        hashing.Insert(item);
                    }
                }

                using (var hashing = StructureFactory.Instance.GetExtendibleHashing<TwoDimObject>(_extendibleHashingPath, new TwoDimObject()))
                {
                    foreach (var item in data)
                    {
                        var found = hashing.Find(item);
                        Assert.True(found.Count == 1 && found.First().Equals(item));
                    }
                }

                RemoveFiles();
            }
        }

        #endregion

        #region Private methods

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
