using Structures;
using Structures.Tree;
using SurveyApp.Event;
using SurveyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Service
{
    public class LocationManager
    {
        private IBSPTree<Location> _properties;
        private IBSPTree<Location> _sites;

        public delegate void LocationsChangedEventHandler(object sender, LocationsChangedEventArgs e);

        public event LocationsChangedEventHandler LocationsChanged;

        public LocationManager()
        {
            _properties = StructureFactory.Instance.GetBSPTree<Location>();
            _sites = StructureFactory.Instance.GetBSPTree<Location>();
        }

        public void FindLocationsByCriteria(SearchCriteria criteria)
        {
            if (criteria.LocationType == LocationType.Property)
            {
                OnLocationsChanged(Find(criteria), Enumerable.Empty<Location>());
            }
            else
            {
                OnLocationsChanged(Enumerable.Empty<Location>(), Find(criteria));
            }
        }

        public void InsertLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
            {
                _properties.Insert(location);
            }
            else
            {
                _sites.Insert(location);
            }

            OnLocationsChanged(_properties, _sites);
        }

        public void UpdateLocation(Location oldLocation, Location newLocation)
        {
            if (oldLocation.LocationType != newLocation.LocationType)
                throw new ArgumentException("Changing type of existing location is not allowed");

            if (oldLocation.LocationType == LocationType.Property)
            {
                _properties.Update(oldLocation, newLocation);
            }
            else
            {
                _sites.Update(oldLocation, newLocation);
            }

            OnLocationsChanged(_properties, _sites);
        }

        public void DeleteLocation(Location location)
        {
            if (location.LocationType == LocationType.Property)
            {
                _properties.Delete(location);
            }
            else
            {
                _sites.Delete(location);
            }

            OnLocationsChanged(_properties, _sites);
        }

        public void GenerateLocations(GenerationCriteria criteria)
        {
            if (criteria.LocationType == LocationType.Property)
            {
                _properties = StructureFactory.Instance.GetBSPTree(Generate(criteria));
            }
            else
            {
                _sites = StructureFactory.Instance.GetBSPTree(Generate(criteria));
            }

            OnLocationsChanged(_properties, _sites);
        }

        public void Reset() => OnLocationsChanged(_properties, _sites);

        protected void OnLocationsChanged(IEnumerable<Location> properties, IEnumerable<Location> sites)
        {
            LocationsChanged?.Invoke(this, new LocationsChangedEventArgs(properties, sites));
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

                var searchCriteriaLocType = criteria.LocationType == LocationType.Property ? LocationType.Site : LocationType.Property;
                var searchCriteria = new SearchCriteria()
                {
                    LocationType = searchCriteriaLocType,
                    MinLatitude = latitude,
                    MinLongitude = longitude
                };

                location.SituatedLocations = Find(searchCriteria);
                locations.Add(location);
            }

            return locations;
        }

        private IEnumerable<Location> Find(SearchCriteria criteria)
        {
            var minLocation = new Location()
            {
                Latitude = criteria.MinLatitude,
                Longitude = criteria.MinLongitude
            };

            var maxLocation = new Location()
            {
                Latitude = criteria.MaxLatitude,
                Longitude = criteria.MaxLongitude
            };

            if (criteria.LocationType == LocationType.Property)
            {
                return _properties.Find(minLocation, maxLocation);
            }
            else
            {
                return _sites.Find(minLocation, maxLocation);
            }
        }
    }
}
