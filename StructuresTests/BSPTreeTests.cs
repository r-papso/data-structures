using Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace StructuresTests
{
    public class BSPTreeTests
    {
        public static string ResultsFolder = "C:\\FRI\\ING\\1_rocnik\\AUS2\\TestResults";

        #region Construction

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        [InlineData(800000)]
        [InlineData(1600000)]
        public void ConstructionTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);

            var timer = Stopwatch.StartNew();
            var tree = StructureFactory.Instance.GetBSPTree(data);
            timer.Stop();

            int actualDepth = tree.GetDepth();
            int expectedDepth = GetExpectedDepth(nodeCount);
            if (actualDepth != expectedDepth)
                Assert.True(false, $"Actual depth of tree ({actualDepth}) was greater than expected ({expectedDepth})");

            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            WriteResultsToCsv(results, "BSPTreeConstructionTime.csv");
        }

        #endregion

        #region Iteration

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        [InlineData(800000)]
        [InlineData(1600000)]
        public void IterationTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetBSPTree(data);
            var iterations = 0;

            var timer = Stopwatch.StartNew();
            foreach (var node in tree) iterations++;
            timer.Stop();

            Assert.Equal(iterations, nodeCount);
            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            WriteResultsToCsv(results, "BSPTreeIterationTime.csv");
        }

        [Theory]
        [InlineData(15)]
        public void IterationTest(int nodeCount)
        {
            var data = GenerateRandomData(nodeCount);
            var csv = new StringBuilder();
            var tree = StructureFactory.Instance.GetBSPTree(data);

            foreach (var node in tree)
            {
                csv.AppendLine($"{node.X};{node.Y}");
            }

            File.WriteAllText(Path.Join(ResultsFolder, "BSPTreeIteration.csv"), csv.ToString());
        }

        #endregion

        #region Search

        [Fact]
        public void IntervalSearchTest()
        {
            var data = GenerateDataGrid(50);
            var csv = new StringBuilder();
            var tree = StructureFactory.Instance.GetBSPTree(data);
            var interval = tree.Find(new TwoDimObject(51, 0, 1), new TwoDimObject(52, 48, 1));

            foreach (var node in interval)
            {
                csv.AppendLine($"{node.X},{node.Y}");
            }

            File.WriteAllText(Path.Join(ResultsFolder, "BSPTreeIntervalSearch.csv"), csv.ToString());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        [InlineData(800000)]
        [InlineData(1600000)]
        public void IntervalSearchTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetBSPTree(data);
            var lower = new TwoDimObject(nodeCount + 1, 400, 600);
            var upper = new TwoDimObject(nodeCount + 2, 400, 600);
            var pointsInInterval = 0;

            foreach (var obj in data)
            {
                if (obj.Between(lower, upper)) pointsInInterval++;
            }

            var timer = Stopwatch.StartNew();
            var interval = tree.Find(lower, upper);
            timer.Stop();

            //var differences = data.Where(p => p.Between(lower, upper))
            //    .Where(p1 => !interval.Any(p2 => p1.Equal(p2))).ToList();

            var count = interval.Count;
            Assert.True(count == pointsInInterval, $"Invalid number of found elements -> expected: {pointsInInterval}, found {count}");
            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            WriteResultsToCsv(results, "BSPTreeIntervalSearchTime.csv");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        [InlineData(800000)]
        [InlineData(1600000)]
        public void PointSearchTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var times = new List<long>(nodeCount);
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetBSPTree(data);
            int i = 0;

            foreach (var obj in data)
            {
                var timer = Stopwatch.StartNew();
                var found = tree.Find(obj);
                timer.Stop();
                times.Add(timer.ElapsedMilliseconds);

                if (found.Count == 0 || !found.All(x => x.Equal(obj)))
                    Assert.True(false, $"Object with coordinates [{obj.X}, {obj.Y}] not found at {i}-th iteration");

                i++;
            }

            results.Add(new Result(nodeCount, times.Average()));
            WriteResultsToCsv(results, "BSPTreePointSearchTime.csv");
        }

        [Fact]
        public void PointSearchTest()
        {
            var seed = 1;
            var data = GenerateRandomData(10, seed);
            var tree = StructureFactory.Instance.GetBSPTree(data);
            var rand = new Random(seed);

            for (int i = 0; i < 100_000; i++)
            {
                var searchData = data[rand.Next(0, 10)];
                try
                {
                    tree.Find(searchData);
                }
                catch (ArgumentException)
                {
                    Assert.True(false, $"Item at index {i} not found");
                }
            }
        }

        #endregion

        #region Insertion

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        public void InsertionTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount, 1);
            var tree = StructureFactory.Instance.GetBSPTree<TwoDimObject>();

            foreach (var obj in data)
            {
                tree.Insert(obj);
            }

            int i = 0;
            foreach (var obj in data)
            {
                var found = tree.Find(obj);

                if (found.Count == 0 || !found.All(x => x.Equal(obj)))
                    Assert.True(false, $"Object with coordinates [{obj.X}, {obj.Y}] not found at {i}-th iteration");

                i++;
            }

            i = 0;
            foreach (var node in tree) i++;

            Assert.Equal(nodeCount, i);
        }

        #endregion

        #region Private methods

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
                data.Add(new TwoDimObject(i, rand.Next(0, 1000), rand.Next(0, 1000)));
                //data.Add(new TwoDimObject(i, rand.NextDouble() * 1000, rand.NextDouble() * 1000));
            }

            return data;
        }

        private List<TwoDimObject> GenerateDataGrid(int gridSize)
        {
            var data = new List<TwoDimObject>(Convert.ToInt32(Math.Pow(gridSize, 2)));

            int k = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    data.Add(new TwoDimObject(k++, i, j));
                }
            }

            return data;
        }

        private void WriteResultsToCsv(IEnumerable<Result> results, string fileName)
        {
            var csv = new StringBuilder();

            foreach (var result in results)
            {
                csv.AppendLine($"{result.N};{result.Time}");
            }

            File.AppendAllText(Path.Join(ResultsFolder, fileName), csv.ToString());
        }

        private int GetExpectedDepth(int nodeCount)
        {
            return Convert.ToInt32(Math.Ceiling(Math.Log(nodeCount + 1) / Math.Log(2)));
        }

        #endregion

        private class Result
        {
            public int N { get; }

            public double Time { get; }

            public Result(int n, double time)
            {
                N = n;
                Time = time;
            }
        }
    }
}
