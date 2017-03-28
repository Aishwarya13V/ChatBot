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
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Bot.Builder.FormFlow;
using MyBot.FormFlow;

namespace Luis
{
    [BotAuthentication]
    [RoutePrefix("api/calling")]
    [Serializable]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            CheckNews news = new CheckNews();
            CheckWeather weather = new CheckWeather();
            CheckResults results = new CheckResults();
            Prompting prompts = new Prompting();
            Enquiry Enquiry = new Enquiry();
            if (activity.Type == ActivityTypes.Message)
            {
                Luis.StockLUIS stLuis = await Luis.LUISStockClient.ParseUserInput(activity.Text);
                string strRet = " ";
                int len = 0;
                //await Conversation.SendAsync(activity, () => new Prompting());

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // Get the stateClient to get/set Bot Data
                //StateClient _stateClient = activity.GetStateClient();
                //BotData _botData = _stateClient.BotState.GetUserData(activity.Type, activity.Text); ;

                //data base connection
                string connectionString = "";
                connectionString = "Data Source=kavya;Initial Catalog=Bot_db;Integrated Security=true;User ID=kavya; Password = ";
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader dr;

                switch (stLuis.topScoringIntent.intent)
                {
                    case "Dialogs":
                        string[] dialog = { "wassuup..!!", "Hello there... how can i help you??", "hello... i'm here to help you" };
                        strRet = dialog[new Random().Next(0, dialog.Length)];
                        break;
                    case "Intro":
                        string[] Intro = { "I'm xxx.You ask anything related to career guidance and apart fromt that you can get trending news and weather forecast.", "Hey..!!!!.I'm xxx.Nice to meet you. You can get career-related information.By the way what's your name.", "I'm the best bot you can ever have" };
                        strRet = Intro[new Random().Next(0, Intro.Length)];
                        break;
                    case "NormalConvo":
                        string[] Convo = { "Don't worry..we will solve it together.", "Tell me what's worrying you", "I guess I can solve it" };
                        strRet = Convo[new Random().Next(0, Convo.Length)];
                        break;
                    case "News":
                        await news.getNews();
                        Activity reply2 = activity.CreateReply("News:");
                        reply2.Attachments = new List<Attachment>();
                        reply2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        for (int i = 0; i < 10; i++)
                        {
                            reply2.Attachments.Add(
                                new HeroCard
                                {
                                    Title = news.title[i],
                                    Subtitle = news.desc[i],
                                    Images = new List<CardImage> {
                                        new CardImage(url:news.urlToImage[i])
                                    },
                                    Buttons = new List<CardAction> {
                                        new CardAction()
                                        {
                                            Value = news.url[i],
                                            Type = "openUrl",
                                            Title = "More"
                                        }
                                    }
                                }.ToAttachment()
                            );
                        }

                        await connector.Conversations.ReplyToActivityAsync(reply2);
                        break;
                    case "Weather":
                        string result = stLuis.entities[0].entity;
                        string strWea = await weather.getWeather(result);
                        string[] words = strWea.Split(',');
                        Activity reply1 = activity.CreateReply("weather:");
                        string subTitle = "Temperature (celcius): " + words[0] + "  \n" + "Condition: " + words[1] + "  \n" + "Humidity: " + words[3];
                        reply1.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        HeroCard card = new HeroCard()
                        {
                            Title = "Current Weather",
                            Subtitle = subTitle,
                            Images = new List<CardImage> {
                            new CardImage(url: "https:"+words[2])
                             }
                        };
                        reply1.Attachments = new List<Attachment> {
                        card.ToAttachment( ) };
                        await connector.Conversations.ReplyToActivityAsync(reply1);
                        break;
                    case "Course":
                        string fields = stLuis.entities[1].entity.ToLower();
                        con.Open();
                        cmd = new SqlCommand("select Course,Course_Desc,Years,Url from Courses where Field = '" + fields + "'", con);
                        dr = cmd.ExecuteReader();
                        string[] Course = new string[15];
                        string[] Course_Desc = new string[15];
                        string[] Years = new string[15];
                        string[] Url1 = new string[15];
                        Activity reply5 = activity.CreateReply(" ");
                        reply5.Attachments = new List<Attachment>();
                        reply5.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        while (dr.Read())
                        {
                            Course[len] = "" + dr["Course"];
                            Course_Desc[len] = "" + dr["Course_Desc"];
                            Years[len] = "" + dr["Years"];
                            Url1[len] = "" + dr["Url"];
                            len++;
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                        for (int i = 0; i < len; i++)
                        {
                            reply5.Attachments.Add(
                                 new ThumbnailCard
                                 {
                                     Title = Course[i],
                                     Subtitle = Course_Desc[i] + "-" + Years[i],
                                     Images = new List<CardImage>
                                     {
                                            new CardImage
                                            {
                                                Url = Url1[i],
                                            }
                                     },
                                 }.ToAttachment()
                            );

                        }
                        await connector.Conversations.ReplyToActivityAsync(reply5);

                        break;
                    case "Specialization":
                        string fieldss = stLuis.entities[1].entity.ToLower();
                        con.Open();
                        cmd = new SqlCommand("select Specialization,S_Desc, Url from Specializations where Field = '" + fieldss + "'", con);
                        dr = cmd.ExecuteReader();
                        string[] Specialization = new string[34];
                        string[] S_Desc = new string[34];
                        string[] Url2 = new string[34];
                        Activity reply4 = activity.CreateReply(" ");
                        reply4.Attachments = new List<Attachment>();
                        reply4.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        while (dr.Read())
                        {

                            Specialization[len] = "" + dr["Specialization"];
                            S_Desc[len] = "" + dr["S_Desc"];
                            Url2[len] = "" + dr["Url"];
                            len++;
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                        for (int i = 0; i < len; i++)
                        {
                            reply4.Attachments.Add(
                                 new ThumbnailCard
                                 {
                                     Title = Specialization[i],
                                     Subtitle = S_Desc[i],
                                     Images = new List<CardImage>
                                    {
                                            new CardImage
                                            {
                                                Url = Url2[i],
                                            }
                                    },
                                 }.ToAttachment()
                            );

                        }
                        await connector.Conversations.ReplyToActivityAsync(reply4);

                        break;
                    case "Exams":
                        string field = stLuis.entities[0].entity.ToLower();
                        con.Open();
                        cmd = new SqlCommand("select Exam_Name,Exam_Desc from Exams where Exam_ID IN (select Exam_ID from UG_Fields where Field = '" + field + "') ", con);
                        dr = cmd.ExecuteReader();
                        string[] Exam_Name = new string[7];
                        string[] Exam_Desc = new string[7];
                        string[] Url3 = new string[7];
                        Activity reply3 = activity.CreateReply(" ");
                        reply3.Attachments = new List<Attachment>();
                        reply3.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        while (dr.Read())
                        {
                            Exam_Name[len] = "" + dr["Exam_Name"];
                            Exam_Desc[len] = "" + dr["Exam_Desc"];
                            Url3[len] = "" + dr["Url"];
                            len++;
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                        for (int i = 0; i < len; i++)
                        {
                            reply3.Attachments.Add(
                                 new ThumbnailCard
                                 {
                                     Title = Exam_Name[i],
                                     Subtitle = Exam_Desc[i],
                                     Images = new List<CardImage>
                                    {
                                            new CardImage
                                            {
                                                Url = Url3[i],
                                            }
                                    },
                                 }.ToAttachment()
                            );

                        }
                        await connector.Conversations.ReplyToActivityAsync(reply3);
                        break;
                    case "Career":
                        await Conversation.SendAsync(activity, () => new RootDialog());                      
                        break;
                    case "None":
                        strRet = "I think you are going off topic. I can help you with career, news and weather. Please ask any queries related to this. ";
                        break;
                    default:
                        string worriedFace = "\U0001F61F";
                        strRet = "Sorry, I am not getting you..." + worriedFace;
                        break;
                }
                if (strRet != " ")
                {
                    // return our reply to the user 
                    Activity reply = activity.CreateReply(strRet);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
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

                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                Activity reply = message.CreateReply($"I'm xxx.You can ask anything related to career guidace and apart from that you can get trending news and weather forecast.");
                connector.Conversations.ReplyToActivityAsync(reply);
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