using SurveyApp.Model;
using System.Collections.Generic;

namespace SurveyApp.Event
{
    public class LocationsChangedEventArgs
    {
        public IEnumerable<Location> Properties { get; }

        public IEnumerable<Location> Sites { get; }

        public LocationsChangedEventArgs(IEnumerable<Location> properties, IEnumerable<Location> sites)
        {
            Properties = properties;
            Sites = sites;
        }
    }
}
