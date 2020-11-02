using Structures.Hepler;
using SurveyApp.Helper;
using SurveyApp.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace SurveyApp.Service
{
    /// <summary>
    /// Manages operations performed on <see cref="Sites"/> and <see cref="Properties"/>
    /// </summary>
    public class LocationManager
    {
        private static string _propertiesFile = "Properties.csv";
        private static string _sitesFile = "Sites.csv";

        /// <summary>
        /// Properties' collection
        /// </summary>
        public CollectionAdapter<Location> Properties { get; }

        /// <summary>
        /// Sites' collection
        /// </summary>
        public CollectionAdapter<Location> Sites { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocationManager()
        {
            Properties = new CollectionAdapter<Location>();
            Sites = new CollectionAdapter<Location>();
        }

        /// <summary>
        /// Finds locations by specified <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used in locations' search</param>
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

        /// <summary>
        /// Inserts a location
        /// </summary>
        /// <param name="location">Location to be inserted</param>
        public void InsertLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
                Properties.Insert(location);
            else
                Sites.Insert(location);

            AddSituatedLocations(location);
        }

        /// <summary>
        /// Updates a location
        /// </summary>
        /// <param name="oldLocation">Old location values</param>
        /// <param name="newLocation">New location values</param>
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

        /// <summary>
        /// Deletes a location
        /// </summary>
        /// <param name="location">Location to be deleted</param>
        public void DeleteLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
                Properties.Delete(location);
            else
                Sites.Delete(location);

            RemoveSituatedLocations(location);
        }

        /// <summary>
        /// Generates locations according to <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used for generate locations</param>
        public void GenerateLocations(GenerationCriteria criteria)
        {
            if (criteria.LocationType == LocationType.Property)
                Properties.Generate(Generate(criteria));
            else
                Sites.Generate(Generate(criteria));
        }

        /// <summary>
        /// Saves <see cref="Properties"/> and <see cref="Sites"/> to file
        /// </summary>
        /// <param name="folderPath">Folder in which <see cref="Properties"/> and <see cref="Sites"/> are being saved</param>
        public void SaveLocations(string folderPath)
        {
            Properties.Save(Path.Combine(folderPath, _propertiesFile));
            Sites.Save(Path.Combine(folderPath, _sitesFile));
        }

        /// <summary>
        /// Loads <see cref="Properties"/> and <see cref="Sites"/> from file
        /// </summary>
        /// <param name="folderPath">Folder from which <see cref="Properties"/> and <see cref="Sites"/> are being loaded</param>
        public void LoadLocations(string folderPath)
        {
            var propFilePath = Path.Combine(folderPath, _propertiesFile);
            var siteFilePath = Path.Combine(folderPath, _sitesFile);
            bool propFileExists = File.Exists(propFilePath);
            bool siteFileExists = File.Exists(siteFilePath);

            if (propFileExists)
                Properties.Load(propFilePath);

            if (siteFileExists)
                Sites.Load(siteFilePath);

            if (propFileExists && siteFileExists)
            {
                foreach (var property in Properties.Tree)
                {
                    AddSituatedLocations(property);
                }
            }
        }

        /// <summary>
        /// Resets all previous searches
        /// </summary>
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
            if (location.LocationType == LocationType.Property)
                location.SituatedLocations = Sites.Get(location);
            else
                location.SituatedLocations = Properties.Get(location);

            foreach (var situated in location.SituatedLocations)
            {
                situated.SituatedLocations.Add(location);
            }
        }
    }
}
