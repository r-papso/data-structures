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
        public IBSPTree<Location> Properties { get; private set; }

        public IBSPTree<Location> Sites { get; private set; }

        public delegate void LocationsChangedEventHandler(object sender, LocationsChangedEventArgs e);

        public event LocationsChangedEventHandler LocationsChanged;

        public LocationManager()
        {
            Properties = StructureFactory.Instance.GetBSPTree<Location>();
            Sites = StructureFactory.Instance.GetBSPTree<Location>();
        }

        public ICollection<Location> GetLocationsByCriteria(SearchCriteria criteria)
        {
            var minLocation = new Location()
            {
                Latitude = criteria.MinLatitude.Value,
                Longitude = criteria.MinLongitude.Value
            };

            if (criteria.MaxLatitude != null && criteria.MaxLongitude != null)
            {
                var maxLocation = new Location()
                {
                    Latitude = criteria.MaxLatitude.Value,
                    Longitude = criteria.MaxLongitude.Value
                };

                if (criteria.LocationType == LocationType.Property)
                {
                    return Properties.Find(minLocation, maxLocation);
                }
                else if (criteria.LocationType == LocationType.Site)
                {
                    return Sites.Find(minLocation, maxLocation);
                }
            }

            if (criteria.LocationType == LocationType.Property)
            {
                return Properties.Find(minLocation);
            }
            else if (criteria.LocationType == LocationType.Site)
            {
                return Sites.Find(minLocation);
            }

            return Enumerable.Empty<Location>().ToList();
        }

        public void GenerateLocations(GenerationCriteria criteria)
        {
            if (criteria.LocationType == LocationType.Property)
            {
                Properties = StructureFactory.Instance.GetBSPTree(Generate(criteria));
                OnLocationsChanged(Properties);
            }
            else
            {
                Sites = StructureFactory.Instance.GetBSPTree(Generate(criteria));
                OnLocationsChanged(Sites);
            }
        }

        protected void OnLocationsChanged(IEnumerable<Location> locations)
        {
            LocationsChanged?.Invoke(this, new LocationsChangedEventArgs(locations));
        }

        private IEnumerable<Location> Generate(GenerationCriteria criteria)
        {
            var locations = new List<Location>();

            if (criteria.RandomValues)
            {
                var rand = new Random();

                for (int i = 0; i < criteria.LocationsCount; i++)
                {
                    var description = $"{criteria.LocationType.Value} {i}";
                    var latitude = criteria.IntegralValues ? rand.Next(criteria.MinValue, criteria.MaxValue)
                                                           : (rand.NextDouble() + criteria.MinValue) * (criteria.MaxValue - criteria.MinValue);
                    var longitude = criteria.IntegralValues ? rand.Next(criteria.MinValue, criteria.MaxValue)
                                                            : (rand.NextDouble() + criteria.MinValue) * (criteria.MaxValue - criteria.MinValue);
                    var location = new Location(i, criteria.LocationType.Value, description, latitude, longitude);

                    var searchCriteriaLocType = criteria.LocationType == LocationType.Property ? LocationType.Site : LocationType.Property;
                    var searchCriteria = new SearchCriteria()
                    {
                        LocationType = searchCriteriaLocType,
                        MinLatitude = latitude,
                        MinLongitude = longitude
                    };

                    location.Locations = GetLocationsByCriteria(searchCriteria);

                    locations.Add(location);
                }
            }
            else
            {
                var gridSize = Convert.ToInt32(Math.Sqrt(criteria.LocationsCount));
                int k = 0;

                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        var description = $"{criteria.LocationType.Value} {k}";
                        var location = new Location(k, criteria.LocationType.Value, description, i, j);

                        var searchCriteriaLocType = criteria.LocationType == LocationType.Property ? LocationType.Site : LocationType.Property;
                        var searchCriteria = new SearchCriteria()
                        {
                            LocationType = searchCriteriaLocType,
                            MinLatitude = i,
                            MinLongitude = j
                        };

                        location.Locations = GetLocationsByCriteria(searchCriteria);

                        locations.Add(location);

                        k++;
                    }
                }
            }

            return locations;
        }
    }
}
