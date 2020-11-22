using System.Collections.Generic;

namespace Structures.Interface
{
    /// <summary>
    /// Defines operations available in tree structure
    /// </summary>
    /// <typeparam name="T">Type of elements stored in tree</typeparam>
    public interface ITree<T> : IStructure<T>
    {
        /// <summary>
        /// Provides in order traversal over tree
        /// </summary>
        public IEnumerable<T> InOrderTraversal { get; }

        /// <summary>
        /// Provides level order traversal over tree
        /// </summary>
        public IEnumerable<T> LevelOrderTraversal { get; }

        /// <summary>
        /// Gets depth of <see cref="ITree{T}"/>
        /// </summary>
        /// <returns>Depth of <see cref="ITree{T}"/></returns>
        public int GetDepth();

        /// <summary>
        /// Finds all occurences in <see cref="ITree{T}"/> between <paramref name="lowerBound"/> and <paramref name="upperBound"/>
        /// </summary>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="upperBound">Upper bound</param>
        /// <returns>All elements that are between <paramref name="lowerBound"/> and <paramref name="upperBound"/></returns>
        public ICollection<T> Find(T lowerBound, T upperBound);
    }
}
