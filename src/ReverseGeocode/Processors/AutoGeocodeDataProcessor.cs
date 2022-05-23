using System;
using System.IO;
using System.Threading.Tasks;

namespace ReverseGeocode.Processors;

public class AutoGeocodeDataProcessor
    : IProcessor
{
    readonly GetGeocodeDataProcessor _getProcessor;
    readonly WriteGeocodeDataProcessor _writeProcessor;
    readonly string _file;

    public AutoGeocodeDataProcessor(GetGeocodeDataProcessor getProcessor, WriteGeocodeDataProcessor writeProcessor, string file)
    {
        _getProcessor = getProcessor ?? throw new ArgumentNullException(nameof(getProcessor));
        _writeProcessor = writeProcessor ?? throw new ArgumentNullException(nameof(writeProcessor));
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    public async Task ProcessAsync()
    {
        await _getProcessor.ProcessAsync();

        if (!File.Exists(_file))
        {
            Console.WriteLine("No reverse geocode records to process, exiting");

            return;
        }

        await _writeProcessor.ProcessAsync();
    }
}
