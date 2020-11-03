using Structures;
using Structures.Hepler;
using SurveyApp.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace StructuresTests
{
    public class SurveyAppTests
    {
        private static bool _skip = true;
        private static bool _integerValues = false;
        private static int _minVal = 0;
        private static int _maxVal = 1000;

        #region Saving/Loading

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void SavingLoadingTest(int nodeCount)
        {
            if (!_skip)
            {
                var data = GenerateRandomData(nodeCount);
                var tree = StructureFactory.Instance.GetBSPTree(data);
                var filePath = Path.Combine(BSPTreeTests.RESULTS_FOLDER, $"SavingTest_{nodeCount}.csv");
                var adapter1 = new CollectionAdapter<TwoDimObject>(tree);
                var adapter2 = new CollectionAdapter<TwoDimObject>();

                adapter1.Save(filePath);
                adapter2.Load(filePath);

                Assert.True(adapter1.Tree.ComparePairWise(adapter2.Tree, (x, y) => x.Identical(y)));
            }
        }

        #endregion

        private List<TwoDimObject> GenerateRandomData(int dataCount) => GenerateRandomData(dataCount, 0);

        private List<TwoDimObject> GenerateRandomData(int dataCount, int seed)
        {
            Random rand;
            if (seed != 0)
                rand = new Random(seed);
            else
                rand = new Random();

            var data = new List<TwoDimObject>(dataCount);

            for (int i = 0; i < dataCount; i++)
            {
                if (_integerValues)
                    data.Add(new TwoDimObject(i, rand.Next(_minVal, _maxVal), rand.Next(_minVal, _maxVal)));
                else
                    data.Add(new TwoDimObject(i, (rand.NextDouble() + _minVal) * _maxVal, (rand.NextDouble() + _minVal) * _maxVal));
            }

            return data;
        }
    }
}
