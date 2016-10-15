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
    internal class DeviceEventListener
    {
        ConsoleColor m_Clr = ConsoleColor.Cyan;

        private bool? autoCommit;
        private string connStr;
        private DeviceClient m_DeviceClient;

        public DeviceEventListener(string connStr)
        {
            this.m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr);
            this.connStr = connStr;
        }

        public DeviceEventListener(string connStr, bool? autoCommit) : this(connStr)
        {
            this.autoCommit = autoCommit;
        }

        public Task Execute()
        {
           return starReceivingCommands();
        }

        private async Task starReceivingCommands()
        {
            Console.ForegroundColor = m_Clr;
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

                Console.ForegroundColor = m_Clr;
                Console.WriteLine($"Received message: {receivedMessage.MessageId} - {Encoding.UTF8.GetString(receivedMessage.GetBytes())}");
               
                Console.WriteLine();
                Console.WriteLine("Enter 'a' for Abandon or 'c' for Complete");
                Console.ResetColor();

                string whatTodo = Console.ReadLine();

                try
                {
                    if (whatTodo == "a")
                    {
                        await m_DeviceClient.AbandonAsync(receivedMessage);

                        Console.ForegroundColor = m_Clr;
                        Console.WriteLine("Command abandoned successfully :)!");
                        Console.ResetColor();
                    }
                    else if (whatTodo == "c")
                    {
                        await m_DeviceClient.CompleteAsync(receivedMessage);
                    
                        Console.ForegroundColor = m_Clr;
                        Console.WriteLine("Command completed successfully :)!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = m_Clr;
                        Console.WriteLine("Receiving of commands has been stopped!");
                        Console.ResetColor();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
