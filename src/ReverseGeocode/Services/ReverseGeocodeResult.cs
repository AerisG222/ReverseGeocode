using System.Collections.Generic;


namespace ReverseGeocode.Services
{
    public class ReverseGeocodeResult
    {
        public string Status { get; set; }
        public string FormattedAddress { get; set; }
        public Dictionary<string, ReverseGeocodeValue> Details { get; } = new Dictionary<string, ReverseGeocodeValue>();
    }
}