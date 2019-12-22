using System;
using ReverseGeocode.Data;


namespace ReverseGeocode.Processors
{
    public class GetGeocodeDataProcessor
        : IProcessor
    {
        readonly DatabaseReader _db;
        readonly string _apiKey;
        readonly string _outputFile;


        public GetGeocodeDataProcessor(DatabaseReader db, string apiKey, string outputFile)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
        }


        public void Process()
        {
            Console.WriteLine("GET - not implemented yet");
        }
    }
}