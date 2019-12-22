using System;
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


        public Task Process()
        {
            Console.WriteLine("WRITE - not implemented yet");

            return Task.FromResult(0);
        }
    }
}