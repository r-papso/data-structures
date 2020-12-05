namespace Structures.Tree
{
    internal class AvlTreeNode<T> : BinaryTreeNode<T>
    {
        public sbyte Balance { get; set; }

        public AvlTreeNode(T data) : base(data) => Balance = 0;
    }
}
