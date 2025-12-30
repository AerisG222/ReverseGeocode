namespace ReverseGeocode.Models;

public record Location (
    Guid Id,
    decimal Latitude,
    decimal Longitude
);
