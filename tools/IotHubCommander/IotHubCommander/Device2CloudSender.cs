// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotHubCommander
{
    /// <summary>
    /// Send Event Device to Cloud
    /// </summary>
    internal class Device2CloudSender : IHubModule
    {
        private int commandDelayInSec;
        private string connStr;
        private string evetFile;
        private string templateFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr"> Device Connection string</param>
        /// <param name="commandDelayInSec">Command delay time</param>
        public Device2CloudSender(string connStr, int commandDelayInSec)
        {
            this.connStr = connStr;
            this.commandDelayInSec = commandDelayInSec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connStr">Connection string for device</param>
        /// <param name="commandDelayInSec">Command delay time</param>
        /// <param name="evetFile">csv formated event file. Every value should be ';' separated</param>
        /// <param name="templateFile">Event template</param>
        public Device2CloudSender(string connStr, 
            int commandDelayInSec, 
            string evetFile, 
            string templateFile) : this(connStr, commandDelayInSec)
        {
            this.evetFile = evetFile;
            this.templateFile = templateFile;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns></returns>
        public Task Execute()
        {
            return sendEvent();
        }

        /// <summary>
        /// Send Event to cloud
        /// </summary>
        /// <returns></returns>
        private async Task sendEvent()
        {
            StreamReader readerEventFile = null;
            StreamReader readerTempFile = null;
            try
            {
                readerEventFile = new StreamReader(File.OpenRead(evetFile));
                while (!readerEventFile.EndOfStream)
                {
                    readerTempFile = new StreamReader(File.OpenRead(templateFile));
                    var template = readerTempFile.ReadToEnd();

                    int cnt = 1;
                    var tokens = readerEventFile.ReadLine().Split(';');
                    foreach (var token in tokens)
                    {
                        template = template.Replace(@"@{cnt}", token);
                        cnt++;
                    }
                                      
                    var json = JsonConvert.SerializeObject(template);
                    var message = new Message(Encoding.UTF8.GetBytes(json));
                    message.MessageId = Guid.NewGuid().ToString();
                    DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connStr);

                    await deviceClient.SendEventAsync(message);

                    Console.WriteLine($"{template}{Environment.NewLine} Event has been sent.");
                   // Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
    }
}
