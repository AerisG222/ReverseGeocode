namespace ReverseGeocode;

// raised when one of the upstream services (google maps, the media api, auth0) reports a failure,
// so callers can tell an expected integration fault from a framework one.
public class ReverseGeocodeException
    : Exception
{
    public ReverseGeocodeException()
    {
    }

    public ReverseGeocodeException(string message)
        : base(message)
    {
    }

    public ReverseGeocodeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
