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


        public async Task ProcessAsync()
        {
            var parser = new GeocodeFileParser();
            var records = parser.Parse(_inputFile);

            records = records.Where(r => string.Equals(r.Status, "OK", StringComparison.OrdinalIgnoreCase));

            foreach(var r in records)
            {
                await _db.WriteDataAsync(r);
            }
        }
    }
}