using Structures.Interface;
using SurveyApp.Model;
using System;

namespace SurveyApp.Service
{
    public class LocationManager : Manager
    {
        private readonly Random _rand = new Random();

        public LocationManager() : base()
        { }

        public override ISerializable GetSerializable()
        {
            return new Location();
        }

        public override ISerializable GenerateSerializable(int id)
        {
            return new Location(id, id, _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), $"Location {id}");
        }
    }
}
