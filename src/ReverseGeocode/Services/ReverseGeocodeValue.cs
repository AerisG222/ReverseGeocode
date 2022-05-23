namespace ReverseGeocode.Services;

public class ReverseGeocodeValue
{
    public string LongName { get; private set; }
    public string ShortName { get; private set; }

    public ReverseGeocodeValue(string longName, string shortName)
    {
        LongName = longName;
        ShortName = shortName;
    }
}
