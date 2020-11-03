namespace SurveyApp.Model
{
    /// <summary>
    /// Model object used in <see cref="View.GenerateWindow"/> View
    /// </summary>
    public class GenerationCriteria
    {
        /// <summary>
        /// <see cref="Model.LocationType"/> of generated locations
        /// </summary>
        public LocationType LocationType { get; set; }

        /// <summary>
        /// Determines if <see cref="Location.Latitude"/> and <see cref="Location.Longitude"/> of generated locations will be integral values or not
        /// </summary>
        public bool IntegralValues { get; set; }

        /// <summary>
        /// Number of generated locations
        /// </summary>
        public int LocationsCount { get; set; }

        /// <summary>
        /// Minimal value of <see cref="Location.Latitude"/> and <see cref="Location.Longitude"/>
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Maximal value of <see cref="Location.Latitude"/> and <see cref="Location.Longitude"/>
        /// </summary>
        public int MaxValue { get; set; }
    }
}
