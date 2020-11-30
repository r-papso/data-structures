using System.Collections.Generic;

namespace Structures.Interface
{
    public interface IStructure<T> : IEnumerable<T>
    {
        /// <summary>
        /// Finds all occurences in <see cref="IStructure{T}"/> of <paramref name="data"/> parameter,
        /// if structure implementing this interface does not support duplicate values, returned collection
        /// will contain only one element
        /// </summary>
        /// <param name="data">Data to be found</param>
        /// <returns>All occurences of <paramref name="data"/></returns>
        public ICollection<T> Find(T data);

        /// <summary>
        /// Updates values of <paramref name="oldData"/> element to <paramref name="newData"/> values
        /// </summary>
        /// <param name="oldData">Element to be updated</param>
        /// <param name="newData">New element values</param>
        public void Update(T oldData, T newData);

        /// <summary>
        /// Inserts new element into <see cref="IStructure{T}"/>
        /// </summary>
        /// <param name="data">Element to be inserted into <see cref="IStructure{T}"/></param>
        public void Insert(T data);

        /// <summary>
        /// Removes element from <see cref="IStructure{T}"/>
        /// </summary>
        /// <param name="data">Element to be removed from <see cref="IStructure{T}"/></param>
        public void Delete(T data);
    }
}
