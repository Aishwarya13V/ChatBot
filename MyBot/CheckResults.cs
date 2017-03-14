using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using MyBot.FormFlow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyBot
{
    [Serializable]
    public class CheckResults 
    {
        ConnectorClient connector = new ConnectorClient(new Uri("http://localhost:9000/api/messages"));
        public async Task<string> getResult(string Qualification, string field, bool More, IDialogContext context)
        {
            string strRet = "";
            string connectionString = "";
            connectionString = "Data Source=VISHWANATH\\SQLSERVER2014;Initial Catalog=Bot_db;Integrated Security=true;MultipleActiveResultSets=true;User ID=Vishwanath; Password = ";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr;

            switch (Qualification)
            {
                case "Tenth":
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "select PreRequest from UG_PreReq where Field='" + field + "'";
                    strRet += "  \nThese are the groups you can take in intermediate for " + field + " :  \n";
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        strRet += dr["PreRequest"] + " , ";
                    }
                    dr.Close();
                    cmd.Dispose();
                    con.Close();

                    con.Open();
                    cmd = new SqlCommand("select Exam_Name from Exams where Exam_ID IN (select Exam_ID from UG_Fields where Field = '" + field + "')", con);
                    strRet += "  \nHere is the list of exams for graduation in " + field + " :  \n";
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        strRet += dr["Exam_Name"] + " , ";
                    }
                    dr.Close();
                    cmd.Dispose();
                    con.Close();

                    //context.Wait(MessageReceivedAsync);
                    //await StartAsync(context);
                    if (More == true)
                    {
                        con.Open();
                        cmd = new SqlCommand("select Exam_Name from Exams where Exam_ID IN (select Exam_ID from PG_Fields where Field = '" + field + "')", con);
                        strRet += "  \nHere is the list of exams for post graduation in " + field + " :  \n";
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            strRet += dr["Exam_Name"] + " , ";
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                    }
                    //await context.PostAsync("Would you like to knoe more about these fields{||}");

                    break;

                case "Inter":
                    con.Open();
                    cmd = new SqlCommand("select * from UG_PreReq where Field='" + field + "'", con);
                    strRet += "  \nThese are the prerequisites you need in intermediate for " + field + " :  \n";
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        strRet += dr["PreRequest"] + " , ";
                    }

                    dr.Close();
                    cmd.Dispose();
                    con.Close();
                    if (field != null)
                    {
                        con.Open();
                        cmd = new SqlCommand("select Exam_Name from Exams where Exam_ID IN (select Exam_ID from UG_Fields where Field = '" + field + "')", con);
                        strRet += "  \nHere is the list of exams for graduation in " + field + " :  \n";
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            strRet += dr["Exam_Name"] + " , ";
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                    }
                    if (More == true)
                    {
                        con.Open();
                        cmd = new SqlCommand("select Exam_Name from Exams where Exam_ID IN (select Exam_ID from PG_Fields where Field = '" + field + "')", con);
                        strRet += "  \nHere is the list of exams for post graduation in " + field + " :  \n";
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            strRet += dr["Exam_Name"] + " , ";
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                    }
                    break;
                case "Graduate":
                    con.Open();
                    cmd = new SqlCommand("select Specialization from  Specializations where Field = '" + field + "' ", con);
                    strRet += "  \nSepcializations in " + field + " are :  \n";
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        strRet += dr["Specialization"] + " , ";
                    }
                    dr.Close();
                    cmd.Dispose();
                    con.Close();
                    if (More == true)
                    {
                        con.Open();
                        cmd = new SqlCommand("select Exam_Name from Exams where Exam_ID IN (select Exam_ID from PG_Fields where Field = '" + field + "')", con);
                        strRet += "  \nHere is the list of exams for post graduation in " + field + " :  \n";
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            strRet += dr["Exam_Name"] + " , ";
                        }
                        dr.Close();
                        cmd.Dispose();
                        con.Close();
                    }
                    break;
            }
            return strRet;

        }       
    }
}
