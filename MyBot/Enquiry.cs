using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;

namespace MyBot.FormFlow
{
    [Serializable]
    public class Enquiry
    {
        internal int count = 0;
        internal string save;
        [Prompt("What is your current qualification?{||}")]
        public Qualification qualification { get; set; }
        [Prompt("ok.That's good..!! Which of the following field you are interested in/or currently pursuing?{||}")]
        public Field field { get; set; }
        [Prompt("Would you like to know about post graduation details in this field?")]
        public bool More { get; set; }
        public enum Qualification
        {
            Other,Tenth, Inter, Graduate
        }
        public enum Field
        {
            Other, Engineering, Medicine, Law
        }

        public static IForm<Enquiry> BuildEnquiryForm()
        {
            CheckResults Result = new CheckResults();
            Enquiry enquiry = new Enquiry();
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
                    string result = await Result.getResult(Enquiry.qualification.ToString(), Enquiry.field.ToString(), Enquiry.More, context);
                    Enquiry.save = Enquiry.field.ToString();                    
                    await context.PostAsync("I guess this is the suitable path according to your interest" + result, "");
                    
                    //await Conversation.SendAsync(activity, () => { return Chain.From(() => FormDialog.FromForm(Prompting.BuildPromptingForm)); });
                    //await Conversation.SendAsync(Enquiry.activity, () => new NumberGuesserDialog());
                })
                .Build();
        }
    }   
}
