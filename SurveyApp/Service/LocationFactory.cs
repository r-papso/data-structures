using Structures.Interface;
using SurveyApp.Model;
using System;

namespace SurveyApp.Service
{
    public class LocationFactory : Factory
    {
        private readonly Random _rand = new Random();

        public LocationFactory() { }

        public override ISerializable GetSerializable()
        {
            return new Location();
        }

        public override ISerializable GetSerializable(int id)
        {
            return new Location(id, id, _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), _rand.Next(0, 1000), $"Location {id}");
        }
    }
}
