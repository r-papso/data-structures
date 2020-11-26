using Structures;
using Structures.Helper;
using System;
using System.Linq;
using Xunit;

namespace StructuresTests
{
    public class AvlTreeTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void InsertDeleteTest(int nodeCount)
        {
            var data = Generator.GenerateRandomData(nodeCount).Shuffle();
            var tree = StructureFactory.Instance.GetAvlTree<TwoDimObject>();

            foreach (var item in data)
            {
                tree.Insert(item);
            }

            foreach (var item in data)
            {
                var found = tree.Find(item);
                Assert.True(found.Count == 1 && item.CompareTo(found.First()) == 0, "Data not inserted properly");
            }

            Assert.Equal(tree.Min.PrimaryKey, data.Min(x => x.PrimaryKey));
            Assert.Equal(tree.Max.PrimaryKey, data.Max(x => x.PrimaryKey));

            foreach (var item in data)
            {
                if (item.PrimaryKey == 19)
                {
                    Console.WriteLine("");
                }
                tree.Delete(item);
                var found = tree.Find(item);
                Assert.True(found.Count == 0, "Data not deleted properly");
            }
        }

        [Fact]
        public void InsertDeleteTest_Fact()
        {
            for (int i = 3494; i < 3495; i++)
            {
                if (i == 9)
                {
                    Console.WriteLine("");
                }
                var rand = new Random(i);
                var data = Generator.GenerateRandomData(12).Shuffle(rand);
                var tree = StructureFactory.Instance.GetAvlTree<TwoDimObject>();

                foreach (var item in data)
                {
                    tree.Insert(item);
                }

                foreach (var item in data)
                {
                    var found = tree.Find(item);
                    Assert.True(found.Count == 1 && item.CompareTo(found.First()) == 0, $"Data not inserted properly, i: {i}");
                }

                foreach (var item in data)
                {
                    //try
                    //{
                    tree.Delete(item);
                    var found = tree.Find(item);
                    Assert.True(found.Count == 0, $"Data not deleted properly, i : {i}");
                    //}
                    //catch (Exception ex)
                    //{
                    //    Assert.True(false, $"exception: {ex}, i: {i}");
                    //}
                }
            }
        }
    }
}
