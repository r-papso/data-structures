using Structures.Hepler;
using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structures.Tree
{
    internal class KdTree<T> : IBSPTree<T> where T : IKdComparable
    {
        private KdTreeNode<T> _root;
        private KdComparer<T> _comparer = new KdComparer<T>();

        public IEnumerable<T> InOrderTraversal
        {
            get
            {
                if (_root == null)
                    yield break;

                foreach (var node in _root.GetInOrderEnumerable())
                    yield return node.Data;
            }
        }

        public IEnumerable<T> LevelOrderTraversal
        {
            get
            {
                if (_root == null)
                    yield break;

                foreach (var node in _root.GetLevelOrderEnumerable())
                    yield return node.Data;
            }
        }

        public KdTree() { }

        public KdTree(T data) => _root = new KdTreeNode<T>(data, 0);

        public KdTree(IEnumerable<T> data) => _root = new KdTreeNode<T>(data);

        //Only for testing purposes
        public int GetDepth() => _root?.Max(x => x.Level + 1) ?? 0;

        public ICollection<T> Find(T data) => Find(data, data);

        public ICollection<T> Find(T lowerBound, T upperBound)
        {
            var result = new LinkedList<T>();

            if (_root == null || !_comparer.GreaterThanOrEqual(upperBound, lowerBound))
                return result;

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

                    if (_comparer.Between(actualNode.Data, lowerBound, upperBound))
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
                throw new ArgumentException($"Data with same values as {nameof(data)} already exist");

            var newNode = new KdTreeNode<T>(data, nearest.Level + 1);
            newNode.Parent = nearest;
            if (CompareKeys(data, nearest.Data, nearest.Level) <= 0)
                nearest.Left = newNode;
            else
                nearest.Right = newNode;
        }

        public void Update(T oldData, T newData)
        {
            var nearestOld = Nearest(oldData, true);

            if (nearestOld == null)
                throw new ArgumentException($"Data passed as argument {nameof(oldData)} not found");

            if (!_comparer.Equal(nearestOld.Data, newData))
            {
                var nearestNew = Nearest(newData, false);

                if (nearestNew.Data.Identical(newData))
                    throw new ArgumentException($"Data with same values as {nameof(newData)} already exist");

                var newNode = new KdTreeNode<T>(newData, nearestNew.Level + 1);
                newNode.Parent = nearestNew;
                if (CompareKeys(newData, nearestNew.Data, nearestNew.Level) <= 0)
                    nearestNew.Left = newNode;
                else
                    nearestNew.Right = newNode;

                Delete(nearestOld);
            }
            else
            {
                nearestOld.Data = newData;
            }
        }

        public void Delete(T data)
        {
            var nodeToDelete = Nearest(data, true);

            if (nodeToDelete == null)
                throw new ArgumentException($"Data passed as argument {nameof(data)} not found");

            Delete(nodeToDelete);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_root == null)
                yield break;

            foreach (var node in _root)
                yield return node.Data;
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

        private void Delete(KdTreeNode<T> nodeToDelete)
        {
            if (nodeToDelete.IsLeaf)
            {
                if (nodeToDelete == _root)
                    _root = null;
                nodeToDelete.Delete();
                return;
            }

            KdTreeNode<T> substitute = null;
            var replaced = new Stack<KdTreeNode<T>>();
            replaced.Push(nodeToDelete);

            while (true)
            {
                if (nodeToDelete.Left == null)
                {
                    nodeToDelete.Left = nodeToDelete.Right;
                    nodeToDelete.Right = null;
                }

                substitute = InOrderNearest(nodeToDelete, out bool _);

                if (substitute.IsLeaf)
                    break;

                replaced.Push(substitute);
                nodeToDelete = substitute;
            }

            nodeToDelete = substitute;
            var substituteData = substitute.Data;

            while (replaced.Count > 0)
            {
                var predecessor = replaced.Pop();
                var temp = substituteData;
                substituteData = predecessor.Data;
                predecessor.Data = temp;
            }

            nodeToDelete.Delete();
        }

        private KdTreeNode<T> InOrderNearest(KdTreeNode<T> node, out bool successor)
        {
            KdTreeNode<T> result = null;
            successor = false;

            if (node.Left != null)
            {
                foreach (var child in node.Left)
                {
                    if (result == null)
                        result = child;
                    else if (CompareKeys(child.Data, result.Data, node.Level) > 0)
                        result = child;
                    else if (CompareKeys(child.Data, result.Data, node.Level) == 0)
                    {
                        if (child.IsLeaf)
                            result = child;
                        else if (child.Level > result.Level && !result.IsLeaf)
                            result = child;
                    }
                }
            }
            else if (node.Right != null)
            {
                foreach (var child in node.Right)
                {
                    if (result == null)
                        result = child;
                    else if (CompareKeys(child.Data, result.Data, node.Level) < 0)
                        result = child;
                    else if (CompareKeys(child.Data, result.Data, node.Level) == 0)
                    {
                        if (child.IsLeaf)
                            result = child;
                        else if (child.Level > result.Level && !result.IsLeaf)
                            result = child;
                    }
                }

                successor = true;
            }

            return result;
        }

        private int CompareKeys(T left, T right, int level)
        {
            var dimension = level % left.DimensionCount;
            return _comparer.Compare(left, right, dimension);
        }
    }
}
