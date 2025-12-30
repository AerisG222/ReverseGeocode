using RestSharp;

namespace ReverseGeocode.Google;

public class GoogleMapService
{
    readonly RestClient _client;

    public GoogleMapService(string apiKey)
    {
        if (string.IsNullOrEmpty(nameof(apiKey)))
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        _client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json");
        _client.AddDefaultQueryParameter("key", apiKey);
    }

    public async Task<ReverseGeocodeResult> ReverseGeocodeAsync(decimal latitude, decimal longitude)
    {
        var request = new RestRequest();

        request.AddQueryParameter("latlng", $"{latitude},{longitude}");

        var response = await _client.ExecuteGetAsync<ReverseGeocodeResponse>(request);

        if (response.IsSuccessful)
        {
            return BuildResult(response.Data);
        }
        else
        {
            throw new ApplicationException(response.ErrorMessage);
        }
    }

    internal ReverseGeocodeResult BuildResult(ReverseGeocodeResponse response)
    {
        var result = new ReverseGeocodeResult
        {
            Status = response.status
        };

        if (string.Equals(response.status, "OK", StringComparison.OrdinalIgnoreCase))
        {
            // order components from most detailed to least
            var addressComponents = response.results
                .OrderByDescending(results => results.address_components.Count)
                .ToList();

            result.FormattedAddress = addressComponents.FirstOrDefault()?.formatted_address;

            var components = addressComponents.SelectMany(result => result.address_components);

            foreach (var component in components)
            {
                var key = BuildKey(component);
                var value = new ReverseGeocodeValue(component.long_name, component.short_name);

                if (!result.Details.ContainsKey(key))
                {
                    result.Details.Add(key, value);
                }
            }
        }

        return result;
    }

    static string BuildKey(AddressComponent ac)
    {
        return string.Join(":", ac.types);
    }
}
