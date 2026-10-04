namespace Geo.Domain;

public class Address
{
    public Address() { }

    public Address(string city, string street, string building, string? apartment, Location coordinates)
    {
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Coordinates = coordinates;
    }
    
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string? Apartment { get; set; }
    public Location Coordinates { get; set; } = null!;
}