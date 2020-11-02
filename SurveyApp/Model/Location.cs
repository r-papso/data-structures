using Structures.Exception;
using Structures.Tree;
using System;
using System.Collections.Generic;

namespace SurveyApp.Model
{
    public enum LocationType : byte { Property, Site };

    public class Location : IKdComparable, ISaveable
    {
        private static int _dimensionsCount = 2;

        public int ID { get; set; }

        public LocationType LocationType { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ICollection<Location> SituatedLocations { get; set; } = new LinkedList<Location>();

        public int DimensionsCount => _dimensionsCount;

        public Location() { }

        public Location(int id, LocationType locationType, string description, double latitude, double longitude)
        {
            ID = id;
            LocationType = locationType;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
        }

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
