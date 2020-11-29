using System;
using System.Collections.Generic;

namespace StructuresTests
{
    public static class Generator
    {
        public static TwoDimObject[] GenerateRandomData(int dataCount, bool intValues = true, int min = 0, int max = 1000)
            => GenerateRandomData(dataCount, 0, intValues, min, max);

        public static TwoDimObject[] GenerateRandomData(int dataCount, int seed, bool intValues = true, int min = 0, int max = 1000)
        {
            Random randxy;
            Random randId;
            if (seed != 0)
            {
                randxy = new Random(seed);
                randId = new Random(seed);
            }
            else
            {
                randxy = new Random();
                randId = new Random();
            }

            var data = new TwoDimObject[dataCount];
            var usedIds = new HashSet<int>();

            for (int i = 0; i < dataCount; i++)
            {
                var id = randId.Next();

                while (usedIds.Contains(id))
                    id = randId.Next();

                usedIds.Add(id);

                if (intValues)
                    data[i] = new TwoDimObject(id, randxy.Next(min, max), randxy.Next(min, max), $"Object {id}");
                else
                    data[i] = new TwoDimObject(id, (randxy.NextDouble() + min) * max, (randxy.NextDouble() + min) * max, $"Object {id}");
            }

            return data;
        }

        public static TwoDimObject[] GenerateDataGrid(int gridSize)
        {
            var data = new TwoDimObject[Convert.ToInt32(Math.Pow(gridSize, 2))];

            int k = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    data[i * gridSize + j] = new TwoDimObject(k, i, j, $"Object {k}");
                    k++;
                }
            }

            return data;
        }
    }
}
