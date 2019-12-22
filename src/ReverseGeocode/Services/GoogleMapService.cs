using System;
using System.Threading.Tasks;
using RestSharp;


namespace ReverseGeocode.Services
{
    public class GoogleMapService
    {
        readonly RestClient _client;


        public GoogleMapService(string apiKey)
        {
            if(string.IsNullOrEmpty(nameof(apiKey)))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            _client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json");
            _client.AddDefaultQueryParameter("key", apiKey);
        }


        public async Task<ReverseGeocodeResponse> ReverseGeocodeAsync(double latitude, double longitude)
        {
            var request = new RestRequest();

            request.AddQueryParameter("latlng", $"{latitude},{longitude}");

            var response = await _client.ExecuteGetTaskAsync<ReverseGeocodeResponse>(request).ConfigureAwait(false);

            if(response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
                return response.Data;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
            }

            return null;
        }
    }
}