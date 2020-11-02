namespace Structures.Interface
{
    /// <summary>
    /// Defines operations used in saving/loading (serializing/deserializing) object
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Serializes object to CSV string
        /// </summary>
        /// <param name="delimiter">Delimiter used in CSV string</param>
        /// <returns>Object in CSV string</returns>
        public string ToCsv(string delimiter = ",");

        /// <summary>
        /// Deserializes object from CSV string
        /// </summary>
        /// <param name="csv">CSV string</param>
        /// <param name="delimiter">Delimiter used in CSV string</param>
        public void FromCsv(string csv, string delimiter = ",");
    }
}
