using SurveyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Helper
{
    public static class StaticFields
    {
        public static IEnumerable<LocationType> LocationTypeValues => Enum.GetValues(typeof(LocationType)).Cast<LocationType>();
    }
}
