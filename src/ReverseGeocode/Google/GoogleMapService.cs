using RestSharp;

namespace ReverseGeocode.Google;

public class GoogleMapService
    : IDisposable
{
    readonly RestClient _client;

    bool _disposed;

    public GoogleMapService(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        _client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json");
        _client.AddDefaultQueryParameter("key", apiKey);
    }

    public async Task<ReverseGeocodeResult> ReverseGeocodeAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken token = default
    )
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var request = new RestRequest();

        // google expects a '.' decimal separator - interpolating with the current culture would
        // emit ',' in many locales and produce a malformed latlng.
        request.AddQueryParameter("latlng", FormattableString.Invariant($"{latitude},{longitude}"));

        var response = await _client.ExecuteGetAsync<ReverseGeocodeResponse>(request, token);

        if (!response.IsSuccessful)
        {
            throw new ApplicationException(response.ErrorMessage ?? $"Reverse geocode request failed with {response.StatusCode}.");
        }

        if (response.Data is null)
        {
            throw new ApplicationException("Reverse geocode request succeeded but returned no content.");
        }

        return BuildResult(response.Data);
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
            var addressComponents = (response.results ?? [])
                .OrderByDescending(results => results.address_components?.Count ?? 0)
                .ToList();

            result.FormattedAddress = addressComponents.FirstOrDefault()?.formatted_address;

            var components = addressComponents.SelectMany(result => result.address_components ?? []);

            foreach (var component in components)
            {
                if (component.long_name is null)
                {
                    continue;
                }

                var key = BuildKey(component);
                var value = new ReverseGeocodeValue(component.long_name, component.short_name);

                result.Details.TryAdd(key, value);
            }
        }

        return result;
    }

    public void Dispose()
    {
        Dispose(true);

        // no finalizer here - we only own managed state - but a derived type may add one.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _client.Dispose();
        }

        _disposed = true;
    }

    static string BuildKey(AddressComponent ac)
    {
        return string.Join(":", ac.types ?? []);
    }
}
