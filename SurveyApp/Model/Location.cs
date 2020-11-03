using Structures.Exception;
using Structures.Interface;
using SurveyApp.Interface;
using System;
using System.Collections.Generic;

namespace SurveyApp.Model
{
    /// <summary>
    /// Represents possible location types
    /// </summary>
    public enum LocationType : byte { Property, Site };

    /// <summary>
    /// Represents instance if either property or site
    /// </summary>
    public class Location : IKdComparable, ISaveable
    {
        private static int _dimensionsCount = 2;

        /// <summary>
        /// ID of location
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Location's type
        /// </summary>
        public LocationType LocationType { get; set; }

        /// <summary>
        /// Location's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Location's latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Location's longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Locations with same <see cref="Latitude"/> and <see cref="Longitude"/>
        /// </summary>
        public ISet<Location> SituatedLocations { get; set; } = new HashSet<Location>();

        /// <summary>
        /// Number of location's dimensions
        /// </summary>
        public int DimensionsCount => _dimensionsCount;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Location() { }

        /// <summary>
        /// Constructor specifying all properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationType"></param>
        /// <param name="description"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Location(int id, LocationType locationType, string description, double latitude, double longitude)
        {
            ID = id;
            LocationType = locationType;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">location to be copied from</param>
        public Location(Location other)
        {
            if (other != null)
            {
                ID = other.ID;
                LocationType = other.LocationType;
                Description = other.Description;
                Latitude = other.Latitude;
                Longitude = other.Longitude;
            }
        }

        public IComparable GetKey(int dimension)
        {
            switch (dimension)
            {
                case 0:
                    return Latitude;
                case 1:
                    return Longitude;
                default:
                    throw new DimensionRangeException(dimension, _dimensionsCount);
            }
        }

        public bool Identical(IKdComparable other)
        {
            var otherLoc = other as Location;

            if (otherLoc == null)
                return false;

            return ID == otherLoc.ID && LocationType == otherLoc.LocationType && Latitude == otherLoc.Latitude && Longitude == otherLoc.Longitude;
        }

        public string ToCsv(string delimiter = ",")
        {
            return $"{ID}{delimiter}{LocationType:D}{delimiter}{Description}{delimiter}{Latitude}{delimiter}{Longitude}";
        }

        public void FromCsv(string csv, string delimiter = ",")
        {
            var props = csv.Split(new string[] { delimiter }, StringSplitOptions.None);

            ID = Int32.Parse(props[0]);
            LocationType = (LocationType)Byte.Parse(props[1]);
            Description = props[2];
            Latitude = Double.Parse(props[3]);
            Longitude = Double.Parse(props[4]);
        }
    }
}
