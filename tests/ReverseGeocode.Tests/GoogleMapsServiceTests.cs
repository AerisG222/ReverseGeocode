using ReverseGeocode.Google;

namespace ReverseGeocode.Tests;

public class GoogleMapsServiceTests
{
    [Fact(Skip = "api key needed")]
    public async Task GetAndParseResults()
    {
        var svc = new GoogleMapService("ApiKey");

        var results = await svc.ReverseGeocodeAsync(51.501100m, -0.121800m);

        Assert.Equal("Westminster Pier, London SW1A 2JH, UK", results.FormattedAddress);
    }
}
