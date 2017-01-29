using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using System.Net;
using Microsoft.Bot.Builder.Dialogs;
using MyBot;
using System.Linq;
using System.Globalization;
using System.Web.Services.Description;

namespace Luis
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        #region Human Language Base

        string forecastFormat = "Hello, For {0}, it'll be {1} in {2}.. with low temp at {3} and high at {4}..";
        string weatherFormat = "Hello, It's {0} in {1}.. with low temp at {2} and high at {3}..";
        string yesFormat = "Hi..actually yes, it's {0} in {1}.";
        string noFormat = "Hi.. actually No, it's {0} in {1}.";
        string helloFormat = "Hello!! You can ask me in {0} now :-)";

        #endregion

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            OWeatherMap weatherService = new OWeatherMap();
            string city, time, condition;

            if (activity.Type == ActivityTypes.Message)
            {
                Luis.StockLUIS stLuis = await Luis.LUISStockClient.ParseUserInput(activity.Text);
                string strRet = string.Empty;
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // Get the stateClient to get/set Bot Data
                StateClient _stateClient = activity.GetStateClient();
                BotData _botData = _stateClient.BotState.GetUserData(activity.ChannelId, activity.Conversation.Id);

                switch (stLuis.topScoringIntent.intent)
                {
                    case "Dialogs":
                        string[] dialog = { "wassuup..!!","Hello there... how can i help you??","hello... i'm here to help you" };
                        strRet = dialog[new Random().Next(0,dialog.Length)];
                        break;
                    case "News":
                         await Conversation.SendAsync(activity, () => new checkNews());
                        //strRet = await checkNews.StartAsync(activity.Type);
                        break;
                    case "Weather":
                        city = stLuis.entities.Where(ent => ent.type == "Location").FirstOrDefault()?.entity;
                        time = stLuis.entities.Where(ent => ent.type == "builtin.datetime.date").FirstOrDefault()?.resolution.date;

                       /*if (city == null)
                        {
                            return Message.CreateReplyMessage(":TODO");
                            break; //"save state to serve again" return message.CreateReplyMessage("Please specify the location.."); }
                        }

                        if (time == null)
                        {
                            time = DateTime.Now.ToShortDateString(); //Default time is now..
                        }*/
                        DateTime requestedDt = Convert.ToDateTime(time);
                        if ((requestedDt - DateTime.Now).Days > 0)
                        {
                            //Forecast Requested
                            var weatherForecast = await weatherService.GetForecastData(city, requestedDt);

                            List lastDayWeather = weatherForecast.list.Last();

                            string description = lastDayWeather.weather.FirstOrDefault()?.description;
                            DateTime date = Convert.ToDateTime(lastDayWeather.dt);
                            string lowAt = Math.Round(lastDayWeather.temp.min) + "°";
                            string highAt = Math.Round(lastDayWeather.temp.max) + "°";
                            string cityName = weatherForecast.city.name + ", " + weatherForecast.city.country;
                            strRet = forecastFormat;
                            strRet = string.Format(strRet, date.ToString("dddd, MMMM, yyyy"), description, cityName, lowAt, highAt);

                        }
                        else
                        {
                            var weather = await weatherService.GetWeatherData(city);

                            string description = weather.weather.FirstOrDefault()?.description;
                            string lowAt = weather.main.temp_min + "";
                            string highAt = weather.main.temp_min + "";
                            string cityName = weather.name + ", " + weather.sys.country;
                            // Build a reply message
                            strRet = weatherFormat;
                            strRet = string.Format(strRet, description, cityName, lowAt, highAt);

                        }
                        break;
                    
                    default:
                        strRet = "Sorry, I am not getting you..." ;
                        break;
                }
                

            // return our reply to the user 
            Activity reply = activity.CreateReply(strRet);
            await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}