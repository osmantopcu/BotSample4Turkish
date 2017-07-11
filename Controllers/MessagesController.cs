using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Globalization;
using Microsoft.Bot.Builder.Resource;
using System.Resources;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("tr");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("tr");
            

            if (activity.Type == ActivityTypes.Message)
            {

                var accessToken = await GetAuthenticationToken("dfb38a46a5c7439498caaea2084d1132");
                var output = await TranslateText(activity.Text, "en", accessToken);

                int length = 0;
                if (!String.IsNullOrWhiteSpace(output))
                {
                    activity.Text = output;
                    length = (output ?? string.Empty).Length;
                }


                // return our reply to the user
                Activity reply = activity.CreateReply($"You sent {output} which was {length} characters");
                await Microsoft.Bot.Builder.Dialogs.Conversation.SendAsync(activity, () => new RootLuisDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = message.CreateReply($"You are typing.");
                connector.Conversations.ReplyToActivityAsync(reply);
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        static async Task<string> TranslateText(string inputText, string language, string accessToken)
        {
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Hata: " + result;

                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }

        static async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }
    }
}