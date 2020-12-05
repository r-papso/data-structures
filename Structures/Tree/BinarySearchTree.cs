using Structures.Interface;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Structures.Tree
{
    internal abstract class BinarySearchTree<T> : IBSTree<T>
    {
        public int Count { get; protected set; }

        public T Min
        {
            get
            {
                if (Root == null)
                    throw new InvalidOperationException("Tree is empty");

                BinaryTreeNode<T> actualNode = Root;

                while (true)
                {
                    if (actualNode.Left != null)
                        actualNode = actualNode.Left;
                    else
                        return actualNode.Data;
                }
            }
        }

        public T Max
        {
            get
            {
                if (Root == null)
                    throw new InvalidOperationException("Tree is empty");

                BinaryTreeNode<T> actualNode = Root;

                while (true)
                {
                    if (actualNode.Right != null)
                        actualNode = actualNode.Right;
                    else
                        return actualNode.Data;
                }
            }
        }

        public IEnumerable<T> InOrderTraversal
        {
            get
            {
                if (Root == null)
                    yield break;

                foreach (var node in Root.GetInOrderEnumerable())
                    yield return node.Data;
            }
        }

        public IEnumerable<T> LevelOrderTraversal
        {
            get
            {
                if (Root == null)
                    yield break;

                foreach (var node in Root.GetLevelOrderEnumerable())
                    yield return node.Data;
            }
        }

        protected BinaryTreeNode<T> Root { get; set; }

        public virtual ICollection<T> Find(T data) => Find(data, data);

        public virtual ICollection<T> Find(T lowerBound, T upperBound)
        {
            var result = new LinkedList<T>();

            if (Root == null || LowerGreaterThanUpper(lowerBound, upperBound))
                return result;

            var stack = new Stack<BinaryTreeNode<T>>();
            var actualNode = Root;

            while (stack.Count > 0 || actualNode != null)
            {
                if (actualNode != null)
                {
                    stack.Push(actualNode);
                    actualNode = CanGoLeft(actualNode, lowerBound) ? actualNode.Left : null;
                }
                else
                {
                    actualNode = stack.Pop();

                    if (Between(actualNode, lowerBound, upperBound))
                        result.AddLast(actualNode.Data);

                    actualNode = CanGoRight(actualNode, upperBound) ? actualNode.Right : null;
                }
            }

            return result;
        }

        public abstract void Insert(T data);

        public abstract void Update(T oldData, T newData);

        public abstract void Delete(T data);

        public IEnumerator<T> GetEnumerator()
        {
            if (Root == null)
                yield break;

            foreach (var item in Root)
                yield return item.Data;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected abstract bool LowerGreaterThanUpper(T lower, T upper);

        protected abstract bool CanGoLeft(BinaryTreeNode<T> actualNode, T data);

        protected abstract bool CanGoRight(BinaryTreeNode<T> actualNode, T data);

        protected abstract bool Between(BinaryTreeNode<T> actualNode, T lower, T upper);
    }
}
