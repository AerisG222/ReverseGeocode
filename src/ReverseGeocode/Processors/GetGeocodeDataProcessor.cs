using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using ReverseGeocode.Data;
using ReverseGeocode.Services;

namespace ReverseGeocode.Processors;

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
        if (File.Exists(_outputFile))
        {
            throw new ApplicationException($"Output file [{_outputFile}] already exists!");
        }

        var sourceRecords = await _db.GetDataToGeocodeAsync().ConfigureAwait(false);

        if (sourceRecords.Count() == 0)
        {
            Console.WriteLine("No records require reverse geocoding, exiting.");
            return;
        }

        var distinctCoordinates = GetDistinctCoordinatesToLookup(sourceRecords);

        Console.WriteLine($"Found { sourceRecords.Count() } to query, containing { distinctCoordinates.Count() } distinct coordinates.");

        var results = await GetGeocodeResults(distinctCoordinates).ConfigureAwait(false);
        var fullResults = BuildResults(sourceRecords, results);

        if (fullResults.Count() > 0)
        {
            Console.WriteLine($"Writing results to { _outputFile }");
            WriteResults(fullResults);
        }
        else
        {
            Console.WriteLine("No rec");
        }

        Console.WriteLine("Completed.");
    }

    IEnumerable<GpsCoordinate> GetDistinctCoordinatesToLookup(IEnumerable<SourceRecord> records)
    {
        return records
            .Select(r => new GpsCoordinate()
            {
                Latitude = r.Latitude,
                Longitude = r.Longitude
            })
            .Distinct();
    }

    IEnumerable<Result> BuildResults(IEnumerable<SourceRecord> records, IEnumerable<(GpsCoordinate coordinate, ReverseGeocodeResult result)> lookupResults)
    {
        var list = new List<Result>();

        foreach (var rec in records)
        {
            var recCoordinate = new GpsCoordinate()
            {
                Latitude = rec.Latitude,
                Longitude = rec.Longitude
            };

            var lookupResult = lookupResults.SingleOrDefault(res => res.coordinate.Equals(recCoordinate));

            if (lookupResult == default)
            {
                Console.WriteLine($"** DID NOT FIND REVERSE GEOCODE RESULT FOR ({ rec.Latitude }, { rec.Longitude })! **");
            }
            else
            {
                list.Add(new Result(rec, lookupResult.result));
            }
        }

        return list;
    }

    async Task<IEnumerable<(GpsCoordinate, ReverseGeocodeResult)>> GetGeocodeResults(IEnumerable<GpsCoordinate> records)
    {
        var list = new List<(GpsCoordinate, ReverseGeocodeResult)>();
        var counter = 0;

        Console.WriteLine("Querying coordinates:");

        foreach (var rec in records)
        {
            var result = await _svc.ReverseGeocodeAsync(rec.Latitude, rec.Longitude);

            list.Add((rec, result));

            if (counter % 200 == 0)
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

        using (var fileWriter = File.CreateText(_outputFile))
        using (var csvWriter = new CsvWriter(fileWriter, CultureInfo.CurrentCulture))
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

        foreach (var field in fields)
        {
            csvWriter.WriteField($"{field}:short");
            csvWriter.WriteField($"{field}:long");
        }
    }

    void WriteDataRows(CsvWriter csvWriter, IEnumerable<string> fields, IEnumerable<Result> results)
    {
        foreach (var result in results)
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

        foreach (var field in fields)
        {
            var shortValue = string.Empty;
            var longValue = string.Empty;

            if (result.GeocodeResult.Details.ContainsKey(field))
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
