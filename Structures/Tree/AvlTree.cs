using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Structures.Tree
{
    internal class AvlTree<T> : ISortedTree<T> where T : IComparable
    {
        private AvlTreeNode<T> _root;

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

        public T Min => throw new NotImplementedException();

        public T Max => throw new NotImplementedException();

        public AvlTree() { }

        public ICollection<T> Find(T data)
        {
            var result = new LinkedList<T>();

            if (_root == null)
                return result;

            var found = Nearest(data);

            if (found != null && found.Data.CompareTo(data) == 0)
                result.AddLast(found.Data);

            return result;
        }

        public ICollection<T> Find(T lowerBound, T upperBound)
        {
            var result = new LinkedList<T>();

            if (_root == null || lowerBound.CompareTo(upperBound) > 0)
                return result;

            var stack = new Stack<AvlTreeNode<T>>();
            var actualNode = _root;

            while (stack.Count > 0 || actualNode != null)
            {
                if (actualNode != null)
                {
                    stack.Push(actualNode);
                    actualNode = (AvlTreeNode<T>)(lowerBound.CompareTo(actualNode.Data) < 0 ? actualNode.Left : null);
                }
                else
                {
                    actualNode = stack.Pop();

                    if (lowerBound.CompareTo(actualNode.Data) <= 0 && actualNode.Data.CompareTo(upperBound) <= 0)
                    {
                        result.AddLast(actualNode.Data);
                    }
                    actualNode = (AvlTreeNode<T>)(upperBound.CompareTo(actualNode.Data) > 0 ? actualNode.Right : null);
                }
            }

            return result;
        }

        public void Insert(T data)
        {
            if (_root == null)
            {
                _root = new AvlTreeNode<T>(data);
                return;
            }

            var nearest = Nearest(data);

            if (nearest.Data.CompareTo(data) == 0)
                throw new ArgumentException("Cannot insert duplicate values");

            var newNode = new AvlTreeNode<T>(data);
            newNode.Parent = nearest;
            sbyte factor;

            if (nearest.Data.CompareTo(data) < 0)
            {
                factor = -1;
                nearest.Left = newNode;
                nearest.Balance--;
            }
            else
            {
                factor = 1;
                nearest.Right = newNode;
                nearest.Balance++;
            }

            var actual = nearest;
            var parent = (AvlTreeNode<T>)actual.Parent;

            while (true)
            {
                if (parent == null)
                    break;

                parent.Balance += factor;

                if (parent.Balance == 0)
                    break;

                if (parent.Balance < -1)
                {
                    if (actual.Balance < 0)
                    {
                        actual.Balance = 0;
                        parent.Balance = 0;
                        RightRotation(actual);
                        break;
                    }
                    else
                    {
                        ((AvlTreeNode<T>)actual.Left).Balance = 0;
                        ((AvlTreeNode<T>)actual.Parent).Balance = 0;
                        actual.Balance = -1;
                        LeftRotation(actual.Left);
                        RightRotation(actual.Left);
                        break;
                    }
                }
                else if (parent.Balance > 1)
                {
                    if (actual.Balance > 0)
                    {
                        actual.Balance = 0;
                        parent.Balance = 0;
                        LeftRotation(actual);
                        break;
                    }
                    else
                    {
                        ((AvlTreeNode<T>)actual.Right).Balance = 0;
                        ((AvlTreeNode<T>)actual.Parent).Balance = 0;
                        actual.Balance = -1;
                        RightRotation(actual.Right);
                        LeftRotation(actual.Right);
                        break;
                    }
                }
            }
        }

        public void Update(T oldData, T newData)
        {
            if (oldData.CompareTo(newData) == 0)
            {
                var node = Nearest(oldData);

                if (node == null || node.Data.CompareTo(oldData) != 0)
                    throw new ArgumentException("Data not found");

                node.Data = newData;
            }
            else
            {
                Delete(oldData);
                Insert(newData);
            }
        }

        public void Delete(T data)
        {

        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_root == null)
                yield break;

            foreach (var item in _root)
                yield return item.Data;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private AvlTreeNode<T> Nearest(T data)
        {
            if (_root == null)
                return null;

            AvlTreeNode<T> actualNode = _root;

            while (true)
            {
                if (actualNode.Data.CompareTo(data) == 0)
                    return actualNode;

                if (actualNode.Data.CompareTo(data) < 0)
                {
                    if (actualNode.Left == null)
                        break;
                    actualNode = (AvlTreeNode<T>)actualNode.Left;
                }
                else
                {
                    if (actualNode.Right == null)
                        break;
                    actualNode = (AvlTreeNode<T>)actualNode.Right;
                }
            }

            return actualNode;
        }

        private void LeftRotation(TreeNode<T> node)
        {
            if (node == _root)
                throw new ArgumentException("Cannot perform rotation on root");

            var parent = node.Parent;
            var parentsParent = parent.Parent;
            var leftChild = node.Left;

            parent.Right = leftChild;
            parent.Parent = node;
            node.Parent = parentsParent;
            node.Left = parent;
        }

        private void RightRotation(TreeNode<T> node)
        {
            if (node == _root)
                throw new ArgumentException("Cannot perform rotation on root");

            var parent = node.Parent;
            var parentsParent = parent.Parent;
            var rightChild = node.Right;

            parent.Left = rightChild;
            parent.Parent = node;
            node.Parent = parentsParent;
            node.Right = parent;
        }
    }
}
