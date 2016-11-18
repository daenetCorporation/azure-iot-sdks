using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    public class FeedbackReceiver : IHubModule
    {
        #region Member Variables 

        private ServiceClient m_ServiceClient;
        private CommandAction m_Action;

        #endregion

        #region Public Methods

        public FeedbackReceiver(string connStr, CommandAction action)
        {
            this.m_ServiceClient = ServiceClient.CreateFromConnectionString(connStr);
            this.m_Action = action;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            await receiveFeedbackAsync();
        }

        #endregion

        #region Peivate Methods 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task receiveFeedbackAsync()
        {
            var feedbackReceiver = m_ServiceClient.GetFeedbackReceiver();

            Helper.WriteLine("\nReceiving c2d feedback from service",ConsoleColor.White);
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;
                Helper.WriteLine($"Received feedback{Environment.NewLine}{feedbackBatch.Records.Select(f => f.StatusCode)}", ConsoleColor.Yellow);

                switch (m_Action)
                {
                    case CommandAction.Abandon:
                        await feedbackReceiver.AbandonAsync(feedbackBatch);
                        Helper.WriteLine($"Command abandoned successfully:)!",ConsoleColor.Yellow);
                        break;
                    case CommandAction.Complete:
                        await feedbackReceiver.CompleteAsync(feedbackBatch);
                        Helper.WriteLine($"Command complete successfully:)!", ConsoleColor.Yellow);
                        break;
                    case CommandAction.None:
                    default:
                        Helper.WriteLine("Enter 'a' for Abandon or 'c' for Complete", ConsoleColor.White);
                        string whatTodo = Console.ReadLine();
                        if (whatTodo == "a")
                        {
                            await feedbackReceiver.AbandonAsync(feedbackBatch);
                            Helper.WriteLine("Command abandoned successfully :)!",ConsoleColor.Yellow);
                        }
                        else if (whatTodo == "c")
                        {
                            await feedbackReceiver.CompleteAsync(feedbackBatch);
                            Helper.WriteLine("Command completed successfully :)!",ConsoleColor.Yellow);
                        }
                        else
                        {
                            Helper.WriteLine("Receiving of commands has been stopped!",ConsoleColor.White);
                        }

                        break;   
                }   
            }
        }

        #endregion
    }
}
