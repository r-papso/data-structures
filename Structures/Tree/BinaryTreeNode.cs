using System.Collections;
using System.Collections.Generic;

namespace Structures.Tree
{
    internal abstract class BinaryTreeNode<T> : IEnumerable<BinaryTreeNode<T>>
    {
        public bool IsLeaf => Right == null && Left == null;

        public bool IsLeftChild => Parent != null && Parent.Left == this;

        public bool IsRightChild => Parent != null && Parent.Right == this;

        public BinaryTreeNode<T> Left { get; set; }

        public BinaryTreeNode<T> Right { get; set; }

        public BinaryTreeNode<T> Parent { get; set; }

        public T Data { get; set; }

        public BinaryTreeNode() { }

        public BinaryTreeNode(T data) => Data = data;

        public void Delete()
        {
            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = null;
                else if (Parent.Right == this)
                    Parent.Right = null;
            }

            Parent = null;
            Left = null;
            Right = null;
        }

        public IEnumerator<BinaryTreeNode<T>> GetEnumerator() => new InOrderEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<BinaryTreeNode<T>> GetInOrderEnumerable() => new InOrderEnumerable(this);

        public IEnumerable<BinaryTreeNode<T>> GetLevelOrderEnumerable() => new LevelOrderEnumerable(this);

        #region Enumerators

        private class InOrderEnumerable : IEnumerable<BinaryTreeNode<T>>
        {
            private readonly BinaryTreeNode<T> _root;

            public InOrderEnumerable(BinaryTreeNode<T> root) => _root = root;

            public IEnumerator<BinaryTreeNode<T>> GetEnumerator() => new InOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class InOrderEnumerator : IEnumerator<BinaryTreeNode<T>>
        {
            private BinaryTreeNode<T> _root;
            private Stack<BinaryTreeNode<T>> _stack;

            public InOrderEnumerator(BinaryTreeNode<T> root)
            {
                _root = root;
                _stack = new Stack<BinaryTreeNode<T>>();
            }

            public BinaryTreeNode<T> Current { get; private set; }

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

        private class LevelOrderEnumerable : IEnumerable<BinaryTreeNode<T>>
        {
            private readonly BinaryTreeNode<T> _root;

            public LevelOrderEnumerable(BinaryTreeNode<T> root) => _root = root;

            public IEnumerator<BinaryTreeNode<T>> GetEnumerator() => new LevelOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class LevelOrderEnumerator : IEnumerator<BinaryTreeNode<T>>
        {
            private readonly BinaryTreeNode<T> _root;
            private readonly Queue<BinaryTreeNode<T>> _queue;

            public LevelOrderEnumerator(BinaryTreeNode<T> root)
            {
                _root = root;
                _queue = new Queue<BinaryTreeNode<T>>();
                _queue.Enqueue(_root);
            }

            public BinaryTreeNode<T> Current { get; private set; }

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

        #endregion
    }
}
