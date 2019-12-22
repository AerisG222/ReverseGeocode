using System.Collections.Generic;


namespace ReverseGeocode.Data
{
    public class DatabaseReader
        : Database
    {
        public DatabaseReader(string connString)
            : base(connString)
        {

        }


        public IEnumerable<SourceRecord> GetDataToGeocode()
        {
            var sql = "SELECT 'photo' AS RecordType, "
                    + "       id AS Id, "
                    + "       gps_latitude AS Latitude, "
                    + "       gps_longitude AS Longitude "
                    + "  FROM photo.photo "
                    + " WHERE gps_latitude IS NOT NULL "
                    + "   AND gps_longitude IS NOT NULL "

                    + "UNION"

                    + "SELECT 'video' AS RecordType, "
                    + "       id AS Id, "
                    + "       gps_latitude AS Latitude, "
                    + "       gps_longitude AS Longitude "
                    + "  FROM video.video "
                    + " WHERE gps_latitude IS NOT NULL "
                    + "   AND gps_longitude IS NOT NULL";


        }
    }
}
