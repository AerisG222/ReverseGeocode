using System;
using System.Linq;
using System.Threading.Tasks;
using ReverseGeocode.Data;
using ReverseGeocode.Services;


namespace ReverseGeocode.Processors
{
    public class GetGeocodeDataProcessor
        : IProcessor
    {
        readonly DatabaseReader _db;
        readonly GoogleMapService _svc;
        readonly string _outputFile;


        public GetGeocodeDataProcessor(DatabaseReader db, GoogleMapService svc, string outputFile)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _svc = svc ?? throw new ArgumentNullException(nameof(svc));
            _outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
        }


        public async Task ProcessAsync()
        {
            var records = await _db.GetDataToGeocodeAsync().ConfigureAwait(false);

            records = records.Take(1);

            foreach(var rec in records)
            {
                Console.WriteLine($"{rec.RecordType} | {rec.Id} | {rec.Latitude} | {rec.Longitude}");

                var response = await _svc.ReverseGeocodeAsync(rec.Latitude, rec.Longitude);
            }

            return;
        }
    }
}