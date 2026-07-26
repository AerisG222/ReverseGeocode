using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using RestSharp;
using RestSharp.Serializers.Json;
using ReverseGeocode.Models;

namespace ReverseGeocode.MawMedia;

public class MediaService
    : IDisposable
{
    readonly RestClient _mediaClient;

    bool _disposed;

    public MediaService(string apiUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiUrl);

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opts.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        _mediaClient = new RestClient(apiUrl, configureSerialization: s => s.UseSystemTextJson(opts));
    }

    public async Task<IReadOnlyList<Location>> GetLocationsWithoutMetadata(CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await _mediaClient.GetAsync<List<Location>>("locations/missing-metadata", token) ?? [];
    }

    public async Task UpdateMetadata(LocationMetadata metadata, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var request = new RestRequest($"locations/{metadata.LocationId}/metadata", Method.Post);
        request.AddJsonBody(metadata);

        var response = await _mediaClient.ExecuteAsync<bool>(request, token);

        if(!response.IsSuccessful)
        {
            throw new ApplicationException($"Failed to update metadata for location {metadata.LocationId}!  Response: {response.ErrorMessage}: {response.StatusCode} - {response.Content}");
        }
    }

    public async Task Login(
        string loginUrl,
        string audience,
        string clientId,
        string clientSecret,
        CancellationToken token = default
    )
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

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

        var response = await client.ExecuteAsync<LoginResponse>(request, token);

        if(!response.IsSuccessful || response.Data?.access_token is null)
        {
            throw new ApplicationException($"Did not successfully authenticate!  Response: {response.Content}");
        }

        _mediaClient.AddDefaultHeader("authorization", $"Bearer {response.Data.access_token}");
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
            _mediaClient.Dispose();
        }

        _disposed = true;
    }
}
