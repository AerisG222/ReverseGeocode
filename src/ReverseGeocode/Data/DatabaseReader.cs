using Dapper;

namespace ReverseGeocode.Data;

public class DatabaseReader
    : Database
{
    public DatabaseReader(string connString)
        : base(connString)
    {

    }

    public Task<IEnumerable<SourceRecord>> GetDataToGeocodeAsync()
    {
        var sql = "SELECT 'photo' AS RecordType, "
                + "       FALSE AS IsOverride, "
                + "       p.id, "
                + "       p.gps_latitude AS Latitude, "
                + "       p.gps_longitude AS Longitude "
                + "  FROM photo.photo p "
                + " WHERE p.gps_latitude IS NOT NULL "
                + "   AND p.gps_longitude IS NOT NULL "
                + "   AND NOT EXISTS "
                + "       ( "
                + "           SELECT 1 "
                + "             FROM photo.reverse_geocode rg "
                + "            WHERE p.id = rg.photo_id "
                + "       ) "

                + " UNION "

                + "SELECT 'photo' AS RecordType, "
                + "       TRUE AS IsOverride, "
                + "       photo_id AS Id, "
                + "       latitude, "
                + "       longitude "
                + "  FROM photo.gps_override "
                + " WHERE has_been_reverse_geocoded = FALSE "

                + " UNION "

                + "SELECT 'video' AS RecordType, "
                + "       FALSE AS IsOverride, "
                + "       v.id, "
                + "       v.gps_latitude AS Latitude, "
                + "       v.gps_longitude AS Longitude "
                + "  FROM video.video v "
                + " WHERE gps_latitude IS NOT NULL "
                + "   AND gps_longitude IS NOT NULL "
                + "   AND NOT EXISTS "
                + "       ( "
                + "           SELECT 1 "
                + "             FROM video.reverse_geocode rg "
                + "            WHERE v.id = rg.video_id "
                + "       ) "

                + " UNION "

                + "SELECT 'video' AS RecordType, "
                + "       TRUE AS IsOverride, "
                + "       video_id AS Id, "
                + "       latitude, "
                + "       longitude "
                + "  FROM video.gps_override "
                + " WHERE has_been_reverse_geocoded = FALSE ";

        return RunAsync(conn => conn.QueryAsync<SourceRecord>(sql));
    }
}
