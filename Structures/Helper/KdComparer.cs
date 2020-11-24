using Structures.Interface;
using System.Collections.Generic;

namespace Structures.Helper
{
    /// <summary>
    /// Comparer used to compare two <see cref="IKdComparable"/> objects
    /// </summary>
    /// <typeparam name="T">Type of object implementing <see cref="IKdComparable"/></typeparam>
    public class KdComparer<T> : IComparer<T> where T : IKdComparable
    {
        /// <summary>
        /// Actual dimension used in <see cref="Compare(T, T)"/> method
        /// </summary>
        public int Dimension { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public KdComparer() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">Dimension used in <see cref="Compare(T, T)"/> method</param>
        public KdComparer(int dimension) => Dimension = dimension;

        /// <summary>
        /// Compares two <see cref="IKdComparable"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>-1 if <paramref name="left"/> is less than <paramref name="right"/>, 0 if operands are equal,
        /// 1 if <paramref name="left"/> is greater than <paramref name="right"/></returns>
        public int Compare(T left, T right)
        {
            return left.GetKey(Dimension).CompareTo(right.GetKey(Dimension));
        }

        /// <summary>
        /// Compares two <see cref="IKdComparable"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <param name="dimension">Dimension used in comparison</param>
        /// <returns>-1 if <paramref name="left"/> is less than <paramref name="right"/>, 0 if operands are equal,
        /// 1 if <paramref name="left"/> is greater than <paramref name="right"/></returns>
        public int Compare(T left, T right, int dimension)
        {
            return left.GetKey(dimension).CompareTo(right.GetKey(dimension));
        }

        /// <summary>
        /// Determines if <paramref name="data"/> is between <paramref name="lower"/> and <paramref name="upper"/>
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>True if <paramref name="data"/> is between <paramref name="lower"/> and <paramref name="upper"/>, False othwerwise</returns>
        public bool Between(T data, T lower, T upper)
        {
            for (int i = 0; i < data.DimensionCount; i++)
            {
                if (lower.GetKey(i).CompareTo(data.GetKey(i)) > 0 || upper.GetKey(i).CompareTo(data.GetKey(i)) < 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is less than <paramref name="right"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>, False otherwise</returns>
        public bool LessThan(T left, T right)
        {
            for (int i = 0; i < left.DimensionCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) >= 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is less than or equal to <paramref name="right"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True if <paramref name="left"/> is less than or equal to <paramref name="right"/>, False otherwise</returns>
        public bool LessThanOrEqual(T left, T right)
        {
            for (int i = 0; i < left.DimensionCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is equal to <paramref name="right"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>, False otherwise</returns>
        public bool Equal(T left, T right)
        {
            for (int i = 0; i < left.DimensionCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) != 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is greater than <paramref name="right"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>, False otherwise</returns>
        public bool GreaterThan(T left, T right)
        {
            for (int i = 0; i < left.DimensionCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) <= 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is greater than or equal to <paramref name="right"/>
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True if <paramref name="left"/> is greater than or equal to <paramref name="right"/>, False otherwise</returns>
        public bool GreaterThanOrEqual(T left, T right)
        {
            for (int i = 0; i < left.DimensionCount; i++)
            {
                if (left.GetKey(i).CompareTo(right.GetKey(i)) < 0)
                    return false;
            }
            return true;
        }
    }
}
