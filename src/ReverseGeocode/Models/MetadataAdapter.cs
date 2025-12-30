using Humanizer;
using NodaTime;
using ReverseGeocode.Google;

namespace ReverseGeocode.Models;

public class MetadataAdapter
{
    public LocationMetadata ConvertGoogleReponse(Location location, ReverseGeocodeResult metadata)
    {
        var pois = BuildPointsOfInterest(metadata);

        return new LocationMetadata(
            location.Id,
            Instant.FromDateTimeUtc(DateTime.UtcNow),
            metadata.FormattedAddress,
            TryGetValue(metadata, "administrative_area_level_1", "administrative_area_level_1:political"),
            TryGetValue(metadata, "administrative_area_level_2", "administrative_area_level_2:political"),
            TryGetValue(metadata, "administrative_area_level_3", "administrative_area_level_3:political"),
            TryGetValue(metadata, "country", "country:political"),
            TryGetValue(metadata, "locality", "locality:political"),
            TryGetValue(metadata, "neighborhood", "neighborhood:political"),
            TryGetValue(metadata, "sublocality_level_1", "political:sublocality_level_1", "political:sublocality:sublocality_level_1"),
            TryGetValue(metadata, "sublocality_level_2", "political:sublocality_level_2", "political:sublocality:sublocality_level_2"),
            TryGetValue(metadata, "postal_code", "political:postal_code"),
            TryGetValue(metadata, "postal_code_suffix", "political:postal_code_suffix"),
            TryGetValue(metadata, "premise", "political:premise"),
            TryGetValue(metadata, "route", "political:route"),
            TryGetValue(metadata, "street_number", "political:street_number"),
            TryGetValue(metadata, "subpremise", "political:subpremise"),
            pois
        );
    }

    static IEnumerable<PointOfInterest> BuildPointsOfInterest(ReverseGeocodeResult metadata)
    {
        List<PointOfInterest> pois = [];

        foreach(var key in metadata.Details.Keys)
        {
            if(!key.Contains("point_of_interest"))
            {
                continue;
            }

            var poiTypeParts = key
                .Replace("point_of_interest", string.Empty)
                .Replace("establishment", string.Empty)
                .Split(':', StringSplitOptions.RemoveEmptyEntries);

            var type = poiTypeParts.Length == 0
                ? "Point of Interest"
                : poiTypeParts[0].Titleize();

            pois.Add(new(type, metadata.Details[key].LongName));
        }

        return pois;
    }

    static string TryGetValue(ReverseGeocodeResult metadata, params string[] fieldNames)
    {
        foreach(var fieldName in fieldNames)
        {
            if(metadata.Details.TryGetValue(fieldName, out var value))
            {
                return value.LongName;
            }
        }

        return null;
    }
}
