using Structures.Tree;
using System.Collections.Generic;

namespace Structures
{
    public class StructureFactory
    {
        private static object _lock = new object();
        private static volatile StructureFactory _instance;

        protected StructureFactory() { }

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

        public IBSPTree<T> GetBSPTree<T>() where T : IKDComparable
        {
            return new KdTree<T>();
        }

        public IBSPTree<T> GetBSPTree<T>(IEnumerable<T> data) where T : IKDComparable
        {
            return new KdTree<T>(data);
        }
    }
}
