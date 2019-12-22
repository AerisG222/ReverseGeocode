namespace ReverseGeocode.Data
{
    public class SourceRecord
    {
        public string RecordType { get; set; }
        public long Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}