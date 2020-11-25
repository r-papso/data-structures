using System.Collections;
using System.Collections.Generic;

namespace Structures.Tree
{
    internal abstract class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        public bool IsLeaf => Right == null && Left == null;

        public TreeNode<T> Left { get; set; }

        public TreeNode<T> Right { get; set; }

        public TreeNode<T> Parent { get; set; }

        public T Data { get; set; }

        public TreeNode() { }

        public TreeNode(T data) => Data = data;

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

        public virtual IEnumerator<TreeNode<T>> GetEnumerator() => new InOrderEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual IEnumerable<TreeNode<T>> GetInOrderEnumerable() => new InOrderEnumerable(this);

        public virtual IEnumerable<TreeNode<T>> GetLevelOrderEnumerable() => new LevelOrderEnumerable(this);

        #region Enumerators

        protected class InOrderEnumerable : IEnumerable<TreeNode<T>>
        {
            private readonly TreeNode<T> _root;

            public InOrderEnumerable(TreeNode<T> root) => _root = root;

            public IEnumerator<TreeNode<T>> GetEnumerator() => new InOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        protected class InOrderEnumerator : IEnumerator<TreeNode<T>>
        {
            private TreeNode<T> _root;
            private Stack<TreeNode<T>> _stack;

            public InOrderEnumerator(TreeNode<T> root)
            {
                _root = root;
                _stack = new Stack<TreeNode<T>>();
            }

            public TreeNode<T> Current { get; private set; }

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

        protected class LevelOrderEnumerable : IEnumerable<TreeNode<T>>
        {
            private readonly TreeNode<T> _root;

            public LevelOrderEnumerable(TreeNode<T> root) => _root = root;

            public IEnumerator<TreeNode<T>> GetEnumerator() => new LevelOrderEnumerator(_root);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        protected class LevelOrderEnumerator : IEnumerator<TreeNode<T>>
        {
            private readonly TreeNode<T> _root;
            private readonly Queue<TreeNode<T>> _queue;

            public LevelOrderEnumerator(TreeNode<T> root)
            {
                _root = root;
                _queue = new Queue<TreeNode<T>>();
                _queue.Enqueue(_root);
            }

            public TreeNode<T> Current { get; private set; }

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
