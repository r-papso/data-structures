using System.Collections.Generic;

namespace Structures.Interface
{
    /// <summary>
    /// Defines operations available in binary search tree structure
    /// </summary>
    /// <typeparam name="T">Type of elements stored in tree</typeparam>
    public interface IBSTree<T> : ITable<T>
    {
        /// <summary>
        /// Count of elements in tree
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Minimum value of tree
        /// </summary>
        public T Min { get; }

        /// <summary>
        /// Maximum value of tree
        /// </summary>
        public T Max { get; }

        /// <summary>
        /// Provides in order traversal over tree
        /// </summary>
        public IEnumerable<T> InOrderTraversal { get; }

        /// <summary>
        /// Provides level order traversal over tree
        /// </summary>
        public IEnumerable<T> LevelOrderTraversal { get; }

        /// <summary>
        /// Finds all occurences in <see cref="IBSTree{T}"/> between <paramref name="lowerBound"/> and <paramref name="upperBound"/>
        /// </summary>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="upperBound">Upper bound</param>
        /// <returns>All elements that are between <paramref name="lowerBound"/> and <paramref name="upperBound"/></returns>
        public ICollection<T> Find(T lowerBound, T upperBound);
    }
}
