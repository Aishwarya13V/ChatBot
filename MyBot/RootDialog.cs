using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using MyBot.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        Enquiry enquiry = new Enquiry();
        Prompting prompt = new Prompting();
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (enquiry.count == 0)
            {
                enquiry.count = 1;

                context.Call(FormDialog.FromForm<Enquiry>(Enquiry.BuildEnquiryForm, FormOptions.PromptInStart), async (ctx, formResult) => ctx.Wait(this.MessageReceivedAsync));
            }
            else if (enquiry.count >= 1)
            {                
                context.Call(FormDialog.FromForm<Prompting>(Prompting.BuildPromptingForm, FormOptions.PromptInStart), async (ctx, formResult) => ctx.Wait(this.MessageReceivedAsync));
            }
        }
    }
}