using Structures.Helper;
using Structures.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Structures.Tree
{
    internal class KdTree<T> : BinarySearchTree<T> where T : IKdComparable
    {
        private KdComparer<T> _comparer = new KdComparer<T>();

        public KdTree() { }

        public KdTree(IEnumerable<T> data)
        {
            Root = new KdTreeNode<T>(data);
            Count = data.Count();
        }

        public override void Insert(T data)
        {
            if (Root == null)
            {
                Root = new KdTreeNode<T>(data, 0);
                Count++;
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

            Count++;
        }

        public override void Update(T oldData, T newData)
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

        public override void Delete(T data)
        {
            var nodeToDelete = Nearest(data, true);

            if (nodeToDelete == null)
                throw new ArgumentException($"Data passed as argument {nameof(data)} not found");

            Delete(nodeToDelete);

            Count--;
        }

        protected override bool LowerGreaterThanUpper(T lower, T upper)
        {
            return !_comparer.GreaterThanOrEqual(upper, lower);
        }

        protected override bool CanGoLeft(BinaryTreeNode<T> actualNode, T data)
        {
            return CompareKeys(data, actualNode.Data, ((KdTreeNode<T>)actualNode).Level) <= 0;
        }

        protected override bool CanGoRight(BinaryTreeNode<T> actualNode, T data)
        {
            return CompareKeys(data, actualNode.Data, ((KdTreeNode<T>)actualNode).Level) > 0;
        }

        protected override bool Between(BinaryTreeNode<T> actualNode, T lower, T upper)
        {
            return _comparer.Between(actualNode.Data, lower, upper);
        }

        private KdTreeNode<T> Nearest(T data, bool exactMatch)
        {
            if (Root == null)
                return null;

            KdTreeNode<T> actualNode = (KdTreeNode<T>)Root;

            while (true)
            {
                if (exactMatch && actualNode.Data.Identical(data))
                    return actualNode;

                if (CompareKeys(data, actualNode.Data, actualNode.Level) <= 0)
                {
                    if (actualNode.Left == null)
                        break;
                    actualNode = (KdTreeNode<T>)actualNode.Left;
                }
                else
                {
                    if (actualNode.Right == null)
                        break;
                    actualNode = (KdTreeNode<T>)actualNode.Right;
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
                if (nodeToDelete == Root)
                    Root = null;
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
                foreach (KdTreeNode<T> child in node.Left)
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
                foreach (KdTreeNode<T> child in node.Right)
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
