namespace ReverseGeocode.Google;

public class ReverseGeocodeResult
{
    public string? Status { get; set; }
    public string? FormattedAddress { get; set; }

    // keyed by a single google address component type ("locality", "postal_code", ...), most
    // specific result winning.  google does not guarantee the order of a component's `types`
    // array, so keying on the joined tuple would force callers to guess every permutation.
    public Dictionary<string, ReverseGeocodeValue> Details { get; } = [];

    // kept out of Details: distinct points of interest all share the "point_of_interest" type,
    // so a single dictionary slot could not hold them all.
    public List<ReverseGeocodeValue> PointsOfInterest { get; } = [];
}
