﻿using System;

namespace Structures.Interface
{
    /// <summary>
    /// Defines operations required by <see cref="IBSPTree{T}"/> data structure
    /// </summary>
    public interface IKdComparable
    {
        /// <summary>
        /// Number of dimensions in which object is comparable
        /// </summary>
        public int DimensionsCount { get; }

        /// <summary>
        /// Gets key of k-th dimension
        /// </summary>
        /// <param name="dimension">K-th dimension of object</param>
        /// <returns>Key of k-th dimension</returns>
        public IComparable GetKey(int dimension);

        /// <summary>
        /// Determines if two <see cref="IKdComparable"/> are identical objects
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if objects are Identical, False otherwise</returns>
        public bool Identical(IKdComparable other);
    }
}
