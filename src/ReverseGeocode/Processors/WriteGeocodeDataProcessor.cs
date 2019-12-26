using System;
using System.Linq;
using System.Threading.Tasks;
using ReverseGeocode.Data;


namespace ReverseGeocode.Processors
{
    public class WriteGeocodeDataProcessor
        : IProcessor
    {
        readonly DatabaseWriter _db;
        readonly string _inputFile;


        public WriteGeocodeDataProcessor(DatabaseWriter db, string inputFile)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _inputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
        }


        public Task ProcessAsync()
        {
            var parser = new GeocodeFileParser();
            var records = parser.Parse(_inputFile);

            records = records.Where(r => r.PointsOfInterest.Count == 2).Take(10);

            foreach(var r in records)
            {
                Console.WriteLine($"{r.RecordType} | {r.RecordId} | {r.FormattedAddress} | {r.PointsOfInterest.First().Type} | {r.PointsOfInterest.First().Name}");
            }

            return Task.FromResult(0);
        }
    }
}