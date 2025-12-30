using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using RestSharp;
using RestSharp.Serializers.Json;
using ReverseGeocode.Models;

namespace ReverseGeocode.MawMedia;

public class MediaService
{
    readonly RestClient _mediaClient;

    public MediaService(string apiUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiUrl);

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        _mediaClient = new RestClient(apiUrl, configureSerialization: s => s.UseSystemTextJson(opts));
    }

    public async Task<IEnumerable<Location>> GetLocationsWithoutMetadata() =>
        await _mediaClient.GetAsync<IEnumerable<Location>>("locations/missing-metadata");

    public async Task UpdateMetadata(LocationMetadata metadata)
    {
        var request = new RestRequest($"locations/{metadata.LocationId}/metadata", Method.Post);
        request.AddJsonBody(metadata);

        var response = await _mediaClient.ExecuteAsync<bool>(request);

        if(!response.IsSuccessful)
        {
            throw new ApplicationException($"Failed to load locations with missing metadata!  Response: {response.ErrorMessage}: {response.StatusCode} - {response.Content}");
        }
    }

    public async Task Login(string loginUrl, string audience, string clientId, string clientSecret)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret);

        using var client = new RestClient(loginUrl);
        var request = new RestRequest("oauth/token", Method.Post);

        request.AddHeader("content-type", "application/json");
        request.AddParameter(
            "application/json",
            $$"""
            {
                "client_id": "{{clientId}}",
                "client_secret": "{{clientSecret}}",
                "audience": "{{audience}}",
                "grant_type": "client_credentials"
            }
            """,
            ParameterType.RequestBody
        );

        var response = await client.ExecuteAsync<LoginResponse>(request);

        if(!response.IsSuccessful)
        {
            throw new ApplicationException($"Did not successfully authenticate!  Response: {response.Content}");
        }

        _mediaClient.AddDefaultHeader("authorization", $"Bearer {response.Data.access_token}");
    }
}
