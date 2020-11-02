using SurveyApp.Model;

namespace SurveyApp.Helper
{
    /// <summary>
    /// Used to generate locations according specific criteria
    /// </summary>
    public static class LocationPrototype
    {
        /// <summary>
        /// Returns tuple of two locations based on <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria according to wich locations are created</param>
        /// <returns>Tuple of two locations created according to <paramref name="criteria"/></returns>
        public static (Location lowerBound, Location upperBound) GetLocationsByCriteria(SearchCriteria criteria)
        {
            var lowerBound = new Location()
            {
                Latitude = criteria.MinLatitude,
                Longitude = criteria.MinLongitude
            };

            var upperBound = new Location()
            {
                Latitude = criteria.MaxLatitude,
                Longitude = criteria.MaxLongitude
            };

            return (lowerBound, upperBound);
        }
    }
}
