using ReverseGeocode.Data;
using ReverseGeocode.Services;


namespace ReverseGeocode.Processors
{
    internal class Result
    {
        public SourceRecord Source { get; private set; }
        public ReverseGeocodeResult GeocodeResult { get; private set; }


        public Result(SourceRecord source, ReverseGeocodeResult result)
        {
            Source = source;
            GeocodeResult = result;
        }
    }
}