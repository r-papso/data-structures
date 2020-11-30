namespace Structures.Exception
{
    /// <summary>
    /// Represents error during obtaining key of dimension out of range of <see cref="Interface.IKdComparable"/> object's dimensions
    /// </summary>
    public class DimensionRangeException : System.Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DimensionRangeException() : base() { }

        /// <summary>
        /// Constructor for throwing exception with custom message
        /// </summary>
        /// <param name="message">Message of exception</param>
        public DimensionRangeException(string message) : base(message) { }

        /// <summary>
        /// Useful for throwing exception with apropiate message
        /// </summary>
        /// <param name="outOfRangeDimension">Dimension out of range</param>
        /// <param name="dimensionCount">Number of object's dimensions</param>
        public DimensionRangeException(int outOfRangeDimension, int dimensionCount)
            : base($"Dimension {outOfRangeDimension} is out of range of available dimensions ({dimensionCount})") { }
    }
}
