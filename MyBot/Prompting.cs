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
        Enquiry enquiry = new Enquiry();
        public bool More { get; set; }
        internal  string fields;
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
                    string result = await prompt.field_Desc(Prompting.More, context);
                    await context.PostAsync(result);
                })
                
                .Build();
        }
        
        public async Task<string> field_Desc(bool More, IDialogContext context)
        {            
            string strRet = " ";
            fields = "" + context.PrivateConversationData.Get<string>("field");
            //field = enquiry.save.ToString();
            string connectionString = "";            
            connectionString = "Data Source=VISHWANATH\\SQLSERVER2014;Initial Catalog=Bot_db;Integrated Security=true;MultipleActiveResultSets=true;User ID=Vishwanath; Password = ";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr;
            if (More == true)
            {
                con.Open();
                cmd = new SqlCommand("select Exam_Name,Exam_Desc from Exams where Exam_ID IN (select Exam_ID from UG_Fields where Field = '" + fields + "')", con);
                strRet += "  \nHere is the list of exams for " + fields + " :  \n";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    strRet += "**" + dr["Exam_Name"] + "**" + dr["Exam_Desc"];
                }
                dr.Close();
                cmd.Dispose();
                con.Close();
            }
            return strRet;
        }
        
    }
}
