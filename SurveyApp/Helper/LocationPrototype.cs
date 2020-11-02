using SurveyApp.Model;

namespace SurveyApp.Helper
{
    public static class LocationPrototype
    {
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
