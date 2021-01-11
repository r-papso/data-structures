using Structures.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StructuresTests
{
    public static class ITableTests
    {
        public static void InsertionTest(ITable<TwoDimObject> structure, int dataCount, Func<ICollection<TwoDimObject>, TwoDimObject, bool> foundEvaluator)
        {
            var data = Generator.GenerateRandomData(dataCount);
            int i = 0;


            foreach (var item in data)
            {
                structure.Insert(item);
                Assert.Equal(++i, structure.Count);
            }

            foreach (var item in data)
            {
                var found = structure.Find(item);
                Assert.True(foundEvaluator(found, item));
            }
        }

        public static void DeletionTest(ITable<TwoDimObject> structure, int dataCount)
        {
            var data = Generator.GenerateRandomData(dataCount);
            int i = 0;

            foreach (var item in data)
            {
                structure.Insert(item);
                Assert.Equal(++i, structure.Count);
            }

            foreach (var item in data)
            {
                structure.Delete(item);
                var found = structure.Find(item);
                Assert.Equal(0, found.Count);
                Assert.Equal(--i, structure.Count);
            }
        }

        public static void IterationTest(ITable<TwoDimObject> structure, int dataCount)
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
            Assert.Equal(count, structure.Count);
        }

        public static void RandomInsertDeletTest(ITable<TwoDimObject> structure, int dataCount, Func<ICollection<TwoDimObject>, TwoDimObject, bool> foundEvaluator)
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
                    Assert.True(foundEvaluator(found, item));
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
                        Assert.True(foundEvaluator(found, item));
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
    }
}
