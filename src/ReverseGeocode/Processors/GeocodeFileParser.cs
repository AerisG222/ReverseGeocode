using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Humanizer;


namespace ReverseGeocode.Processors
{
    public class GeocodeFileParser
    {
        public IEnumerable<ParsedResult> Parse(string file)
        {
            if(string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if(!File.Exists(file))
            {
                throw new FileNotFoundException("Could not find file to parse", file);
            }

            return ParseFile(file);
        }


        IEnumerable<ParsedResult> ParseFile(string file)
        {
            var list = new List<ParsedResult>();

            using(var fileReader = new StreamReader(file))
            using(var csvReader = new CsvReader(fileReader, CultureInfo.CurrentCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();

                while(csvReader.Read())
                {
                    list.Add(BuildParsedResult(csvReader));
                }
            }

            return list;
        }


        ParsedResult BuildParsedResult(CsvReader reader)
        {
            var result = new ParsedResult();

            foreach(var header in reader.Context.HeaderRecord)
            {
                if(header.EndsWith(":short"))
                {
                    // we currently do not use any short values - only the long ones from the reverse geocoder
                    continue;
                }

                if(string.Equals(header, "Record Type", StringComparison.OrdinalIgnoreCase))
                {
                    result.RecordType = reader.GetField(header);
                    continue;
                }

                if(string.Equals(header, "Record Id", StringComparison.OrdinalIgnoreCase))
                {
                    result.RecordId = long.Parse(reader.GetField(header));
                    continue;
                }

                if(string.Equals(header, "Latitude", StringComparison.OrdinalIgnoreCase))
                {
                    result.Latitude = double.Parse(reader.GetField(header));
                    continue;
                }

                if(string.Equals(header, "Longitude", StringComparison.OrdinalIgnoreCase))
                {
                    result.Longitude = double.Parse(reader.GetField(header));
                    continue;
                }

                if(string.Equals(header, "Status", StringComparison.OrdinalIgnoreCase))
                {
                    result.Status = reader.GetField(header);
                    continue;
                }

                if(string.Equals(header, "Formatted Address", StringComparison.OrdinalIgnoreCase))
                {
                    result.FormattedAddress = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("administrative_area_level_1:", StringComparison.OrdinalIgnoreCase))
                {
                    result.AdministrativeAreaLevel1 = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("administrative_area_level_2:", StringComparison.OrdinalIgnoreCase))
                {
                    result.AdministrativeAreaLevel2 = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("administrative_area_level_3:", StringComparison.OrdinalIgnoreCase))
                {
                    result.AdministrativeAreaLevel3 = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("country:political", StringComparison.OrdinalIgnoreCase))
                {
                    result.Country = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("locality:political", StringComparison.OrdinalIgnoreCase))
                {
                    result.Locality = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("neighborhood:political", StringComparison.OrdinalIgnoreCase))
                {
                    result.Neighborhood = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("political:sublocality:sublocality_level_1", StringComparison.OrdinalIgnoreCase))
                {
                    result.SubLocalityLevel1 = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("political:sublocality:sublocality_level_2", StringComparison.OrdinalIgnoreCase))
                {
                    result.SubLocalityLevel2 = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("postal_code:", StringComparison.OrdinalIgnoreCase))
                {
                    result.PostalCode = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("postal_code_suffix:", StringComparison.OrdinalIgnoreCase))
                {
                    result.PostalCodeSuffix = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("premise:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Premise = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("route:", StringComparison.OrdinalIgnoreCase))
                {
                    result.Route = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("street_number:", StringComparison.OrdinalIgnoreCase))
                {
                    result.StreetNumber = reader.GetField(header);
                    continue;
                }

                if(header.StartsWith("subpremise:", StringComparison.OrdinalIgnoreCase))
                {
                    result.SubPremise = reader.GetField(header);
                    continue;
                }

                // all named properties were handled above.  the following will only process points of interest.
                if(header.IndexOf("establishment:") < 0)
                {
                    // this field is not a point of interest, move along
                    continue;
                }

                var value = reader.GetField(header);

                if(string.IsNullOrWhiteSpace(value))
                {
                    // this record is not associated with this type of POI
                    continue;
                }

                result.PointsOfInterest.Add(new PointOfInterest {
                    Type = GetPointOfInterestType(header),
                    Name = value
                 });
            }

            return result;
        }


        string GetPointOfInterestType(string header)
        {
            header = header
                .Replace(":long", string.Empty)
                .Replace("establishment", string.Empty)
                .Replace("point_of_interest", string.Empty);

            var parts = header.Split(":", StringSplitOptions.RemoveEmptyEntries);

            if(parts.Length == 0)
            {
                return "Point of Interest";
            }

            return parts[0].Titleize();
        }
    }
}
