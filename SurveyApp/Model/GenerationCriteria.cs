namespace SurveyApp.Model
{
    public class GenerationCriteria
    {
        public LocationType? LocationType { get; set; }

        public bool IntegralValues { get; set; }

        public bool RandomValues { get; set; }

        public int LocationsCount { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}
