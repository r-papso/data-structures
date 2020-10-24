namespace Structures.Exception
{
    public class DimensionRangeException : System.Exception
    {
        public DimensionRangeException() { }

        public DimensionRangeException(string message) : base(message) { }

        public DimensionRangeException(int outOfRangeDimension, int dimensionCount)
            : base($"Dimension {outOfRangeDimension} is out of range of available dimensions ({dimensionCount})") { }
    }
}
