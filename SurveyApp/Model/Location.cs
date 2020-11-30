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
        private static int _dimensionCount = 2;
        private static int _idSequence = 0;

        /// <summary>
        /// Unique ID of location
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID of location
        /// </summary>
        public int Number { get; set; }

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
        public ICollection<Location> SituatedLocations { get; set; } = new LinkedList<Location>();

        /// <summary>
        /// Number of location's dimensions
        /// </summary>
        public int DimensionCount => _dimensionCount;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Location() => ID = ++_idSequence;

        /// <summary>
        /// Constructor specifying all properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationType"></param>
        /// <param name="description"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Location(int number, LocationType locationType, string description, double latitude, double longitude)
        {
            ID = ++_idSequence;
            Number = number;
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
                Number = other.Number;
                LocationType = other.LocationType;
                Description = other.Description;
                Latitude = other.Latitude;
                Longitude = other.Longitude;
                SituatedLocations = other.SituatedLocations;
            }
            ID = ++_idSequence;
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
                    throw new DimensionRangeException(dimension, _dimensionCount);
            }
        }

        public bool Identical(IKdComparable other)
        {
            var otherLoc = other as Location;

            if (otherLoc == null)
                return false;

            return ID == otherLoc.ID && Number == otherLoc.Number && LocationType == otherLoc.LocationType && Latitude == otherLoc.Latitude && Longitude == otherLoc.Longitude;
        }

        public string ToCsv(string delimiter = ",")
        {
            return $"{ID}{delimiter}{Number}{delimiter}{LocationType:D}{delimiter}{Description}{delimiter}{Latitude}{delimiter}{Longitude}";
        }

        public void FromCsv(string csv, string delimiter = ",")
        {
            var props = csv.Split(new string[] { delimiter }, StringSplitOptions.None);

            ID = Int32.Parse(props[0]);
            Number = Int32.Parse(props[1]);
            LocationType = (LocationType)Byte.Parse(props[2]);
            Description = props[3];
            Latitude = Double.Parse(props[4]);
            Longitude = Double.Parse(props[5]);
        }
    }
}
