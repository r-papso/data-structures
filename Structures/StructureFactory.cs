using Structures.Hashing;
using Structures.Interface;
using Structures.Tree;
using System.Collections.Generic;

namespace Structures
{
    /// <summary>
    /// Factory for providing instances of data structures in <see cref="Structures"/> namespace
    /// </summary>
    public class StructureFactory
    {
        private static object _lock = new object();
        private static volatile StructureFactory _instance;

        protected StructureFactory() { }

        /// <summary>
        /// Instance of the factory
        /// </summary>
        public static StructureFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new StructureFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets K-d tree
        /// </summary>
        /// <typeparam name="T">Type of elements in K-d tree</typeparam>
        /// <returns>Empty K-d tree</returns>
        public ITree<T> GetKdTree<T>() where T : IKdComparable
        {
            return new KdTree<T>();
        }

        /// <summary>
        /// Constructs balanced K-d tree from <paramref name="data"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in K-d tree</typeparam>
        /// <param name="data">Data used in K-d tree construction</param>
        /// <returns>Balanced K-d tree constructed from <paramref name="data"/></returns>
        public ITree<T> GetKdTree<T>(IEnumerable<T> data) where T : IKdComparable
        {
            return new KdTree<T>(data);
        }

        public IFileStructure<T> GetExtendibleHashing<T>(int clusterSize) where T : ISerializable, new()
        {
            return new ExtendibleHashing<T>(clusterSize);
        }

        public IStructure<T> GetHashSet<T>()
        {
            return new Hashing.HashSet<T>();
        }

        public IStructure<T> GetHashSet<T>(int initialCapacity)
        {
            return new Hashing.HashSet<T>(initialCapacity);
        }
    }
}
