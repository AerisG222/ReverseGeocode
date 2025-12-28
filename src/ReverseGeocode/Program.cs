using ReverseGeocode.Processors;

namespace ReverseGeocode;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            ShowUsage();
            Environment.Exit(1);
        }

        var builder = new ProcessorBuilder();

        var processor = builder.Build(args);

        if (processor == null)
        {
            ShowUsage();
            Environment.Exit(2);
        }

        try
        {
            await processor.ProcessAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error encountered when processing: { ex.Message }");
            Environment.Exit(3);
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("A small utility to acquire Reverse geocoding data for current photos and videos.");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("    ReverseGeocode AUTO <db_conn_str> <google_maps_api_key> <archive_directory>");
        Console.WriteLine("  - or -");
        Console.WriteLine("    ReverseGeocode GET <db_conn_str> <google_maps_api_key> <output_file>");
        Console.WriteLine("  - or -");
        Console.WriteLine("    ReverseGeocode UPDATE <db_conn_str> <input_file>");
        Console.WriteLine();
    }
}
