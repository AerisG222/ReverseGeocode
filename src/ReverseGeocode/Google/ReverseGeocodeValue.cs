namespace ReverseGeocode.Google;

public class ReverseGeocodeValue
{
    // google tags every point of interest with these generic markers, alongside zero or more
    // specific types (transit_station, tourist_attraction, ...) saying what the place actually is.
    const string POINT_OF_INTEREST = "point_of_interest";
    const string ESTABLISHMENT = "establishment";

    public string LongName { get; }
    public string? ShortName { get; }
    public IReadOnlyList<string> Types { get; }

    public bool IsPointOfInterest =>
        Types.Contains(POINT_OF_INTEREST);

    // the useful label for a poi is whatever type google returns beyond the generic markers
    public string? SpecificType =>
        Types.FirstOrDefault(type => type is not POINT_OF_INTEREST and not ESTABLISHMENT);

    public ReverseGeocodeValue(string longName, string? shortName, IReadOnlyList<string> types)
    {
        LongName = longName;
        ShortName = shortName;
        Types = types;
    }
}
