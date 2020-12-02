using System;

namespace Structures.Tree
{
    internal class AvlTree<T> : BinarySearchTree<T> where T : IComparable
    {
        public AvlTree() { }

        public override void Insert(T data)
        {
            if (Root == null)
            {
                Root = new AvlTreeNode<T>(data);
                Count++;
                return;
            }

            var nearest = Nearest(data);

            if (nearest.Data.CompareTo(data) == 0)
                throw new ArgumentException("Cannot insert duplicate values");

            var newNode = new AvlTreeNode<T>(data);
            newNode.Parent = nearest;

            if (nearest.Data.CompareTo(data) < 0)
                nearest.Right = newNode;
            else
                nearest.Left = newNode;

            var actual = newNode;
            AvlTreeNode<T> last = null;

            while (true)
            {
                if (actual == null)
                    break;

                if (last != null)
                    ChangeParentFactor(last, true);

                if ((actual.Balance == 0 && last != null))
                    break;

                if (actual.Balance < -1 || actual.Balance > 1)
                {
                    Balance(actual);
                    break;
                }

                last = actual;
                actual = (AvlTreeNode<T>)last.Parent;
            }

            Count++;
        }

        public override void Update(T oldData, T newData)
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

        public override void Delete(T data)
        {
            var node = (AvlTreeNode<T>)Nearest(data);

            if (node == null || node.Data.CompareTo(data) != 0)
                throw new ArgumentException("Data not found");

            (var nodeToDelete, var actual) = Delete(node);

            if (actual == null)
            {
                Count--;
                return;
            }

            AvlTreeNode<T> last = actual;
            actual = (AvlTreeNode<T>)last.Parent;

            while (true)
            {
                if (actual == null)
                    break;

                if (actual.Balance == 0)
                {
                    ChangeParentFactor(last, false);
                    break;
                }

                ChangeParentFactor(last, false);

                if ((actual.Balance < -1 && ((AvlTreeNode<T>)actual.Left).Balance == 0) || (actual.Balance > 1 && ((AvlTreeNode<T>)actual.Right).Balance == 0))
                {
                    Balance(actual);
                    break;
                }

                actual = Balance(actual);

                last = actual;
                actual = (AvlTreeNode<T>)last.Parent;
            }

            nodeToDelete.Delete();
            Count--;
        }

        protected override bool LowerGreaterThanUpper(T lower, T upper)
        {
            return lower.CompareTo(upper) > 0;
        }

        protected override bool CanGoLeft(BinaryTreeNode<T> actualNode, T data)
        {
            return data.CompareTo(actualNode.Data) < 0;
        }

        protected override bool CanGoRight(BinaryTreeNode<T> actualNode, T data)
        {
            return data.CompareTo(actualNode.Data) > 0;
        }

        protected override bool Between(BinaryTreeNode<T> actualNode, T lower, T upper)
        {
            return lower.CompareTo(actualNode.Data) <= 0 && upper.CompareTo(actualNode.Data) >= 0;
        }

        private BinaryTreeNode<T> Nearest(T data)
        {
            if (Root == null)
                return null;

            var actualNode = Root;

            while (true)
            {
                if (actualNode.Data.CompareTo(data) == 0)
                    return actualNode;

                if (actualNode.Data.CompareTo(data) < 0)
                {
                    if (actualNode.Right == null)
                        break;
                    actualNode = actualNode.Right;
                }
                else
                {
                    if (actualNode.Left == null)
                        break;
                    actualNode = actualNode.Left;
                }
            }

            return actualNode;
        }

        private (AvlTreeNode<T> nodeToDelete, AvlTreeNode<T> actual) Delete(AvlTreeNode<T> node)
        {
            (AvlTreeNode<T> nodeToDelete, AvlTreeNode<T> actual) = (null, null);

            if (node.Left != null && node.Right != null)
            {
                var substitute = Predecessor(node);
                node.Data = substitute.Data;

                if (substitute.Right != null)
                {
                    SwapNodes(substitute, substitute.Right);
                    nodeToDelete = substitute;
                    actual = (AvlTreeNode<T>)substitute.Right;
                }
                else if (substitute.Left != null)
                {
                    SwapNodes(substitute, substitute.Left);
                    nodeToDelete = substitute;
                    actual = (AvlTreeNode<T>)substitute.Left;
                }
                else
                {
                    (nodeToDelete, actual) = (substitute, substitute);
                }
            }
            else
            {
                if (node.IsLeaf)
                {
                    if (node == Root)
                    {
                        Root = null;
                        return (null, null);
                    }
                    (nodeToDelete, actual) = (node, node);
                }
                else
                {
                    if (node.Right != null)
                    {
                        SwapNodes(node, node.Right);
                        nodeToDelete = node;
                        actual = (AvlTreeNode<T>)node.Right;
                    }
                    else
                    {
                        SwapNodes(node, node.Left);
                        nodeToDelete = node;
                        actual = (AvlTreeNode<T>)node.Left;
                    }
                }
            }

            return (nodeToDelete, actual);
        }

