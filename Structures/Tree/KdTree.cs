using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures.Tree
{
    internal class KdTree<T> : IBSPTree<T> where T : IKDComparable
    {
        private KdTreeNode<T> _root;

        public KdTree() { }

        public KdTree(T data) => _root = new KdTreeNode<T>(data, 0);

        public KdTree(IEnumerable<T> data) => _root = new KdTreeNode<T>(data, 0);

        //Only for testing purposes
        public int GetDepth() => _root.Max(x => x.Level) + 1;

        public ICollection<T> Find(T data) => Find(data, data);

        public ICollection<T> Find(T lowerBound, T upperBound)
        {
            if (_root == null || (!upperBound.GreaterThan(lowerBound) && !upperBound.Equal(lowerBound)))
                return Enumerable.Empty<T>().ToList();

            var result = new LinkedList<T>();
            var stack = new Stack<KdTreeNode<T>>();
            var actualNode = _root;

            while (stack.Count > 0 || actualNode != null)
            {
                if (actualNode != null)
                {
                    stack.Push(actualNode);
                    actualNode = CompareKeys(lowerBound, actualNode.Data, actualNode.Level) <= 0 ? actualNode.Left : null;
                }
                else
                {
                    actualNode = stack.Pop();
                    if (actualNode.Data.Between(lowerBound, upperBound))
                    {
                        result.AddLast(actualNode.Data);
                    }
                    actualNode = CompareKeys(upperBound, actualNode.Data, actualNode.Level) > 0 ? actualNode.Right : null;
                }
            }

            return result;
        }

        public void Insert(T data)
        {
            if (_root == null)
            {
                _root = new KdTreeNode<T>(data, 0);
                return;
            }

            var nearest = Nearest(data, false);

            if (nearest.Data.Identical(data))
                throw new ArgumentException("Cannot insert exact same data");

            var newNode = new KdTreeNode<T>(data, nearest.Level + 1);
            if (CompareKeys(data, nearest.Data, nearest.Level) <= 0)
                nearest.Left = newNode;
            else
                nearest.Right = newNode;
        }

        public void Update(T data)
        {
            var nearest = Nearest(data, true);

            if (nearest == null)
                throw new ArgumentException("Data not found");

            nearest.Data = data;
        }

        public void Delete(T data)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in _root)
            {
                yield return node.Data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private KdTreeNode<T> Nearest(T data, bool exactMatch)
        {
            if (_root == null)
                return null;

            KdTreeNode<T> actualNode = _root;

            while (true)
            {
                if (exactMatch && actualNode.Data.Identical(data))
                    return actualNode;

                if (CompareKeys(data, actualNode.Data, actualNode.Level) <= 0)
                {
                    if (actualNode.Left == null)
                        break;
                    actualNode = actualNode.Left;
                }
                else
                {
                    if (actualNode.Right == null)
                        break;
                    actualNode = actualNode.Right;
                }
            }

            if (!exactMatch)
                return actualNode;
            else
                return null;
        }

        private int CompareKeys(T left, T right, int level)
        {
            var dimension = level % left.DimensionsCount;
            return left.GetKey(dimension).CompareTo(right.GetKey(dimension));
        }
    }
}
