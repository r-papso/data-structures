using SurveyApp.Helper;
using SurveyApp.Model;
using System;
using System.Collections.Generic;

namespace SurveyApp.Service
{
    public class LocationManager
    {
        //private IBSPTree<Location> _properties;
        //private IBSPTree<Location> _sites;

        public CollectionAdapter<Location> Properties { get; }

        public CollectionAdapter<Location> Sites { get; }

        //public delegate void LocationsChangedEventHandler(object sender, LocationsChangedEventArgs e);

        //public event LocationsChangedEventHandler LocationsChanged;

        public LocationManager()
        {
            //_properties = StructureFactory.Instance.GetBSPTree<Location>();
            //_sites = StructureFactory.Instance.GetBSPTree<Location>();
            Properties = new CollectionAdapter<Location>();
            Sites = new CollectionAdapter<Location>();
        }

        public void FindLocationsByCriteria(SearchCriteria criteria)
        {
            /*if (criteria.LocationType == LocationType.Property)
            {
                OnLocationsChanged(Find(criteria), Enumerable.Empty<Location>());
            }
            else
            {
                OnLocationsChanged(Enumerable.Empty<Location>(), Find(criteria));
            }*/
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
                Properties.Find(minLocation, maxLocation);
            else
                Sites.Find(minLocation, maxLocation);
        }

        public void InsertLocation(Location location)
        {
            /*if (location.LocationType == LocationType.Property)
            {
                _properties.Insert(location);
            }
            else
            {
                _sites.Insert(location);
            }

            OnLocationsChanged(_properties, _sites);*/
            if (location.LocationType == LocationType.Property)
                Properties.Insert(location);
            else
                Sites.Insert(location);
        }

        public void UpdateLocation(Location oldLocation, Location newLocation)
        {
            /*if (oldLocation.LocationType != newLocation.LocationType)
                throw new ArgumentException("Changing type of existing location is not allowed");

            if (oldLocation.LocationType == LocationType.Property)
            {
                _properties.Update(oldLocation, newLocation);
                OnLocationsChanged(_properties, null);
            }
            else
            {
                _sites.Update(oldLocation, newLocation);
                OnLocationsChanged(null, _sites);
            }*/

            if (oldLocation.LocationType != newLocation.LocationType)
                throw new ArgumentException("Changing type of existing location is not allowed");

            if (oldLocation.LocationType == LocationType.Property)
                Properties.Update(oldLocation, newLocation);
            else
                Sites.Update(oldLocation, newLocation);
        }

        public void DeleteLocation(Location location)
        {
            /*if (location.LocationType == LocationType.Property)
            {
                _properties.Delete(location);
                OnLocationsChanged(_properties, null);
            }
            else
            {
                _sites.Delete(location);
                OnLocationsChanged(null, _sites);
            }*/

            if (location.LocationType == LocationType.Property)
                Properties.Delete(location);
            else
                Sites.Delete(location);
        }

        public void GenerateLocations(GenerationCriteria criteria)
        {
            /*if (criteria.LocationType == LocationType.Property)
            {
                _properties = StructureFactory.Instance.GetBSPTree(Generate(criteria));
                OnLocationsChanged(_properties, null);
            }
            else
            {
                _sites = StructureFactory.Instance.GetBSPTree(Generate(criteria));
                OnLocationsChanged(null, _sites);
            }*/
            if (criteria.LocationType == LocationType.Property)
                //Properties = new CollectionAdapter<Location>(Generate(criteria));
                Properties.Generate(Generate(criteria));
            else
                //Sites = new CollectionAdapter<Location>(Generate(criteria));
                Sites.Generate(Generate(criteria));
        }

        //public void Reset() => OnLocationsChanged(_properties, _sites);
        public void Reset()
        {
            Properties.Reset();
            Sites.Reset();
        }

        /*protected void OnLocationsChanged(IEnumerable<Location> properties, IEnumerable<Location> sites)
        {
            LocationsChanged?.Invoke(this, new LocationsChangedEventArgs(properties, sites));
        }*/

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

                location.SituatedLocations = Get(searchCriteria);
                locations.Add(location);
            }

            return locations;
        }

        private ICollection<Location> Get(SearchCriteria criteria)
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
                return Properties.Get(minLocation, maxLocation);
            else
                return Sites.Get(minLocation, maxLocation);
        }
    }
}
