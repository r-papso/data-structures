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
        /// Gets BSP tree
        /// </summary>
        /// <typeparam name="T">Type of elements in BSP tree</typeparam>
        /// <returns>Empty BSP tree</returns>
        public IBSPTree<T> GetBSPTree<T>() where T : IKdComparable, ISaveable, new()
        {
            return new KdTree<T>();
        }

        /// <summary>
        /// Constructs balanced BSP tree from <paramref name="data"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in BSP tree</typeparam>
        /// <param name="data">Data used in BSP tree construction</param>
        /// <returns>Balanced BSP tree constructed from <paramref name="data"/></returns>
        public IBSPTree<T> GetBSPTree<T>(IEnumerable<T> data) where T : IKdComparable, ISaveable, new()
        {
            return new KdTree<T>(data);
        }
    }
}
