using Microsoft.AspNetCore.Components;

namespace Everflow.Data
{
    public class WeatherForecast
    {
        public decimal Temperature { get; set; } = 0;

        public string WeatherDescription { get; set; } = "";

        public string IconUrl { get; set; } = "";

        public DateTime Date { get; set; } = new DateTime();
    }
}