namespace ReverseGeocode.Data
{
    public class SourceRecord
    {
        public string RecordType { get; set; }
        public long Id { get; set; }
        public bool IsOverride { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}