using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ReverseGeocode.Google;
using ReverseGeocode.MawMedia;
using ReverseGeocode.Models;

namespace ReverseGeocode.Commands;

internal sealed class LoadGeocodeDataCommand
    : AsyncCommand<LoadGeocodeDataCommand.Settings>
{
    const int STATUS_SUCCESS = 0;
    const int STATUS_ERROR = 1;

    readonly MetadataAdapter _adapter = new();

    GoogleMapService _mapService;
    MediaService _mediaService;

    public sealed class Settings
        : CommandSettings
    {
        [CommandOption("-l|--login-url", true)]
        [Description("URL for Auth0 Login")]
        public string Auth0LoginUrl { get; init; } = "";

        [CommandOption("-a|--audience", true)]
        [Description("API Audience to request during Auth0 Login")]
        public string Audience { get; init; } = "";

        [CommandOption("-c|--client-id", true)]
        [Description("Client Id to use when logging into the Media API")]
        public string ClientId { get; init; } = "";

        [CommandOption("-s|--client-secret", true)]
        [Description("Client Secret to use when logging into the Media API")]
        public string ClientSecret { get; init; } = "";

        [CommandOption("-u|--media-api-url", true)]
        [Description("URL for the Media API")]
        public string MediaApiUrl { get; init; } = "";

        [CommandOption("-g|--google-api-key", true)]
        [Description("API key to use when calling Google reverse geocode maps API")]
        public string GoogleApiKey { get; init; } = "";
    }

    public async override Task<int> ExecuteAsync(
        CommandContext context,
        Settings settings,
        CancellationToken token = default
    )
    {
        try
        {
            _mapService = new GoogleMapService(settings.GoogleApiKey);
            _mediaService = new MediaService(settings.MediaApiUrl);

            await _mediaService.Login(
                settings.Auth0LoginUrl,
                settings.Audience,
                settings.ClientId,
                settings.ClientSecret
            );

            var locationsToLookup = await _mediaService.GetLocationsWithoutMetadata();

            if (locationsToLookup.Count() == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No records require reverse geocoding, exiting.[/]");
                return STATUS_SUCCESS;
            }

            AnsiConsole.MarkupLineInterpolated($"[green]Found { locationsToLookup.Count() } locations to query.[/]");

            // we plan to run this once a day - to keep under the google monthly limit of 10k free events / month, limit to
            // 300/day.  (300 * 31 = 9300 - should be more than enough to comfortably stay under our free limit)
            foreach(var location in locationsToLookup.Take(300))
            {
                var lookupResult = await _mapService.ReverseGeocodeAsync(location.Latitude, location.Longitude);
                var metadata = _adapter.ConvertGoogleReponse(location, lookupResult);

                await _mediaService.UpdateMetadata(metadata);
            }

            AnsiConsole.MarkupLine("[green]Completed, exiting.[/]");
            return STATUS_SUCCESS;
        }
        catch(Exception ex)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error: {ex.Message}, exiting.[/]");
            return STATUS_ERROR;
        }
    }
}
