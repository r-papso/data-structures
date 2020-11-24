using System;

namespace StructuresTests
{
    public static class Generator
    {
        public static TwoDimObject[] GenerateRandomData(int dataCount, bool intValues = true, int min = 0, int max = 1000)
            => GenerateRandomData(dataCount, 0, intValues, min, max);

        public static TwoDimObject[] GenerateRandomData(int dataCount, int seed, bool intValues = true, int min = 0, int max = 1000)
        {
            Random rand;
            if (seed != 0)
                rand = new Random(seed);
            else
                rand = new Random();

            var data = new TwoDimObject[dataCount];

            for (int i = 0; i < dataCount; i++)
            {
                if (intValues)
                    data[i] = new TwoDimObject(i, rand.Next(min, max), rand.Next(min, max), $"Object {i}");
                else
                    data[i] = new TwoDimObject(i, (rand.NextDouble() + min) * max, (rand.NextDouble() + min) * max, $"Object {i}");
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
