/*using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;

namespace MyBot
{
    public class weatherBot : LuisDialog<object>
    {
        #region Human Language Base

        string forecastFormat = "Hello, For {0}, it'll be {1} in {2}.. with low temp at {3} and high at {4}..";
        string weatherFormat = "Hello, It's {0} in {1}.. with low temp at {2} and high at {3}..";
        string yesFormat = "Hi..actually yes, it's {0} in {1}.";
        string noFormat = "Hi.. actually No, it's {0} in {1}.";
        string helloFormat = "Hello!! You can ask me in {0} now :-)";

        #endregion
        Luis.StockLUIS stLuis = new Luis.StockLUIS();

        OWeatherMap weatherService = new OWeatherMap();
        string city, time, condition;

        city = stLuis.Entity.Where(ent => ent.type == "Location").FirstOrDefault()?.Entity;
        time = stLuis.Entity.Where(ent => ent.type == "builtin.datetime.date").FirstOrDefault()?.resolution.date;
        
        if (city == null)
        {
            return Message.CreateReplyMessage(":TODO");
            break; //"save state to serve again" return message.CreateReplyMessage("Please specify the location.."); }
        }

        if (time == null)
        {
            time = DateTime.Now.ToShortDateString(); //Default time is now..
        }
        DateTime requestedDt = time.ConvertToDateTime();
        string replyBase;
        if ((requestedDt - DateTime.Now).Days > 0)
        {
            //Forecast Requested
            var weatherForecast = await weatherService.GetForecastData(city, requestedDt, lang);
            List lastDayWeather = weatherForecast.list.Last();
            string description = lastDayWeather.weather.FirstOrDefault()?.description;
            DateTime date = lastDayWeather.dt.ConvertToDateTime();
            string lowAt = Math.Round(lastDayWeather.temp.min) + "°";
            string highAt = Math.Round(lastDayWeather.temp.max) + "°";
            string cityName = "weatherForecast.city.name + ", " + weatherForecast.city.country";
            replyBase = forecastFormat;
            replyBase = string.Format(replyBase, date.ToString("dddd, MMMM, yyyy", new CultureInfo($"{lang},SA")), description, cityName, lowAt, highAt);
        }
        else
        {
            var weather = await weatherService.GetWeatherData(city, lang);
            string description = weather.weather.FirstOrDefault()?.description;
            string lowAt = weather.main.temp_min + "";
            string highAt = weather.main.temp_min + "";
            string cityName = "weather.name + ", " + weather.sys.country";
        
    }
}
*/