using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
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
            if(File.Exists(_outputFile))
            {
                throw new ApplicationException($"Output file [{_outputFile}] already exists!");
            }

            var sourceRecords = await _db.GetDataToGeocodeAsync().ConfigureAwait(false);

            if(sourceRecords.Count() == 0)
            {
                Console.WriteLine("No records require reverse geocoding, exiting.");
                return;
            }

            Console.WriteLine($"Found { sourceRecords.Count() } to query");

            var results = await GetGeocodeResults(sourceRecords).ConfigureAwait(false);

            if(results.Count() > 0)
            {
                Console.WriteLine($"Writing results to { _outputFile }");
                WriteResults(results);
            }
            else
            {
                Console.WriteLine("No rec");
            }

            Console.WriteLine("Completed.");
        }


        async Task<IEnumerable<Result>> GetGeocodeResults(IEnumerable<SourceRecord> records)
        {
            var list = new List<Result>();
            var counter = 0;

            Console.WriteLine("Querying records:");

            foreach(var rec in records)
            {
                var result = await _svc.ReverseGeocodeAsync(rec.Latitude, rec.Longitude);

                list.Add(new Result(rec, result));

                if(counter % 200 == 0)
                {
                    Console.WriteLine($"    {counter} - most recent status: { result.Status }");
                }

                counter++;
            }

            return list;
        }


        void WriteResults(IEnumerable<Result> results)
        {
            var fields = results
                .SelectMany(r => r.GeocodeResult.Details.Keys)
                .Distinct()
                .OrderBy(x => x);

            using(var fileWriter = File.CreateText(_outputFile))
            using(var csvWriter = new CsvWriter(fileWriter, CultureInfo.CurrentCulture))
            {
                WriteHeaderRow(csvWriter, fields);

                csvWriter.NextRecord();

                WriteDataRows(csvWriter, fields, results);
            }
        }


        void WriteHeaderRow(CsvWriter csvWriter, IEnumerable<string> fields)
        {
            csvWriter.WriteField("Record Type");
            csvWriter.WriteField("Record Id");
            csvWriter.WriteField("Is Override");
            csvWriter.WriteField("Latitude");
            csvWriter.WriteField("Longitude");

            csvWriter.WriteField("Status");
            csvWriter.WriteField("Formatted Address");

            foreach(var field in fields)
            {
                csvWriter.WriteField($"{field}:short");
                csvWriter.WriteField($"{field}:long");
            }
        }


        void WriteDataRows(CsvWriter csvWriter, IEnumerable<string> fields, IEnumerable<Result> results)
        {
            foreach(var result in results)
            {
                WriteDataRow(csvWriter, fields, result);

                csvWriter.NextRecord();
            }
        }


        void WriteDataRow(CsvWriter csvWriter, IEnumerable<string> fields, Result result)
        {
            csvWriter.WriteField(result.Source.RecordType);
            csvWriter.WriteField(result.Source.Id);
            csvWriter.WriteField(result.Source.IsOverride);
            csvWriter.WriteField(result.Source.Latitude);
            csvWriter.WriteField(result.Source.Longitude);

            csvWriter.WriteField(result.GeocodeResult.Status);
            csvWriter.WriteField(result.GeocodeResult.FormattedAddress);

            foreach(var field in fields)
            {
                var shortValue = string.Empty;
                var longValue = string.Empty;

                if(result.GeocodeResult.Details.ContainsKey(field))
                {
                    var val = result.GeocodeResult.Details[field];

                    shortValue = val.ShortName;
                    longValue = val.LongName;
                }

                csvWriter.WriteField(shortValue);
                csvWriter.WriteField(longValue);
            }
        }
    }
}