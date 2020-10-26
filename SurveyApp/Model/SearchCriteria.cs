namespace SurveyApp.Model
{
    public class SearchCriteria
    {
        public LocationType? LocationType { get; set; }

        public double? MinLatitude { get; set; }

        public double? MaxLatitude { get; set; }

        public double? MinLongitude { get; set; }

        public double? MaxLongitude { get; set; }
    }
}
