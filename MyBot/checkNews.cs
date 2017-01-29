using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace MyBot
{
    [Serializable]
    public class checkNews : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            PromptDialog.Confirm(context, askNews, "Are you sure?", "Didn't get that", promptStyle: PromptStyle.Auto);
        }
        private async Task askNews(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm) // They said yes
            {
                string message = $"you can check the above links";
                await context.PostAsync(message);
                context.Wait(MessageReceivedAsync);
            }
            else // They said no
            {
                string msg = $"ok check them later!";
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}