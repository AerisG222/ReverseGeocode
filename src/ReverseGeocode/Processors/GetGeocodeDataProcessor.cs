using System;
using System.Linq;
using System.Threading.Tasks;
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


        public async Task Process()
        {
            var records = await _db.GetDataToGeocode().ConfigureAwait(false);

            records = records.Take(10);

            foreach(var rec in records)
            {
                Console.WriteLine($"{rec.RecordType} | {rec.Id} | {rec.Latitude} | {rec.Longitude}");
            }

            return;
        }
    }
}