using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace ReverseGeocode.Data
{
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
                    + "       p.id AS Id, "
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

                    + "SELECT 'video' AS RecordType, "
                    + "       v.id AS Id, "
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
                    + "       ) ";

            return RunAsync(conn => conn.QueryAsync<SourceRecord>(sql));
        }
    }
}
