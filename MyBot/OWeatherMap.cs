using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MyBot
{
    class OWeatherMap
    {

        public async Task<WeatherObject> GetWeatherData(string query)
        {
            try
            {
                using (HttpClient OWMHttpClient = new HttpClient())
                {

                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?q={query}&"));
                    WeatherObject owmResponde = JsonConvert.DeserializeObject<WeatherObject>(response);
                    if (owmResponde != null) return owmResponde;
                    return null;

                }
            }
            catch (Exception)
            {
                return null;

            }
        }


        public async Task<WeatherForecastObject> GetForecastData(string query, DateTime dt)
        {
            try
            {
                int days = (dt - DateTime.Now).Days;

                using (HttpClient OWMHttpClient = new HttpClient())
                {

                    string response = await OWMHttpClient.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/forecast/daily?q={query}&appid={ResourcesManager.OWMAppID}&units=metric&lang=en&cnt={days}"));

                    WeatherForecastObject owmResponde = JsonConvert.DeserializeObject<WeatherForecastObject>(response);
                    if (owmResponde != null) return owmResponde;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }



        }

    }
}