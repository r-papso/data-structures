using SurveyApp.Model;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SurveyApp.Helper
{
    public class LocationEqualityComparer : EqualityComparer<Location>
    {
        public override bool Equals([AllowNull] Location x, [AllowNull] Location y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            return x.Identical(y);
        }

        public override int GetHashCode([DisallowNull] Location obj)
        {
            unchecked
            {
                int hash = obj.ID.GetHashCode();
                hash = 31 * hash + obj.LocationType.GetHashCode();
                hash = 31 * hash + obj.Latitude.GetHashCode();
                hash = 31 * hash + obj.Longitude.GetHashCode();
                return hash;
            }
        }
    }
}
