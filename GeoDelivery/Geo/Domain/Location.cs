using NetTopologySuite.Geometries;

namespace Geo.Domain;

public class Location
{
    public Location() { }

    public Location(double latitude, double longitude, Point point)
    {
        Latitude = latitude;
        Longitude = longitude;
        Point = point;
    }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Point Point { get; set; } = null!;
}