        private void SwapNodes(BinaryTreeNode<T> node, BinaryTreeNode<T> replaced)
        {
            if (node.Parent == null)
            {
                Root = replaced;
                Root.Parent = null;
            }
            else
            {
                replaced.Parent = node.Parent;

                if (node.IsLeftChild)
                    replaced.Parent.Left = replaced;
                else if (node.IsRightChild)
                    replaced.Parent.Right = replaced;
                else
                    throw new System.Exception();
            }
        }

        private AvlTreeNode<T> Balance(AvlTreeNode<T> node)
        {
            if (node.Balance < -1)
            {
                var prevNode = (AvlTreeNode<T>)node.Left;

                if (prevNode.Balance < 0)
                {
                    node.Balance = 0;
                    prevNode.Balance = 0;
                    RightRotation(prevNode);

                    return prevNode;
                }
                else if (prevNode.Balance == 0)
                {
                    node.Balance = -1;
                    prevNode.Balance = 1;
                    RightRotation(prevNode);

                    return prevNode;
                }
                else
                {
                    var grandChild = (AvlTreeNode<T>)prevNode.Right;
                    var grandChildBalance = 0;

                    if (grandChild != null)
                    {
                        grandChildBalance = grandChild.Balance;
                        grandChild.Balance = 0;
                    }

                    if (grandChildBalance == 0)
                    {
                        prevNode.Balance = 0;
                        node.Balance = 0;
                    }
                    else if (grandChildBalance == -1)
                    {
                        prevNode.Balance = 0;
                        node.Balance = 1;
                    }
                    else
                    {
                        prevNode.Balance = -1;
                        node.Balance = 0;
                    }

                    LeftRotation(grandChild);
                    RightRotation(grandChild);

                    return grandChild;
                }
            }
            else if (node.Balance > 1)
            {
                var prevNode = (AvlTreeNode<T>)node.Right;

                if (prevNode.Balance > 0)
                {
                    node.Balance = 0;
                    prevNode.Balance = 0;
                    LeftRotation(prevNode);

                    return prevNode;
                }
                else if (prevNode.Balance == 0)
                {
                    node.Balance = 1;
                    prevNode.Balance = -1;
                    LeftRotation(prevNode);

                    return prevNode;
                }
                else
                {
                    var grandChild = (AvlTreeNode<T>)prevNode.Left;
                    var grandChildBalance = 0;

                    if (grandChild != null)
                    {
                        grandChildBalance = grandChild.Balance;
                        grandChild.Balance = 0;
                    }

                    if (grandChildBalance == 0)
                    {
                        prevNode.Balance = 0;
                        node.Balance = 0;
                    }
                    else if (grandChildBalance == 1)
                    {
                        prevNode.Balance = 0;
                        node.Balance = -1;
                    }
                    else
                    {
                        prevNode.Balance = 1;
                        node.Balance = 0;
                    }

                    RightRotation(grandChild);
                    LeftRotation(grandChild);

                    return grandChild;
                }
            }

            return node;
        }

        private void ChangeParentFactor(BinaryTreeNode<T> actual, bool inserting)
        {
            var parent = (AvlTreeNode<T>)actual.Parent;

            if (actual.IsLeftChild)
                parent.Balance = (sbyte)(inserting ? parent.Balance - 1 : parent.Balance + 1);
            else if (actual.IsRightChild)
                parent.Balance = (sbyte)(inserting ? parent.Balance + 1 : parent.Balance - 1);
            else
                throw new System.Exception();
        }

        private void LeftRotation(AvlTreeNode<T> node)
        {
            if (node == Root)
                throw new ArgumentException("Cannot perform rotation on root");

            var parent = node.Parent;
            var isLeftChild = parent.IsLeftChild;
            var parentsParent = parent.Parent;
            var leftChild = node.Left;

            parent.Right = leftChild;
            if (leftChild != null)
                leftChild.Parent = parent;

            parent.Parent = node;
            node.Left = parent;
            node.Parent = parentsParent;

            if (node.Parent != null)
            {
                if (isLeftChild)
                    node.Parent.Left = node;
                else
                    node.Parent.Right = node;
            }
            else
                Root = node;
        }

        private void RightRotation(AvlTreeNode<T> node)
        {
            if (node == Root)
                throw new ArgumentException("Cannot perform rotation on root");

            var parent = node.Parent;
            var isLeftChild = parent.IsLeftChild;
            var parentsParent = parent.Parent;
            var rightChild = node.Right;

            parent.Left = rightChild;
            if (rightChild != null)
                rightChild.Parent = parent;

            parent.Parent = node;
            node.Right = parent;
            node.Parent = parentsParent;

            if (node.Parent != null)
            {
                if (isLeftChild)
                    node.Parent.Left = node;
                else
                    node.Parent.Right = node;
            }
            else
                Root = node;
        }

        private AvlTreeNode<T> Predecessor(BinaryTreeNode<T> node)
        {
            AvlTreeNode<T> result = null;

            if (node.Left != null)
            {
                foreach (AvlTreeNode<T> child in node.Left)
                {
                    if (result == null || result.Data.CompareTo(child.Data) < 0)
                        result = child;
                }
            }

            return result;
        }
    }
}
