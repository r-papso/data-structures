using SurveyApp.Model;
using System.Collections.Generic;

namespace SurveyApp.Event
{
    public class LocationsChangedEventArgs
    {
        public IEnumerable<Location> Locations { get; }

        public LocationsChangedEventArgs(IEnumerable<Location> locations)
        {
            Locations = locations;
        }
    }
}
