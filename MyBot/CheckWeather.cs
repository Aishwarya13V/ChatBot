using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace MyBot
{
    public class CheckWeather
    {
        public async Task<string> getWeather(string query)
        {
            using (var newsHttpClient = new HttpClient())
            {
                string uri = $"https://api.apixu.com/v1/current.json?key={ResourcesManager.WMAppID}&q={query}";
                HttpResponseMessage msg = await newsHttpClient.GetAsync(uri);

                string sJson = string.Empty;
                sJson = await msg.Content.ReadAsStringAsync();
                weatherObject weatherObj = new weatherObject();
                weatherObj = JsonConvert.DeserializeObject<weatherObject>(sJson);
                string strRet = "";
                var cur = weatherObj.current;
                var con = weatherObj.current.condition;
                if (null != cur)

                {
                    //strRet = "Temperature in celcius:" + cur.temp_c + "Condition:" + con.text + "Icon:" + con.icon +
                             //"Humidity:" + cur.humidity + "Cloud:" + cur.cloud;
                    string[] arr = { cur.temp_c.ToString(), con.text, con.icon, cur.humidity.ToString()};
                    strRet = string.Join(",", arr);
                }
                return strRet;
            }
        }
    }
    public class weatherObject
    {
        public Location location { get; set; }
        public Current current { get; set; }
    }
    public class Location
    {
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string tz_id { get; set; }
        public int localtime_epoch { get; set; }
        public string localtime { get; set; }
    }
    public class Current
    {
        public int last_updated_epoch { get; set; }
        public string last_updated { get; set; }
        public decimal temp_c { get; set; }
        public decimal temp_f { get; set; }
        public int isday { get; set; }
        public Condition condition { get; set; }
        public decimal wind_mph { get; set; }
        public decimal wind_kph { get; set; }
        public int wind_degree { get; set; }
        public string wind_dir { get; set; }
        public decimal pressure_mb { get; set; }
        public decimal pressure_in { get; set; }
        public decimal precip_mm { get; set; }
        public decimal precip_in { get; set; }
        public int humidity { get; set; }
        public int cloud { get; set; }
        public decimal feelslike_c { get; set; }
        public decimal feelslike_f { get; set; }
    }
    public class Condition
    {
        public string text { get; set; }
        public string icon { get; set; }
        public int code { get; set; }
    }
}