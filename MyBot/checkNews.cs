using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyBot
{
    public class CheckNews
    {
        newsObject newsObj = new newsObject();
        Articles res = new Articles();
        public string[] desc = new string[10];
        public string[] title = new string[10];
        public string[] url = new string[10];
        public string[] urlToImage = new string[10];
        public async Task getNews()
        {
            using (var newsHttpClient = new HttpClient())
            {
                string uri = $"https://newsapi.org/v1/articles?source=bbc-news&sortBy=top&apiKey={ResourcesManager.newsAppID}";
                HttpResponseMessage msg = await newsHttpClient.GetAsync(uri);
                string sJson = string.Empty;
                sJson = await msg.Content.ReadAsStringAsync();
                newsObj = JsonConvert.DeserializeObject<newsObject>(sJson);
                int i = 0;                
                string[] store = new string[10];
                if (null != newsObj)
                {
                    foreach (Articles r in newsObj.articles)
                    {                                
                        title[i] = r.title;
                        desc[i] = r.description;
                        url[i] = r.url;
                        urlToImage[i] = r.urlToImage;
                        i++;                                              
                    }                    
                }                
            }
        }
    }  
    public class newsObject
    {
        public string status { get; set; }
        public string source { get; set; }
        public string sortBy { get; set; }
        public List<Articles> articles { get; set; }
    }
    
    public class Articles
    {
        public string author { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string urlToImage { get; set; }
        public string publishedAt { get; set; }
    }
    
}
