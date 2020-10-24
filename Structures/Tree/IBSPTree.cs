﻿using System.Collections.Generic;

namespace Structures.Tree
{
    public interface IBSPTree<T> : IEnumerable<T> where T : IKDComparable
    {
        //Only for testing purposes
        public int GetDepth();

        public ICollection<T> Find(T data);

        public ICollection<T> Find(T lowerBound, T upperBound);

        public void Update(T data);

        public void Insert(T data);

        public void Delete(T data);
    }
}
