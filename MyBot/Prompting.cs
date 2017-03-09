using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyBot.FormFlow
{
    [Serializable]
    public class Prompting
    {
        //[Prompt("would you like to know more about this field?{||}")]
        public bool More { get; set; }
        public static IForm<Prompting> BuildPromptingForm()
        {
            Prompting prompt = new Prompting();
            return new FormBuilder<Prompting>()
                .Field(new FieldReflector<Prompting>(nameof(Prompting.More))
                    .SetPrompt(new PromptAttribute("would you like to know more about this field?{||}")))
                .OnCompletion(async (context, Prompting) =>
                {
                    context.PrivateConversationData.SetValue<bool>(
                       "More Information", Prompting.More);
                    string result = await prompt.field_Desc(Prompting.More);
                    await context.PostAsync(result);

                })
                
                .Build();
        }
        public async Task<string> field_Desc(bool More)
        {
            string strRet = " ";
            string connectionString = "";
            connectionString = "Data Source=VISHWANATH\\SQLSERVER2014;Initial Catalog=Bot_db;Integrated Security=true;MultipleActiveResultSets=true;User ID=Vishwanath; Password = ";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr;
            if (More == true)
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "select PreRequest from UG_PreReq";
                strRet += "  \nThese are the groups you can take in intermediate for  :  \n";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    strRet += dr["PreRequest"] + " , ";
                }
                dr.Close();
                cmd.Dispose();
                con.Close();
            }
            return strRet;
        }

    }
}