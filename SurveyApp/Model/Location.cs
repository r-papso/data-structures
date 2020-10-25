using Structures.Exception;
using Structures.Tree;
using System;
using System.Collections.Generic;

namespace SurveyApp.Model
{
    public enum LocationType : byte { Property, Site };

    public class Location : IKdComparable
    {
        private static int _dimensionsCount = 2;

        public int ID { get; set; }

        public LocationType LocationType { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ICollection<Location> Locations { get; set; } = new LinkedList<Location>();

        public int DimensionsCount => _dimensionsCount;

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
            return ID == otherLoc.ID && LocationType == otherLoc.LocationType && Latitude == otherLoc.Latitude && Longitude == otherLoc.Longitude;
        }
    }
}
