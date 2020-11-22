using Structures;
using Structures.Hepler;
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
        #region Static fields

        public static string RESULTS_FOLDER = "C:\\FRI\\ING\\1_rocnik\\AUS2\\TestResults";

        private static bool _saveResults = false;
        private static bool _integerValues = true;
        private static int _minVal = 0;
        private static int _maxVal = 1000;
        private static KdComparer<TwoDimObject> _comparer = new KdComparer<TwoDimObject>();

        #endregion

        #region Construction

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void ConstructionTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);

            var timer = Stopwatch.StartNew();
            var tree = StructureFactory.Instance.GetKdTree(data);
            timer.Stop();

            /*int actualDepth = tree.GetDepth();
            int expectedDepth = GetExpectedDepth(nodeCount);
            if (actualDepth != expectedDepth)
                Assert.True(false, $"Actual depth of tree ({actualDepth}) was greater than expected ({expectedDepth})");*/

            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeConstructionTime.csv");
        }

        #endregion

        #region Iteration

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void IterationTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree(data);
            var iterations = 0;

            var timer = Stopwatch.StartNew();
            foreach (var node in tree) iterations++;
            timer.Stop();

            Assert.Equal(iterations, nodeCount);
            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeIterationTime.csv");
        }

        [Theory]
        [InlineData(15)]
        public void IterationTest(int nodeCount)
        {
            var data = GenerateRandomData(nodeCount);
            var csv = new StringBuilder();
            var tree = StructureFactory.Instance.GetKdTree(data);

            foreach (var node in tree)
            {
                csv.AppendLine($"{node.X};{node.Y}");
            }

            if (_saveResults)
                File.WriteAllText(Path.Join(RESULTS_FOLDER, "BSPTreeIteration.csv"), csv.ToString());
        }

        #endregion

        #region Search

        [Fact]
        public void IntervalSearchTest()
        {
            var data = GenerateDataGrid(50);
            var csv = new StringBuilder();
            var tree = StructureFactory.Instance.GetKdTree(data);
            var interval = tree.Find(new TwoDimObject(51, 0, 1), new TwoDimObject(52, 48, 1));

            foreach (var node in interval)
            {
                csv.AppendLine($"{node.X},{node.Y}");
            }

            if (_saveResults)
                File.WriteAllText(Path.Join(RESULTS_FOLDER, "BSPTreeIntervalSearch.csv"), csv.ToString());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void IntervalSearchTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree(data);

            var lowerIndex = (_maxVal - _minVal) / 2 - (_maxVal - _minVal) / 10;
            var upperIndex = (_maxVal - _minVal) / 2 + (_maxVal - _minVal) / 10;
            //var lowerIndex = _minVal;
            //var upperIndex = _minVal + (_maxVal - _minVal) / 10;

            var lower = new TwoDimObject(nodeCount + 1, lowerIndex, lowerIndex);
            var upper = new TwoDimObject(nodeCount + 2, upperIndex, upperIndex);

            var pointsInInterval = 0;

            foreach (var obj in data)
            {
                if (_comparer.Between(obj, lower, upper)) pointsInInterval++;
            }

            var timer = Stopwatch.StartNew();
            var interval = tree.Find(lower, upper);
            timer.Stop();

            //var differences = data.Where(p => p.Between(lower, upper))
            //    .Where(p1 => !interval.Any(p2 => p1.Equal(p2))).ToList();

            var count = interval.Count;
            Assert.True(count == pointsInInterval, $"Invalid number of found elements -> expected: {pointsInInterval}, found {count}");
            results.Add(new Result(nodeCount, timer.ElapsedMilliseconds));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeIntervalSearchTime.csv");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void PointSearchTimeTest(int nodeCount)
        {
            var results = new List<Result>();
            var times = new List<long>(nodeCount);
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree(data);
            int i = 0;

            foreach (var obj in data)
            {
                var timer = Stopwatch.StartNew();
                var found = tree.Find(obj);
                timer.Stop();
                times.Add(timer.ElapsedMilliseconds);

                if (found.Count == 0 || !found.All(x => _comparer.Equal(x, obj)))
                    Assert.True(false, $"Object with coordinates [{obj.X}, {obj.Y}] not found at {i}-th iteration");

                i++;
            }

            results.Add(new Result(nodeCount, times.Average()));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreePointSearchTime.csv");
        }

        [Fact]
        public void PointSearchTest()
        {
            var seed = 1;
            var data = GenerateRandomData(10, seed);
            var tree = StructureFactory.Instance.GetKdTree(data);
            var rand = new Random(seed);

            for (int i = 0; i < 100_000; i++)
            {
                var searchData = data[rand.Next(0, 10)];
                var result = tree.Find(searchData);

                Assert.True(result.Count > 0, $"Item at index {i} not found");

            }
        }

        #endregion

        #region Insertion

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void InsertionTest(int nodeCount)
        {
            var results = new List<Result>();
            var times = new List<long>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree<TwoDimObject>();

            foreach (var obj in data)
            {
                var timer = Stopwatch.StartNew();
                tree.Insert(obj);
                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);
            }

            int i = 0;
            foreach (var obj in data)
            {
                var found = tree.Find(obj);

                if (found.Count == 0 || !found.All(x => _comparer.Equal(x, obj)))
                    Assert.True(false, $"Object with coordinates [{obj.X}, {obj.Y}] not found at {i}-th iteration");

                i++;
            }

            i = 0;
            foreach (var node in tree) i++;

            Assert.Equal(nodeCount, i);

            results.Add(new Result(nodeCount, times.Average()));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeInsertionTime.csv");
        }

        #endregion

        #region Deletion

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void DeletionTest(int nodeCount)
        {
            var results = new List<Result>();
            var times = new List<long>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree(data);
            var expectedCount = nodeCount;

            foreach (var obj in data)
            {
                var timer = Stopwatch.StartNew();
                tree.Delete(obj);
                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);

                //int actualCount = 0;
                //foreach (var node in tree) actualCount++;
                var found = tree.Find(obj);

                Assert.DoesNotContain(found, x => x.Identical(obj));
                //Assert.Equal(--expectedCount, actualCount);
            }

            results.Add(new Result(nodeCount, times.Average()));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeDeletionTime.csv");
        }

        #endregion

        #region Updating

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(200_000)]
        [InlineData(400_000)]
        [InlineData(800_000)]
        [InlineData(1_600_000)]
        public void UpdatingTest(int nodeCount)
        {
            var results = new List<Result>();
            var times = new List<long>();
            var data = GenerateRandomData(nodeCount);
            var tree = StructureFactory.Instance.GetKdTree(data);
            var rand = new Random();
            var i = 0;

            foreach (var obj in data)
            {
                var newObj = _integerValues ? new TwoDimObject(nodeCount + i, rand.Next(_minVal, _maxVal), rand.Next(_minVal, _maxVal))
                                            : new TwoDimObject(nodeCount + i, (rand.NextDouble() + _minVal) * _maxVal, (rand.NextDouble() + _minVal) * _maxVal);

                var timer = Stopwatch.StartNew();
                tree.Update(obj, newObj);
                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);

                var found = tree.Find(newObj);

                if (found.Count == 0 || !found.All(x => _comparer.Equal(x, newObj)))
                    Assert.True(false, $"Object with coordinates [{obj.X}, {obj.Y}] not found at {i}-th iteration");

                i++;
            }

            results.Add(new Result(nodeCount, times.Average()));

            if (_saveResults)
                WriteResultsToCsv(results, "BSPTreeUpdatingTime.csv");
        }

        #endregion

        #region Custom tests

        [Fact]
        public void TestListArray()
        {
            IEnumerable<TwoDimObject> data = GenerateRandomData(10);
            TwoDimObject[] dataArray;
            if (data is TwoDimObject[])
            {
                dataArray = data as TwoDimObject[];
            }
            else
            {
                dataArray = data.ToArray();
            }

            Array.Sort(dataArray, (x, y) => x.GetKey(0).CompareTo(y.GetKey(0)));

            foreach (var obj in data)
            {
                Console.WriteLine($"[{obj.X}, {obj.Y}]");
            }

            foreach (var obj in dataArray)
            {
                Console.WriteLine($"[{obj.X}, {obj.Y}]");
            }
        }

        #endregion

        #region Private methods

        private TwoDimObject[] GenerateRandomData(int dataCount) => GenerateRandomData(dataCount, 0);

        private TwoDimObject[] GenerateRandomData(int dataCount, int seed)
        {
            Random rand;
            if (seed != 0)
                rand = new Random(seed);
            else
                rand = new Random();

            var data = new TwoDimObject[dataCount];

            for (int i = 0; i < dataCount; i++)
            {
                if (_integerValues)
                    data[i] = new TwoDimObject(i, rand.Next(_minVal, _maxVal), rand.Next(_minVal, _maxVal));
                else
                    data[i] = new TwoDimObject(i, (rand.NextDouble() + _minVal) * _maxVal, (rand.NextDouble() + _minVal) * _maxVal);
            }

            return data;
        }

        private TwoDimObject[] GenerateDataGrid(int gridSize)
        {
            var data = new TwoDimObject[Convert.ToInt32(Math.Pow(gridSize, 2))];

            int k = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    data[i * gridSize + j] = new TwoDimObject(k++, i, j);
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

            File.AppendAllText(Path.Join(RESULTS_FOLDER, fileName), csv.ToString());
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
