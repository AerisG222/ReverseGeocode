using System;
using System.Threading.Tasks;
using ReverseGeocode.Data;
using ReverseGeocode.Processors;


namespace ReverseGeocode
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length < 1)
            {
                ShowUsage();
                Environment.Exit(1);
            }

            var processor = BuildProcessor(args);

            if(processor == null)
            {
                ShowUsage();
                Environment.Exit(2);
            }

            try
            {
                await processor.Process().ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error encountered when processing: { ex.Message }");
                Environment.Exit(3);
            }
        }


        static IProcessor BuildProcessor(string[] args)
        {
            var mode = args[0];

            if(string.Equals(mode, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return BuildGetProcessor(args);
            }

            if(string.Equals(mode, "UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                return BuildWriteProcessor(args);
            }

            return null;
        }


        static GetGeocodeDataProcessor BuildGetProcessor(string[] args)
        {
            if(args.Length != 4)
            {
                return null;
            }

            var db = new DatabaseReader(args[1]);
            var apiKey = args[2];
            var outputFile = args[3];

            return new GetGeocodeDataProcessor(db, apiKey, outputFile);
        }


        static WriteGeocodeDataProcessor BuildWriteProcessor(string[] args)
        {
            if(args.Length != 3)
            {
                return null;
            }

            var db = new DatabaseWriter(args[1]);
            var inputFile = args[2];

            return new WriteGeocodeDataProcessor(db, inputFile);
        }


        static void ShowUsage()
        {
            Console.WriteLine("A small utility to acquire Reverse geocoding data for current photos and videos.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("    ReverseGeocode GET <db_conn_str> <google_maps_api_key> <output_file>");
            Console.WriteLine("  - or -");
            Console.WriteLine("    ReverseGeocode UPDATE <db_conn_str> <input_file>");
            Console.WriteLine();
        }
    }
}
