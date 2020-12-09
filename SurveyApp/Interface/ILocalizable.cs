namespace SurveyApp.Interface
{
    /// <summary>
    /// Represents localizable object
    /// </summary>
    public interface ILocalizable
    {
        /// <summary>
        /// Unique ID of localizable
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Localizable's first longitude coordinate
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Localizable's first latitude coordinate
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Localizable's second longitude coordinate
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Localizable's second latitude coordinate
        /// </summary>
        public float Y2 { get; set; }
    }
}
