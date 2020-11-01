using Structures.Hepler;
using SurveyApp.Helper;
using SurveyApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SurveyApp.Service
{
    public class LocationManager
    {
        private static string _delimiter = ";";
        private static string _propertiesFile = "Properties.csv";
        private static string _sitesFile = "Sites.csv";

        public CollectionAdapter<Location> Properties { get; }

        public CollectionAdapter<Location> Sites { get; }

        public LocationManager()
        {
            Properties = new CollectionAdapter<Location>();
            Sites = new CollectionAdapter<Location>();
        }

        public void FindLocationsByCriteria(SearchCriteria criteria)
        {
            (var lowerBound, var upperBound) = LocationPrototype.GetLocationsByCriteria(criteria);

            if (criteria.LocationType == LocationType.Property)
            {
                Properties.Find(lowerBound, upperBound);
                Sites.SetEmptyFound();
            }
            else
            {
                Sites.Find(lowerBound, upperBound);
                Properties.SetEmptyFound();
            }
        }

        public void InsertLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
                Properties.Insert(location);
            else
                Sites.Insert(location);

            AddSituatedLocations(location);
        }

        public void UpdateLocation(Location oldLocation, Location newLocation)
        {
            if (oldLocation.LocationType != newLocation.LocationType)
                throw new ArgumentException("Changing type of existing location is not allowed");

            if (oldLocation.LocationType == LocationType.Property)
                Properties.Update(oldLocation, newLocation);
            else
                Sites.Update(oldLocation, newLocation);

            var comparer = new KdComparer<Location>();

            if (!comparer.Equal(oldLocation, newLocation))
            {
                AddSituatedLocations(newLocation);
                RemoveSituatedLocations(oldLocation);
            }
        }

        public void DeleteLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
                Properties.Delete(location);
            else
                Sites.Delete(location);

            RemoveSituatedLocations(location);
        }

        public void GenerateLocations(GenerationCriteria criteria)
        {
            if (criteria.LocationType == LocationType.Property)
                Properties.Generate(Generate(criteria));
            else
                Sites.Generate(Generate(criteria));
        }

        public void SaveLocations(string folderPath)
        {
            Properties.Save(Path.Combine(folderPath, _propertiesFile), _delimiter);
            Sites.Save(Path.Combine(folderPath, _sitesFile), _delimiter);
        }

        public void LoadLocations(string folderPath)
        {
            var propFilePath = Path.Combine(folderPath, _propertiesFile);
            var siteFilePath = Path.Combine(folderPath, _sitesFile);

            if (File.Exists(propFilePath))
                Properties.Load(Path.Combine(folderPath, _propertiesFile), _delimiter);

            if (File.Exists(siteFilePath))
                Sites.Load(Path.Combine(folderPath, _sitesFile), _delimiter);

            if (File.Exists(propFilePath) && File.Exists(siteFilePath))
            {
                var propDict = new Dictionary<int, Location>(Properties.Tree.Select(x => new KeyValuePair<int, Location>(x.ID, x)));
                var siteDict = new Dictionary<int, Location>(Sites.Tree.Select(x => new KeyValuePair<int, Location>(x.ID, x)));

                foreach (var property in Properties.Tree)
                {
                    foreach (var siteId in property.SituatedLocationIds)
                    {
                        property.SituatedLocations.Add(siteDict[siteId]);
                    }
                    property.SituatedLocationIds = null;
                }

                foreach (var site in Sites.Tree)
                {
                    foreach (var propertyId in site.SituatedLocationIds)
                    {
                        site.SituatedLocations.Add(propDict[propertyId]);
                    }
                    site.SituatedLocationIds = null;
                }
            }
        }

        public void Reset()
        {
            Properties.Reset();
            Sites.Reset();
        }

        private IEnumerable<Location> Generate(GenerationCriteria criteria)
        {
            var locations = new List<Location>();
            var rand = new Random();

            for (int i = 0; i < criteria.LocationsCount; i++)
            {
                var description = $"{criteria.LocationType} {i}";
                var latitude = criteria.IntegralValues ? rand.Next(criteria.MinValue, criteria.MaxValue)
                                                        : (rand.NextDouble() + criteria.MinValue) * (criteria.MaxValue - criteria.MinValue);
                var longitude = criteria.IntegralValues ? rand.Next(criteria.MinValue, criteria.MaxValue)
                                                        : (rand.NextDouble() + criteria.MinValue) * (criteria.MaxValue - criteria.MinValue);
                var location = new Location(i, criteria.LocationType, description, latitude, longitude);

                AddSituatedLocations(location);
                locations.Add(location);
            }

            return locations;
        }

        private void RemoveSituatedLocations(Location location)
        {
            foreach (var situated in location.SituatedLocations)
            {
                situated.SituatedLocations.Remove(location);
            }
        }

        private void AddSituatedLocations(Location location)
        {
            var criteria = LocationPrototype.GetCriteriaByLocation(location);
            (var lowerBound, var upperBound) = LocationPrototype.GetLocationsByCriteria(criteria);

            if (criteria.LocationType == LocationType.Property)
                location.SituatedLocations = Properties.Get(lowerBound, upperBound);
            else
                location.SituatedLocations = Sites.Get(lowerBound, upperBound);

            foreach (var situated in location.SituatedLocations)
            {
                situated.SituatedLocations.Add(location);
            }
        }
    }
}
