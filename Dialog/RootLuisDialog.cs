using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Globalization;
using System.Threading;

namespace Bot_Application1
{


    [LuisModel("12a168ce-53c3-4108-975a-ce696e7b46b0", "dac286e2891f405d8f1525a9b10dc888")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("tr");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("tr");

            string message = $"Özür dilerim. Ne demek istediğini anlamadım.";
            await context.PostAsync(message);


            PromptDialog.Confirm(
                context: context,
                resume: ResumeAndHandleConfirmAsync,
                prompt: "Emin misin?",
                retry: "Gerçekten?");
        }



        public async Task ResumeAndHandleConfirmAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            bool choicesAreCorrect = await argument;

            if (choicesAreCorrect)
                await context.PostAsync("Kesin yaşanmıştır bu.");
            else
                await context.PostAsync("Peki.");

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Selam");

            context.Wait(this.MessageReceived);
        }

    }
}