using SurveyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Helper
{
    /// <summary>
    /// Defines static fields used across this project
    /// </summary>
    public static class StaticFields
    {
        /// <summary>
        /// List of <see cref="LocationType"/> values
        /// </summary>
        public static IEnumerable<LocationType> LocationTypeValues => Enum.GetValues(typeof(LocationType)).Cast<LocationType>();
    }
}
