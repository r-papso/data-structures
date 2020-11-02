using System.Collections.Generic;

namespace Structures.Interface
{
    /// <summary>
    /// Defines operations available in BSP tree structure
    /// </summary>
    /// <typeparam name="T">Type of elements stored in BSP tree</typeparam>
    public interface IBSPTree<T> : IEnumerable<T> where T : IKdComparable, ISaveable, new()
    {
        //Only for testing purposes
        public int GetDepth();

        /// <summary>
        /// Finds all occurences in <see cref="IBSPTree{T}"/> based on <paramref name="data"/> parameter
        /// </summary>
        /// <param name="data">Data to be found</param>
        /// <returns>All elements that are <see cref="Hepler.KdComparer{T}.Equal(T, T)"/> to <paramref name="data"/></returns>
        public ICollection<T> Find(T data);

        /// <summary>
        /// Finds all occurences in <see cref="IBSPTree{T}"/> between <paramref name="lowerBound"/> and <paramref name="upperBound"/>
        /// </summary>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="upperBound">Upper bound</param>
        /// <returns>All elements that are <see cref="Hepler.KdComparer{T}.Between(T, T, T)"/> <paramref name="lowerBound"/> and <paramref name="upperBound"/></returns>
        public ICollection<T> Find(T lowerBound, T upperBound);

        /// <summary>
        /// Updates values of <paramref name="oldData"/> element to <paramref name="newData"/> values
        /// </summary>
        /// <param name="oldData">Element <see cref="IKdComparable.Identical(IKdComparable)"/> to updating element</param>
        /// <param name="newData">New element values</param>
        public void Update(T oldData, T newData);

        /// <summary>
        /// Inserts new element into <see cref="IBSPTree{T}"/>
        /// </summary>
        /// <param name="data">Element to be inserted</param>
        public void Insert(T data);

        /// <summary>
        /// Removes element from <see cref="IBSPTree{T}"/>
        /// </summary>
        /// <param name="data">Element <see cref="IKdComparable.Identical(IKdComparable)"/> to removing element</param>
        public void Delete(T data);

        /// <summary>
        /// Saves BSP tree to CSV file
        /// </summary>
        /// <param name="filePath">Path of file, the file should have .csv extension</param>
        public void Save(string filePath);

        /// <summary>
        /// Loads BSP tree from CSV file
        /// </summary>
        /// <param name="filePath">Path of file, the file has to be created by <see cref="Save(string)"/> method</param>
        public void Load(string filePath);
    }
}
