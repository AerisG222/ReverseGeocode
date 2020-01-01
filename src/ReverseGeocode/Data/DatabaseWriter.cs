using System;
using System.Threading.Tasks;
using Dapper;
using ReverseGeocode.Processors;


namespace ReverseGeocode.Data
{
    public class DatabaseWriter
        : Database
    {
        public DatabaseWriter(string connString)
            : base(connString)
        {

        }


        public async Task WriteDataAsync(ParsedResult result)
        {
            await WriteReverseGecodeData(result);

            if(result.PointsOfInterest != null && result.PointsOfInterest.Count > 0)
            {
                await WritePointsOfInterest(result);
            }
        }


        async Task WriteReverseGecodeData(ParsedResult result)
        {
            var sql = $"INSERT INTO { result.RecordType }.reverse_geocode "
                    + $"( "
                    + $"  { result.RecordType }_id, "
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
            catch(Exception ex)
            {
                Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
            }
        }


        async Task WritePointsOfInterest(ParsedResult result)
        {
            var sql = $"INSERT INTO { result.RecordType }.point_of_interest "
                    + $"( "
                    + $"  { result.RecordType }_id, "
                    + $"  poi_type, "
                    + $"  poi_name "
                    + $") "
                    + $"VALUES "
                    + $"( "
                    + $"  @RecordId, "
                    + $"  @PoiType, "
                    + $"  @PoiName "
                    + $") ";

            foreach(var poi in result.PointsOfInterest)
            {
                try
                {
                    await RunAsync(conn => conn.ExecuteAsync(sql, new {
                        result.RecordId,
                        PoiType = poi.Type,
                        PoiName = poi.Name
                    }));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error processing { result.RecordType } with ID: { result.RecordId }.  Error: { ex.Message }");
                }
            }
        }
    }
}
