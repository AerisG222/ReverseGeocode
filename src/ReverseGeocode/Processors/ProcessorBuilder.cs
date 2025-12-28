using ReverseGeocode.Data;
using ReverseGeocode.Services;

namespace ReverseGeocode.Processors;

public class ProcessorBuilder
{
    public IProcessor Build(string[] args)
    {
        var mode = args[0];

        if (string.Equals(mode, "AUTO", StringComparison.OrdinalIgnoreCase) && args.Length == 4)
        {
            return BuildAutoProcessor(args[1], args[2], args[3]);
        }

        if (string.Equals(args[0], "GET", StringComparison.OrdinalIgnoreCase) && args.Length == 4)
        {
            return BuildGetProcessor(args[1], args[2], args[3]);
        }

        if (string.Equals(mode, "UPDATE", StringComparison.OrdinalIgnoreCase) && args.Length == 3)
        {
            return BuildWriteProcessor(args[1], args[2]);
        }

        return null;
    }

    public AutoGeocodeDataProcessor BuildAutoProcessor(string connString, string apiKey, string archiveDir)
    {
        if (!Directory.Exists(archiveDir))
        {
            throw new DirectoryNotFoundException($"Directory [{archiveDir}] not found!");
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_hhmmss");
        var outputFile = Path.Combine(archiveDir, $"geocode_{timestamp}.csv");

        var getProcessor = BuildGetProcessor(connString, apiKey, outputFile);
        var writeProcessor = BuildWriteProcessor(connString, outputFile);

        return new AutoGeocodeDataProcessor(getProcessor, writeProcessor, outputFile);
    }

    public GetGeocodeDataProcessor BuildGetProcessor(string connString, string apiKey, string outputFile)
    {
        var db = new DatabaseReader(connString);
        var googleMaps = new GoogleMapService(apiKey);

        return new GetGeocodeDataProcessor(db, googleMaps, outputFile);
    }

    public WriteGeocodeDataProcessor BuildWriteProcessor(string connString, string inputFile)
    {
        var db = new DatabaseWriter(connString);

        return new WriteGeocodeDataProcessor(db, inputFile);
    }
}
