using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Structures.Tree
{
    internal class KdTreeNode<T> : TreeNode<T> where T : IKdComparable
    {
        public int Level { get; set; }

        public KdTreeNode(T data, int level) : base(data) => Level = level;

        public KdTreeNode(IEnumerable<T> data) : base() => TreeFromData(data);

        private void TreeFromData(IEnumerable<T> data)
        {
            int level = 0;
            T[] dataArray;

            if (data is T[])
                dataArray = data as T[];
            else
                dataArray = data.ToArray();

            if (dataArray == null || dataArray.Length < 1)
                throw new ArgumentNullException($"{nameof(data)} cannot be null or empty");

            int dimension = level % dataArray[0].DimensionCount;
            var stack = new Stack<ConstructionNode>();
            var comparer = new KdComparer<T>(dimension);

            Array.Sort(dataArray, 0, dataArray.Length, comparer);

            int median = Convert.ToInt32((dataArray.Length - 1) / 2);
            while (median < dataArray.Length - 1 && dataArray[median].GetKey(dimension).CompareTo(dataArray[median + 1].GetKey(dimension)) == 0)
                median++;

            Data = dataArray[median];
            Level = level;

            if (median < dataArray.Length - 1)
                stack.Push(new ConstructionNode(level + 1, median + 1, dataArray.Length - 1, this, false));
            if (median > 0)
                stack.Push(new ConstructionNode(level + 1, 0, median - 1, this, true));

            while (stack.Count > 0)
            {
                var currNode = stack.Pop();

                if (currNode.Min != currNode.Max)
                {
                    dimension = currNode.Level % dataArray[0].DimensionCount;
                    comparer.Dimension = dimension;

                    Array.Sort(dataArray, currNode.Min, currNode.Max - currNode.Min + 1, comparer);
                    median = Convert.ToInt32((currNode.Min + currNode.Max) / 2);
                    while (median < currNode.Max && dataArray[median].GetKey(dimension).CompareTo(dataArray[median + 1].GetKey(dimension)) == 0)
                        median++;
                }
                else
                {
                    median = currNode.Min;
                }

                var newNode = new KdTreeNode<T>(dataArray[median], currNode.Level);
                newNode.Parent = currNode.Parent;

                if (currNode.IsLeft)
                    newNode.Parent.Left = newNode;
                else
                    newNode.Parent.Right = newNode;

                if (median < currNode.Max)
                    stack.Push(new ConstructionNode(currNode.Level + 1, median + 1, currNode.Max, newNode, false));
                if (median > currNode.Min)
                    stack.Push(new ConstructionNode(currNode.Level + 1, currNode.Min, median - 1, newNode, true));
            }
        }

        private class ConstructionNode
        {
            public int Level { get; }

            public int Min { get; }

            public int Max { get; }

            public KdTreeNode<T> Parent { get; }

            public bool IsLeft { get; }

            public ConstructionNode() { }

            public ConstructionNode(int level, KdTreeNode<T> parent, bool isLeft)
            {
                Level = level;
                Parent = parent;
                IsLeft = isLeft;
            }

            public ConstructionNode(int level, int min, int max, KdTreeNode<T> parent, bool isLeft)
            {
                Level = level;
                Min = min;
                Max = max;
                Parent = parent;
                IsLeft = isLeft;
            }
        }
    }
}
