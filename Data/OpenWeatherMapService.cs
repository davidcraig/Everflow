using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Everflow.Data
{
    public class OpenWeatherMapService
    {
        private Dictionary<string, OpenWeatherMapGeocodeResponse> geocodeMemo = new Dictionary<string, OpenWeatherMapGeocodeResponse>();
        private string appkey = "";
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public OpenWeatherMapService(HttpClient _httpClient, IConfiguration _config) {
            httpClient = _httpClient;
            config = _config;
            appkey = config.GetValue<string>("OpenWeatherMap:Key");
        }

        protected async Task<OpenWeatherMapGeocodeResponse> CityGeocode(string searchCity)
        {
            if (searchCity == "" || searchCity == null)
            {
                throw new Exception("City not set");
            }

            if (geocodeMemo.TryGetValue(searchCity, out OpenWeatherMapGeocodeResponse memoCode)) {
                return memoCode;
            }

            try
            {
                var response = await httpClient.GetFromJsonAsync<OpenWeatherMapGeocodeResponse>(
                    $"http://api.openweathermap.org/geo/1.0/direct?q={searchCity}&limit=1&appid={appkey}"
                );
                Console.WriteLine(response);
                if (response == null) {
                    throw new Exception("Response not valid");
                } else
                {
                    geocodeMemo.Add(searchCity, response);
                    return response;
                }
                
            }
            catch (JsonException e)
            {
                Console.WriteLine(e.Message);
                throw new Exception($"Geocode exception {e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Geocode exception {e.Message}");
            }
        }

        public async Task<OpenWeatherMapRootobject?> WeatherSearch(string searchCity)
        {
            if (appkey == "" || appkey == null)
            {
                throw new Exception("OpenWeatherMap:Key is not set");
            }
            var geoResponse = await CityGeocode(searchCity);

            if (geoResponse != null)
            {
                try
                {
                    return await httpClient.GetFromJsonAsync<OpenWeatherMapRootobject>(
                        $"https://api.openweathermap.org/data/2.5/forecast?lat={geoResponse.lat}&lon={geoResponse.lon}&units=metric&cnt=5&appid={appkey}"
                    );
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                    throw new Exception($"JSON exception {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); return null;
                    // throw new Exception($"Exception {e.Message}");
                }
            }
            else
            {
                throw new Exception($"geoResponse is not set");
            }
        }
    }

    public class OpenWeatherMapGeocodeResponse
    {
        public required string lat { get; set; }

        public required string lon { get; set; }

        public required string country { get; set; }

        public required string name { get; set; }

        public required Dictionary<string, string> local_names { get; set; }
    }



    public class OpenWeatherMapRootobject
    {
        public string cod { get; set; }
        public int message { get; set; }
        public int cnt { get; set; }
        public List[] list { get; set; }
        public City city { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public int population { get; set; }
        public int timezone { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Coord
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class List
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public Weather[] weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public int visibility { get; set; }
        public float pop { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
        public int humidity { get; set; }
        public float temp_kf { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
        public float gust { get; set; }
    }

    public class Sys
    {
        public string pod { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
}
