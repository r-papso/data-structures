using SurveyApp.Model;
using System;

namespace SurveyApp.Service
{
    public class LocationManager : Manager<Location>
    {
        private readonly Random _rand = new Random();

        public LocationManager() : base()
        { }

        public override Location GetLocalizable()
        {
            return new Location();
        }

        public override Location GenerateLocalizable(int locId)
        {
            return new Location(locId, locId, _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), $"Location {locId}");
        }
    }
}
