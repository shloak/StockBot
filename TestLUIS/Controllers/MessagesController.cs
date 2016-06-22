using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace TestLUIS
{
    [BotAuthentication]


    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 

        public static bool isPrevious = false;
        public static string prev = string.Empty;

        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {

                StockLUIS stLuis = await LUISStockClient.ParseUserInput(message.Text);
                string ret = string.Empty;
                
                string stockName = message.Text;


                if (stLuis.intents.Count() > 0)
                {
                    switch (stLuis.intents[0].intent)
                    {
                        case "Recheck":
                            if (!isPrevious)
                                ret = "No previous stock";
                            else
                            {
                                ret = await GetStock(prev);
                            }
                            break;
                        case "FindStock":
                            if (stLuis.entities.Count() > 0)
                            {
                                ret = await GetStock(stLuis.entities[0].entity);
                                isPrevious = true;
                                prev = stLuis.entities[0].entity;
                            }
                            else
                                ret = "Not valid stock";
                            break;
                        case "None":
                            ret = "Not valid command";
                            break;
                    }
                }
                else
                {
                    ret = "Not valid command";
                }
                

                Message ReplyMessage = message.CreateReplyMessage(ret);

                return ReplyMessage;
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private async Task<string> GetStock(string strStock)
        {
            string ret = string.Empty;
            double? dblS = await yahoo.GetStockPriceAsync(strStock);

            if (null == dblS)
                ret = string.Format("Stock {0} does not exist", strStock.ToUpper());
            else
                ret = string.Format("Stock {0} has value: {1}", strStock.ToUpper(), dblS);

            return ret;
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}