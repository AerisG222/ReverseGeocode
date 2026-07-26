using ReverseGeocode.Google;
using ReverseGeocode.Models;

namespace ReverseGeocode.Tests;

public class GoogleMapsServiceTests
{
    [Fact(Skip = "api key needed")]
    public async Task GetAndParseResults()
    {
        using var svc = new GoogleMapService("ApiKey", MetadataAdapter.RelevantTypes);

        var results = await svc.ReverseGeocodeAsync(51.501100m, -0.121800m, TestContext.Current.CancellationToken);

        Assert.Equal("Westminster Pier, London SW1A 2JH, UK", results.FormattedAddress);
    }
}
