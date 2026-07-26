using Humanizer;
using NodaTime;
using ReverseGeocode.Google;

namespace ReverseGeocode.Models;

public class MetadataAdapter
{
    // the only address component types we persist.  GoogleMapService is handed this set and drops
    // everything else google returns (political, postal_town, plus_code, ...), which would
    // otherwise sit in Details unread.  points of interest are collected separately.
    public static readonly IReadOnlySet<string> RelevantTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "administrative_area_level_1",
        "administrative_area_level_2",
        "administrative_area_level_3",
        "country",
        "locality",
        "neighborhood",
        "sublocality_level_1",
        "sublocality_level_2",
        "postal_code",
        "postal_code_suffix",
        "premise",
        "route",
        "street_number",
        "subpremise"
    };

    public LocationMetadata ConvertGoogleReponse(Location location, ReverseGeocodeResult metadata)
    {
        var pois = BuildPointsOfInterest(metadata);

        return new LocationMetadata(
            location.Id,
            Instant.FromDateTimeUtc(DateTime.UtcNow),
            metadata.FormattedAddress,
            TryGetValue(metadata, "administrative_area_level_1"),
            TryGetValue(metadata, "administrative_area_level_2"),
            TryGetValue(metadata, "administrative_area_level_3"),
            TryGetValue(metadata, "country"),
            TryGetValue(metadata, "locality"),
            TryGetValue(metadata, "neighborhood"),
            TryGetValue(metadata, "sublocality_level_1"),
            TryGetValue(metadata, "sublocality_level_2"),
            TryGetValue(metadata, "postal_code"),
            TryGetValue(metadata, "postal_code_suffix"),
            TryGetValue(metadata, "premise"),
            TryGetValue(metadata, "route"),
            TryGetValue(metadata, "street_number"),
            TryGetValue(metadata, "subpremise"),
            pois
        );
    }

    static IEnumerable<PointOfInterest> BuildPointsOfInterest(ReverseGeocodeResult metadata) =>
        metadata.PointsOfInterest
            .Select(poi => new PointOfInterest(
                poi.SpecificType?.Titleize() ?? "Point of Interest",
                poi.LongName
            ))
            .ToList();

    static string? TryGetValue(ReverseGeocodeResult metadata, string fieldName)
    {
        // asking for a type missing from RelevantTypes would always return null, silently.  fail
        // loudly instead - the tests below cover every field, so drift cannot reach production.
        if (!RelevantTypes.Contains(fieldName))
        {
            throw new ArgumentException(
                $"'{fieldName}' is not listed in {nameof(RelevantTypes)}, so it is never indexed.",
                nameof(fieldName)
            );
        }

        return metadata.Details.TryGetValue(fieldName, out var value) ? value.LongName : null;
    }
}
