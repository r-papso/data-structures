using Structures;
using SurveyApp.Adapter;
using SurveyApp.Model;
using System;
using System.Collections.Generic;

namespace SurveyApp.Service
{
    /// <summary>
    /// Manages operations performed on <see cref="Sites"/> and <see cref="Properties"/>
    /// </summary>
    public class LocationManager
    {
        /// <summary>
        /// Properties' collection
        /// </summary>
        public HashFileAdapter<Location> Locations { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocationManager() => Locations = new HashFileAdapter<Location>();

        /// <summary>
        /// Finds locations by specified <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used in locations' search</param>
        public void FindLocations(int locationId)
        {
            var loc = new Location() { ID = locationId };
            Locations.Find(loc);
        }

        /// <summary>
        /// Inserts a location
        /// </summary>
        /// <param name="location">Location to be inserted</param>
        public void InsertLocation(Location location) => Locations.Insert(location);

        /// <summary>
        /// Updates a location
        /// </summary>
        /// <param name="oldLocation">Old location values</param>
        /// <param name="newLocation">New location values</param>
        public void UpdateLocation(Location oldLocation, Location newLocation) => Locations.Update(oldLocation, newLocation);

        /// <summary>
        /// Deletes a location
        /// </summary>
        /// <param name="location">Location to be deleted</param>
        public void DeleteLocation(int locationId) => Locations.Delete(new Location() { ID = locationId });

        /// <summary>
        /// Generates locations according to <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used for generate locations</param>
        public void GenerateLocations(GenerationCriteria criteria) => Locations.Generate(Generate(criteria));

        /// <summary>
        /// Initializes new database at specified directory
        /// </summary>
        /// <param name="directory">Directory where the database will be created</param>
        /// <param name="clusterSize">File system's cluster size in bytes</param>
        public void NewDatabase(string directory, int clusterSize) => Locations.New(directory, clusterSize);

        /// <summary>
        /// Loads existing database from specified directory
        /// </summary>
        /// <param name="directory">Directory from where database will be loaded</param>
        public void LoadDatabase(string directory) => Locations.Load(directory);

        /// <summary>
        /// Releases all resources held by database
        /// </summary>
        public void Release() => Locations.Release();

        private IEnumerable<Location> Generate(GenerationCriteria criteria)
        {
            var locations = new List<Location>();
            var randId = new Random();
            var randxy = new Random();
            var usedIds = StructureFactory.Instance.GetHashSet<int>();

            for (int i = 0; i < criteria.LocationsCount; i++)
            {
                var id = i;

                if (criteria.RandomIds)
                {
                    id = randId.Next();

                    while (usedIds.Find(id).Count != 0)
                        id = randId.Next();

                    usedIds.Insert(id);
                }

                var newLoc = new Location(id, i, randxy.Next(0, 1000), randxy.Next(0, 1000), randxy.Next(0, 1000), randxy.Next(0, 1000), $"Location {id}");
                locations.Add(newLoc);
            }

            return locations;
        }
    }
}
