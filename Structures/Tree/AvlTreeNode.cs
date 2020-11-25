namespace Structures.Tree
{
    internal class AvlTreeNode<T> : TreeNode<T>
    {
        public sbyte Balance { get; set; }

        public AvlTreeNode(T data) : base(data) => Balance = 0;
    }
}
