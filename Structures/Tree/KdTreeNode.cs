using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures.Tree
{
    internal class KdTreeNode<T> : IEnumerable<KdTreeNode<T>> where T : IKdComparable
    {
        public KdTreeNode(T data, int level) => (Data, Level) = (data, level);

        public KdTreeNode(IEnumerable<T> data) => TreeFromData(data);

        public bool IsLeaf => Right == null && Left == null;

        public int Level { get; set; }

        public KdTreeNode<T> Left { get; set; }

        public KdTreeNode<T> Right { get; set; }

        public KdTreeNode<T> Parent { get; set; }

        public T Data { get; set; }

        public void Delete()
        {
            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = null;
                else
                    Parent.Right = null;
            }

            Parent = null;
            Left = null;
            Right = null;
        }

        public IEnumerator<KdTreeNode<T>> GetEnumerator() => new InOrderEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<KdTreeNode<T>> GetInOrderEnumerable() => new InOrderEnumerable(this);

        public IEnumerable<KdTreeNode<T>> GetLevelOrderEnumerable() => new LevelOrderEnumerable(this);

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

        private class InOrderEnumerable : IEnumerable<KdTreeNode<T>>
        {
            private readonly KdTreeNode<T> _root;

            public InOrderEnumerable(KdTreeNode<T> root) => _root = root;

            public IEnumerator<KdTreeNode<T>> GetEnumerator() => new InOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class InOrderEnumerator : IEnumerator<KdTreeNode<T>>
        {
            private KdTreeNode<T> _root;
            private Stack<KdTreeNode<T>> _stack;

            public InOrderEnumerator(KdTreeNode<T> root)
            {
                _root = root;
                _stack = new Stack<KdTreeNode<T>>();
            }

            public KdTreeNode<T> Current { get; private set; }

            object IEnumerator.Current => Current;

            //Not neccessary
            public void Dispose() { }

            public bool MoveNext()
            {
                if (Current == null)
                    Current = _root;
                else
                    Current = Current.Right;

                while (true)
                {
                    if (Current != null)
                    {
                        _stack.Push(Current);
                        Current = Current.Left;
                    }
                    else
                    {
                        if (_stack.Count > 0)
                        {
                            Current = _stack.Pop();
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }

            public void Reset()
            {
                _stack.Clear();
                Current = null;
            }
        }

        private class LevelOrderEnumerable : IEnumerable<KdTreeNode<T>>
        {
            private readonly KdTreeNode<T> _root;

            public LevelOrderEnumerable(KdTreeNode<T> root) => _root = root;

            public IEnumerator<KdTreeNode<T>> GetEnumerator() => new LevelOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class LevelOrderEnumerator : IEnumerator<KdTreeNode<T>>
        {
            private readonly KdTreeNode<T> _root;
            private readonly Queue<KdTreeNode<T>> _queue;

            public LevelOrderEnumerator(KdTreeNode<T> root)
            {
                _root = root;
                _queue = new Queue<KdTreeNode<T>>();
                _queue.Enqueue(_root);
            }

            public KdTreeNode<T> Current { get; private set; }

            object IEnumerator.Current => Current;

            //Not neccessary
            public void Dispose()
            { }

            public bool MoveNext()
            {
                if (_queue.Count > 0)
                {
                    Current = _queue.Dequeue();

                    if (Current.Left != null)
                        _queue.Enqueue(Current.Left);

                    if (Current.Right != null)
                        _queue.Enqueue(Current.Right);

                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _queue.Clear();
                _queue.Enqueue(_root);
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
