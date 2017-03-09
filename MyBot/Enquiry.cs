using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;

namespace MyBot.FormFlow
{
    [Serializable]
    public class Enquiry
    {
        [Prompt("What is your current qualification?{||}")]
        public Qualification qualification { get; set; }
        [Prompt("ok.That's good..!! Which of the following field you are interested in/or currently pursuing?{||}")]
        public Field field { get; set; }
        [Prompt("Would you like to know about post graduation details in this field?")]
        public bool More { get; set; }

        public enum Qualification
        {
            other, Tenth, Inter, Graduate
        }
        public enum Field
        {
            Other, Engineering, Medicine, Law
        }
        public static IForm<Enquiry> BuildEnquiryForm()
        {
            CheckResults Result = new CheckResults();
            Prompting prompt = new Prompting();
            return new FormBuilder<Enquiry>()
                .Message("If you have difficulty in answering please enter 'help'")
                /*.Field(new FieldReflector<Enquiry>(nameof(Enquiry.field))
                    .SetPrompt(new PromptAttribute("What type of engines does the ship have? {||}")))*/
                .OnCompletion(async (context, Enquiry) =>
                {
                    // Set BotUserData
                    context.PrivateConversationData.SetValue<bool>(
                        "ProfileComplete", true);
                    context.PrivateConversationData.SetValue(
                        "Qualification", Enquiry.qualification.ToString());
                    context.PrivateConversationData.SetValue(
                        "field", Enquiry.field.ToString());
                    context.PrivateConversationData.SetValue<bool>(
                       "More Information", Enquiry.More);
                    // Tell the user that the form is complete
                    /*FormButton fb = new FormButton();
                    fb.Message = "Would you like to know more about these fields";
                    fb.Title = "More";*/
                    string result =  await Result.getResult(Enquiry.qualification.ToString(), Enquiry.field.ToString(), Enquiry.More, context);
                    await context.PostAsync("I guess this is the suitable path according to your interest" + result);
                    //await context.PostAsync("Would you like to know more about these fields");
                })
                .Build();
        }
    }
}
