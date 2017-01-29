using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Luis
{
    public class LUISStockClient
    {
        public static async Task<StockLUIS> ParseUserInput(string strInput)
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                string uri = "https://api.projectoxford.ai/luis/v2.0/apps/23ec4ca7-e8b6-4e08-a40f-d4f3919b264a?subscription-key=6804323b2a6f441cb728ce5b55546882&q=" + strEscaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<StockLUIS>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }
    public class StockLUIS
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public Entity[] entities { get; set; }
        public Dialog dialog { get; set; }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
        public Action[] actions { get; set; }
    }

    public class Action
    {
        public bool triggered { get; set; }
        public string name { get; set; }
        public object[] parameters { get; set; }
    }

    public class Dialog
    {
        public string contextId { get; set; }
        public string status { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
        public Resolution resolution { get; set; }
    }

    public class Resolution
    {
        public string date { get; set; }
    }


}