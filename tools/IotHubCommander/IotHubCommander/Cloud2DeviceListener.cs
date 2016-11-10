﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.using System;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    /// <summary>
    /// Event Listener Cloud to Device
    /// </summary>
    internal class Cloud2DeviceListener : IHubModule
    {
        ConsoleColor m_FeedbackClr = ConsoleColor.Cyan; 
        ConsoleColor m_MsgRvcClr = ConsoleColor.Cyan;

        private CommandAction Action;
        private string m_ConnStr;
        private DeviceClient m_DeviceClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        public Cloud2DeviceListener(string connStr)
        {
            this.m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr);
            this.m_ConnStr = connStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="action"></param>
        public Cloud2DeviceListener(string connStr, CommandAction action) : this(connStr)
        {
            this.Action = action;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns></returns>
        public Task Execute()
        {
           return starReceivingCommands();
        }

        /// <summary>
        /// Receive the event from cloud 
        /// User can auto Commit, Abandon or None of them, user will be asked 'a' for Abandon and 'c' for Commit
        /// </summary>
        /// <returns></returns>
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
               
                

                try
                {
                    if (Action == CommandAction.Commit)
                    {
                        await m_DeviceClient.CompleteAsync(receivedMessage);

                        Console.ForegroundColor = m_FeedbackClr;
                        Console.WriteLine("Command completed successfully (AutoCommit) :)!");
                        Console.ResetColor();
                    }
                    else if (Action == CommandAction.Abandon)
                    {
                        await m_DeviceClient.AbandonAsync(receivedMessage);

                        Console.ForegroundColor = m_FeedbackClr;
                        Console.WriteLine("Command abandoned successfully :)!");
                        Console.ResetColor();

                    }
                    else if (Action == CommandAction.None)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = m_FeedbackClr;
                        Console.WriteLine("Enter 'a' for Abandon or 'c' for Complete");
                        Console.ResetColor();
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
                    throw new Exception($"{ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Command Action for None, Commint and Abandon
    /// </summary>
    public enum CommandAction
    {
        None = 0,
        Commit = 1,
        Abandon = 2
    }
}
