using System;
using System.Diagnostics.CodeAnalysis;

namespace ReverseGeocode.Processors;

internal class GpsCoordinate
    : IEquatable<GpsCoordinate>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }


    public override bool Equals(object obj)
    {
        return Equals(obj as GpsCoordinate);
    }


    public bool Equals([AllowNull] GpsCoordinate other)
    {
        return other != null &&
            Latitude == other.Latitude &&
            Longitude == other.Longitude;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }
}
