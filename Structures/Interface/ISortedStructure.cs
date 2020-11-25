namespace Structures.Interface
{
    public interface ISortedStructure<T> : IStructure<T>
    {
        public T Min { get; }

        public T Max { get; }
    }
}
