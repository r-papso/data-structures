using Structures;
using Structures.Helper;
using SurveyApp.Helper;
using System.IO;
using Xunit;

namespace StructuresTests
{
    public class SurveyAppTests
    {
        private static bool _skip = true;
        private static bool _integerValues = true;
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
                var data = Generator.GenerateRandomData(nodeCount, _integerValues, _minVal, _maxVal);
                var tree = StructureFactory.Instance.GetKdTree(data);
                var filePath = Path.Combine(BSPTreeTests.RESULTS_FOLDER, $"SavingTest_{nodeCount}.csv");
                var adapter1 = new CollectionAdapter<TwoDimObject>(tree);
                var adapter2 = new CollectionAdapter<TwoDimObject>();

                adapter1.Save(filePath);
                adapter2.Load(filePath);

                Assert.True(adapter1.Tree.ComparePairWise(adapter2.Tree, (x, y) => x.Identical(y)));
            }
        }

        #endregion
    }
}
