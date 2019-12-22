using System.Collections.Generic;

namespace ReverseGeocode.Services
{
    public class ReverseGeocodeResponse
    {
        public PlusCode plus_code { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }


    public class PlusCode
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }


    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }


    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }


    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Bounds viewport { get; set; }
    }


    public class Bounds
    {
        public Location northeast { get; set; }
        public Location southwest { get; set; }
    }


    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
