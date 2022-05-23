using System;
using System.Threading.Tasks;
using Dapper;
using ReverseGeocode.Processors;


namespace ReverseGeocode.Data;

public class DatabaseWriter
    : Database
{
    public DatabaseWriter(string connString)
        : base(connString)
    {

    }


    public async Task WriteDataAsync(ParsedResult result)
    {
        if (result.IsOverride)
        {
            await DeleteReverseGeocodeOverrideData(result);
            await DeletePointOfInterestOverrideData(result);
        }

        await WriteReverseGeocodeData(result);

        if (result.PointsOfInterest != null && result.PointsOfInterest.Count > 0)
        {
            await WritePointsOfInterest(result);
        }

        if (result.IsOverride)
        {
            await MarkOverrideAsProcessed(result);
        }
    }


    async Task WriteReverseGeocodeData(ParsedResult result)
    {
        var sql = $"INSERT INTO { result.RecordType }.reverse_geocode "
                + $"( "
                + $"  { result.RecordType }_id, "
                + $"  is_override, "
                + $"  formatted_address, "
                + $"  administrative_area_level_1, "
                + $"  administrative_area_level_2, "
                + $"  administrative_area_level_3, "
                + $"  country, "
                + $"  locality, "
                + $"  neighborhood, "
                + $"  sub_locality_level_1, "
                + $"  sub_locality_level_2, "
                + $"  postal_code, "
                + $"  postal_code_suffix, "
                + $"  premise, "
                + $"  route, "
                + $"  street_number, "
                + $"  sub_premise "
                + $") "
                + $"VALUES "
                + $"( "
                + $"  @RecordId, "
                + $"  @IsOverride, "
                + $"  @FormattedAddress, "
                + $"  @AdministrativeAreaLevel1, "
                + $"  @AdministrativeAreaLevel2, "
                + $"  @AdministrativeAreaLevel3, "
                + $"  @Country, "
                + $"  @Locality, "
                + $"  @Neighborhood, "
                + $"  @SubLocalityLevel1, "
                + $"  @SubLocalityLevel2, "
                + $"  @PostalCode, "
                + $"  @PostalCodeSuffix, "
                + $"  @Premise, "
                + $"  @Route, "
                + $"  @StreetNumber, "
                + $"  @SubPremise "
                + $") ";

        try
        {
            await RunAsync(conn => conn.ExecuteAsync(sql, result));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
        }
    }


    async Task WritePointsOfInterest(ParsedResult result)
    {
        var sql = $"INSERT INTO { result.RecordType }.point_of_interest "
                + $"( "
                + $"  { result.RecordType }_id, "
                + $"  is_override, "
                + $"  poi_type, "
                + $"  poi_name "
                + $") "
                + $"VALUES "
                + $"( "
                + $"  @RecordId, "
                + $"  @IsOverride, "
                + $"  @PoiType, "
                + $"  @PoiName "
                + $") ";

        foreach (var poi in result.PointsOfInterest)
        {
            try
            {
                await RunAsync(conn => conn.ExecuteAsync(sql, new
                {
                    result.RecordId,
                    result.IsOverride,
                    PoiType = poi.Type,
                    PoiName = poi.Name
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
            }
        }
    }


    async Task MarkOverrideAsProcessed(ParsedResult result)
    {
        var sql = $"UPDATE { result.RecordType }.gps_override "
                + $"   SET has_been_reverse_geocoded = TRUE "
                + $" WHERE { result.RecordType }_id = { result.RecordId } "
                + $"   AND ROUND(latitude::numeric, 5) = ROUND(CAST({ result.Latitude } AS REAL)::numeric, 5) "
                + $"   AND ROUND(longitude::numeric, 5) = ROUND(CAST({ result.Longitude } AS REAL)::numeric, 5) ";

        try
        {
            await RunAsync(conn => conn.ExecuteAsync(sql));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
        }
    }


    Task DeleteReverseGeocodeOverrideData(ParsedResult result)
    {
        return DeleteOverrideData(result, "reverse_geocode");
    }


    Task DeletePointOfInterestOverrideData(ParsedResult result)
    {
        return DeleteOverrideData(result, "point_of_interest");
    }


    async Task DeleteOverrideData(ParsedResult result, string tablename)
    {
        var sql = $"DELETE FROM { result.RecordType }.{ tablename } "
                + $" WHERE { result.RecordType }_id = { result.RecordId } "
                + $"   AND is_override = TRUE ";

        try
        {
            await RunAsync(conn => conn.ExecuteAsync(sql));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
        }
    }
}
