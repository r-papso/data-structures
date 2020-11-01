using SurveyApp.Model;

namespace SurveyApp.Helper
{
    public static class LocationPrototype
    {
        private static Location _lowerBound = new Location();
        private static Location _upperBound = new Location();
        private static SearchCriteria _criteria = new SearchCriteria();

        public static (Location lowerBound, Location upperBound) GetLocationsByCriteria(SearchCriteria criteria)
        {
            _lowerBound.Latitude = criteria.MinLatitude;
            _lowerBound.Longitude = criteria.MinLongitude;

            _upperBound.Latitude = criteria.MaxLatitude;
            _upperBound.Longitude = criteria.MaxLongitude;

            return (_lowerBound, _upperBound);
        }

        public static SearchCriteria GetCriteriaByLocation(Location location)
        {
            _criteria.LocationType = location.LocationType == LocationType.Property ? LocationType.Site : LocationType.Property;
            _criteria.MinLatitude = location.Latitude;
            _criteria.MaxLatitude = location.Latitude;
            _criteria.MinLongitude = location.Longitude;
            _criteria.MaxLongitude = location.Longitude;

            return _criteria;
        }
    }
}
