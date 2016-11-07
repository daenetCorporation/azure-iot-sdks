// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.using System;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    internal class Cloud2DeviceListener : IHubModule
    {
        ConsoleColor m_FeedbackClr = ConsoleColor.Cyan; 
        ConsoleColor m_MsgRvcClr = ConsoleColor.Cyan;

        private CommandAction Action;
        private string m_ConnStr;
        private DeviceClient m_DeviceClient;

        public Cloud2DeviceListener(string connStr)
        {
            this.m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr);
            this.m_ConnStr = connStr;
        }

        public Cloud2DeviceListener(string connStr, CommandAction action) : this(connStr)
        {
            this.Action = action;
        }

        public Task Execute()
        {
           return starReceivingCommands();
        }

        private async Task starReceivingCommands()
        {
            Console.ForegroundColor = m_FeedbackClr;
            Console.WriteLine("\nStarted Receiving cloud to device messages from service");
            Console.ResetColor();

            while (true)
            {
                Message receivedMessage = await m_DeviceClient.ReceiveAsync();
                if (receivedMessage == null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Commend receiver timed out. Reconnected...");
                    Console.ResetColor();
                    continue;
                }

                Console.ForegroundColor = m_MsgRvcClr;
                Console.WriteLine($"Received message: {receivedMessage.MessageId} - {Encoding.UTF8.GetString(receivedMessage.GetBytes())}");
               
                Console.WriteLine();
                Console.ForegroundColor = m_FeedbackClr;
                Console.WriteLine("Enter 'a' for Abandon or 'c' for Complete");
                Console.ResetColor();

                try
                {
                    //TODO: You have to check it is autoCommit or not 
                    if (Action == CommandAction.Commit)
                    {
                        await m_DeviceClient.CompleteAsync(receivedMessage);

                        Console.ForegroundColor = m_FeedbackClr;
                        Console.WriteLine("Command completed successfully (AutoCommit) :)!");
                        Console.ResetColor();
                    }
                    else if (Action == CommandAction.Abandon)
                    {
                        // todo abandon
                        Action = CommandAction.Abandon;
                       
                    }
                    else if (Action == CommandAction.None)
                    {
                        string whatTodo = Console.ReadLine();



                        if (whatTodo == "a")
                        {
                            await m_DeviceClient.AbandonAsync(receivedMessage);

                            Console.ForegroundColor = m_FeedbackClr;
                            Console.WriteLine("Command abandoned successfully :)!");
                            Console.ResetColor();
                        }
                        else if (whatTodo == "c")
                        {
                            await m_DeviceClient.CompleteAsync(receivedMessage);

                            Console.ForegroundColor = m_FeedbackClr;
                            Console.WriteLine("Command completed successfully :)!");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = m_FeedbackClr;
                            Console.WriteLine("Receiving of commands has been stopped!");
                            Console.ResetColor();
                            break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    public enum CommandAction
    {
        None = 0,
        Commit = 1,
        Abandon = 2
    }
}
